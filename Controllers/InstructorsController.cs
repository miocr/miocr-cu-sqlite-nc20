using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Controllers
{
  public class InstructorsController : Controller
  {
    private readonly SchoolContext _context;

    public InstructorsController(SchoolContext context)
    {
      _context = context;
    }

    // GET: Instructors
    public async Task<IActionResult> Index(int? id, int? courseID)
    {
      var viewModel = new InstructorIndexData();
      viewModel.Instructors = await _context.Instructors
            .Include(i => i.OfficeAssignment)
            .Include(i => i.CoursesAssign)
              .ThenInclude(i => i.Course)
            .Include(i => i.CoursesAssign)
              .ThenInclude(i => i.Course)
                  .ThenInclude(i => i.Department)
                      .ThenInclude(i => i.Administrator)
            .OrderBy(i => i.LastName)
            .ToListAsync();

      if (id != null)
      {
        ViewData["InstructorID"] = id.Value;
        Instructor instructor = viewModel.Instructors.Where(
            _instructor => _instructor.ID == id.Value).Single();

        viewModel.Courses = instructor.CoursesAssign.Select(_courseAssing => _courseAssing.Course);

        /*
        viewModel.Courses = instructor.CoursesAssign.Select(
            delegate (CourseAssignment ca){return ca.Course;});
        */
      }

      if (courseID != null)
      {
        ViewData["CourseID"] = courseID.Value;

        // // lazy loading
        // _context.Enrollments
        //     .Include(i => i.Student)
        //     .Where(c => c.CourseID == courseID.Value).Load();

        // /* data fill to viewModel */
        // viewModel.Enrollments = viewModel.Courses.Where(
        //     x => x.CourseID == courseID).Single().Enrollments;

        // all in one (lazy loading and data fill to viewModel)
        viewModel.Enrollments = _context.Enrollments
             .Include(i => i.Student)
             .Where(w => w.CourseID == courseID.Value)
             .ToList();
      }

      return View(viewModel);
    }

    // GET: Instructors/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.ID == id.Value);
      if (instructor == null)
      {
        return NotFound();
      }

      return View(instructor);
    }

    // GET: Instructors/Create
    public IActionResult Create()
    {
      var instructor = new Instructor();
      instructor.CoursesAssign = new List<CourseAssignment>();
      PopulateAssignedCourseData(instructor);
      return View();
    }

    // POST: Instructors/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstMidName,HireDate,LastName,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
    {
      if (selectedCourses != null)
      {
        instructor.CoursesAssign = new List<CourseAssignment>();
        foreach (var course in selectedCourses)
        {
          //var courseToAdd = new CourseAssignment { InstructorID = instructor.ID, CourseID = int.Parse(course) };
          CourseAssignment courseToAdd = new CourseAssignment();
          courseToAdd.InstructorID = instructor.ID;
          courseToAdd.CourseID = int.Parse(course);
          instructor.CoursesAssign.Add(courseToAdd);
        }
      }
      if (ModelState.IsValid)
      {
        _context.Add(instructor);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(instructor);
    }
    // GET: Instructors/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var instructor = await _context.Instructors
          .Include(i => i.OfficeAssignment)
          .Include(i => i.CoursesAssign).ThenInclude(i => i.Course)
          .AsNoTracking()
          .SingleOrDefaultAsync(m => m.ID == id);
      if (instructor == null)
      {
        return NotFound();
      }
      PopulateAssignedCourseData(instructor);
      return View(instructor);
    }

    private void PopulateAssignedCourseData(Instructor instructor)
    {
      var allCourses = _context.Courses.Include(d => d.Department).Include(i => i.Department.Administrator);
      var allDepartments = _context.Departments;
      //var instructorCourses = new HashSet<int>(instructor.CoursesAssign.Select(c => c.Course.CourseID));
      var instructorCourses = new HashSet<Course>(instructor.CoursesAssign.Select(c => c.Course));
      var viewModel = new List<AssignedCourseData>();
      foreach (var course in allCourses)
      {
        string courseAdminName = String.Empty;
        if (course.Department != null && course.Department.Administrator != null)
          courseAdminName = course.Department.Administrator.FullName;

        bool isAssigned = false;
        foreach (Course instructorCourse in instructorCourses)
        {
            if (course.CourseID == instructorCourse.CourseID)
            {
                isAssigned = true;
                break;
            }
        }
        viewModel.Add(new AssignedCourseData
        {
          CourseID = course.CourseID,
          Title = course.Title,
          Admin = courseAdminName,
          //Assigned = instructorCourses.Contains(course.CourseID)
          Assigned = isAssigned
      });
    }
    ViewData["Courses"] = viewModel;
        }

  // POST: Instructors/Edit/5
  // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
  // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
  {
    if (id == null)
    {
      return NotFound();
    }

    var instructorToUpdate = await _context.Instructors
        .Include(i => i.OfficeAssignment)
        .Include(i => i.CoursesAssign)
            .ThenInclude(i => i.Course)
        .SingleOrDefaultAsync(m => m.ID == id);

    if (await TryUpdateModelAsync<Instructor>(
        instructorToUpdate,
        "",
        i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
    {
      if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
      {
        instructorToUpdate.OfficeAssignment = null;
      }
      UpdateInstructorCourses(selectedCourses, instructorToUpdate);
      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateException /* ex */)
      {
        //Log the error (uncomment ex variable name and write a log.)
        ModelState.AddModelError("", "Unable to save changes. " +
            "Try again, and if the problem persists, " +
            "see your system administrator.");
      }
      return RedirectToAction("Index");
    }
    return View(instructorToUpdate);
  }
  private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
  {
    if (selectedCourses == null)
    {
      instructorToUpdate.CoursesAssign = new List<CourseAssignment>();
      return;
    }

    var selectedCoursesHS = new HashSet<string>(selectedCourses);
    var instructorCourses = new HashSet<int>
        (instructorToUpdate.CoursesAssign.Select(c => c.Course.CourseID));
    foreach (var course in _context.Courses)
    {
      if (selectedCoursesHS.Contains(course.CourseID.ToString()))
      {
        if (!instructorCourses.Contains(course.CourseID))
        {
          instructorToUpdate.CoursesAssign.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.CourseID });
        }
      }
      else
      {

        if (instructorCourses.Contains(course.CourseID))
        {
          CourseAssignment courseToRemove = instructorToUpdate.CoursesAssign.SingleOrDefault(i => i.CourseID == course.CourseID);
          _context.Remove(courseToRemove);
        }
      }
    }
  }

  // GET: Instructors/Delete/5
  public async Task<IActionResult> Delete(int? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.ID == id);
    if (instructor == null)
    {
      return NotFound();
    }

    return View(instructor);
  }

  // POST: Instructors/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    Instructor instructor = await _context.Instructors
        .Include(i => i.OfficeAssignment)
        .Include(i => i.CoursesAssign)
        .SingleAsync(i => i.ID == id);


    var departments = await _context.Departments
        .Where(d => d.InstructorID == id)
        .ToListAsync();
    departments.ForEach(d => d.InstructorID = null);

    _context.Instructors.Remove(instructor);

    await _context.SaveChangesAsync();
    return RedirectToAction("Index");
  }

  private bool InstructorExists(int id)
  {
    return _context.Instructors.Any(e => e.ID == id);
  }
}
}

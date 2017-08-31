using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ContosoUniversity
{
  public class Program
  {
    public static void Main(string[] args)
    {
#region Net.Core 1
      // var host = new WebHostBuilder()
      //     .UseKestrel()
      //     .UseContentRoot(Directory.GetCurrentDirectory())
      //     .UseIISIntegration()
      //     .UseStartup<Startup>()
      //     .Build();
      // host.Run();
#endregion

#region Net.Core 2
      BuildWebHost(args).Run();
      //note
      //note
      int i = 0;
#endregion

    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
       .UseStartup<Startup>()
       .Build();
  }
}

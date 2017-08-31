// Write your Javascript code.

$(document).ready(function () {
    $.validator.methods.date = function(value, element) {
        var date_regex = /^([1-31]|0[1-9]|1\d|2\d|3[01]).([1-12]|0[1-9]|1[0-2]).(19|20)\d{2}$/ ;
        var result = this.optional(element) || date_regex.test(value);
        return result;
    };
});
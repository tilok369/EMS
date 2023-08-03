
using System.Net.Mail;

namespace EMS.Model.Validators;

public class EmployeeValidator : IEmployeeValidator
{
    public (bool Success, string Message) Validate(Employee employee)
    {
        if (employee == null)
            return (false, "No employee selected!");
        if (string.IsNullOrEmpty(employee.name))
            return (false, "Name cannot be empty");
        if (string.IsNullOrEmpty(employee.email))
            return (false, "Email cannot be empty");
        if (!IsValidEmailAddress(employee.email))
            return (false, "Invalid email address");
        if (string.IsNullOrEmpty(employee.gender))
            return (false, "Gender cannot be empty");
        if (string.IsNullOrEmpty(employee.status))
            return (false, "Status cannot be empty");

        return (true, "");
    }

    public bool IsValidEmailAddress(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

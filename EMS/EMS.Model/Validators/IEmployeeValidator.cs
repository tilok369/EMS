
namespace EMS.Model.Validators;

public interface IEmployeeValidator
{
    (bool Success, string Message) Validate(Employee employee);
}

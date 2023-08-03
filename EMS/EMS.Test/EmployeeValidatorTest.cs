using EMS.Model.Validators;
using NUnit.Framework;

namespace EMS.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Validate_NullEmployee_ValidationFailed()
        {
            var employeeValidator = new EmployeeValidator();
            var validateResult = employeeValidator.Validate(null);

            Assert.IsFalse(validateResult.Success);
            Assert.AreEqual(validateResult.Message, "No employee selected!");
        }

        [Test]
        public void Validate_EmployeeWithEmptyName_ValidationFailed()
        {
            var employeeValidator = new EmployeeValidator();
            var validateResult = employeeValidator.Validate(
                new Model.Employee { id = 12345, name = string.Empty, email = "test@gmail.com", gender = "male", status = "active"});

            Assert.IsFalse(validateResult.Success);
            Assert.AreEqual(validateResult.Message, "Name cannot be empty");
        }

        [Test]
        public void Validate_EmployeeWithEmptyEmail_ValidationFailed()
        {
            var employeeValidator = new EmployeeValidator();
            var validateResult = employeeValidator.Validate(
                new Model.Employee { id = 12345, name = "test", email = string.Empty, gender = "male", status = "active" });

            Assert.IsFalse(validateResult.Success);
            Assert.AreEqual(validateResult.Message, "Email cannot be empty");
        }

        [Test]
        public void Validate_EmployeeWithInvalidEmail_ValidationFailed()
        {
            var employeeValidator = new EmployeeValidator();
            var validateResult = employeeValidator.Validate(
                new Model.Employee { id = 12345, name = "test", email = "test.com", gender = "male", status = "active" });

            Assert.IsFalse(validateResult.Success);
            Assert.AreEqual(validateResult.Message, "Invalid email address");
        }

        [Test]
        public void Validate_EmployeeWithEmptyGender_ValidationFailed()
        {
            var employeeValidator = new EmployeeValidator();
            var validateResult = employeeValidator.Validate(
                new Model.Employee { id = 12345, name = "test", email = "test@gmail.com", gender = string.Empty, status = "active" });

            Assert.IsFalse(validateResult.Success);
            Assert.AreEqual(validateResult.Message, "Gender cannot be empty");
        }

        [Test]
        public void Validate_EmployeeWithEmptyStatus_ValidationFailed()
        {
            var employeeValidator = new EmployeeValidator();
            var validateResult = employeeValidator.Validate(
                new Model.Employee { id = 12345, name = "test", email = "test@gmail.com", gender = "male", status = string.Empty });

            Assert.IsFalse(validateResult.Success);
            Assert.AreEqual(validateResult.Message, "Status cannot be empty");
        }
    }
}
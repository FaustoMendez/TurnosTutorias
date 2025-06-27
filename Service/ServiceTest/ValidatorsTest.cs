using System;
using Xunit;
using Data;
using Data.Resources;

namespace ServiceTests
{
    public class ValidatorsTests
    {
        [Theory]
        [InlineData("Juan", "firstName")]
        [InlineData("Maria Jose", "firstName")]
        public void ValidateFirstName_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidateFirstName(value, paramName);
        }

        [Theory]
        [InlineData("", "firstName")]
        [InlineData("ThisNameIsWayTooLongToBeValid123", "firstName")]
        [InlineData("John123", "firstName")]
        public void ValidateFirstName_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(() => Validators.ValidateFirstName(value, paramName));
        }

        [Theory]
        [InlineData("Smith", "name")]
        [InlineData("OConnor", "name")]
        public void ValidateName_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidateName(value, paramName);
        }

        [Theory]
        [InlineData("", "name")]
        [InlineData("Smith123", "name")]
        [InlineData("VeryLongNameExceedingTwentyChars", "name")]
        public void ValidateName_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(() => Validators.ValidateName(value, paramName));
        }

        [Theory]
        [InlineData("T001", "tutorId")]
        [InlineData("T999", "tutorId")]
        public void ValidateTutorId_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidateTutorId(value, paramName);
        }

        [Theory]
        [InlineData("", "tutorId")]
        [InlineData("001", "tutorId")]
        [InlineData("T01", "tutorId")]
        [InlineData("t001", "tutorId")]
        public void ValidateTutorId_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(() => Validators.ValidateTutorId(value, paramName));
        }

        [Theory]
        [InlineData("1234567890", "phone")]
        [InlineData("1", "phone")]
        public void ValidatePhone_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidatePhone(value, paramName);
        }

        [Theory]
        [InlineData("", "phone")]
        [InlineData("12345678901", "phone")]
        [InlineData("123-456", "phone")]
        public void ValidatePhone_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(() => Validators.ValidatePhone(value, paramName));
        }

        [Theory]
        [InlineData("user@uv.mx", "email")]
        [InlineData("user@estudiantes.uv.mx", "email")]
        [InlineData("user@gmail.com", "email")]
        public void ValidateEmail_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidateEmail(value, paramName);
        }

        [Theory]
        [InlineData("", "email")]
        [InlineData("user@domain.com", "email")]
        [InlineData("not-an-email", "email")]
        public void ValidateEmail_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(() => Validators.ValidateEmail(value, paramName));
        }

        [Theory]
        [InlineData("AB12345678", "matricula")]
        [InlineData("XY00000000", "matricula")]
        public void ValidateMatricula_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidateMatricula(value, paramName);
        }

        [Theory]
        [InlineData("", "matricula")]
        [InlineData("A12345678", "matricula")]
        [InlineData("ABC12345678", "matricula")]
        public void ValidateMatricula_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(() => Validators.ValidateMatricula(value, paramName));
        }

        [Theory]
        [InlineData("Abcdef1!", "password")]
        [InlineData("Strong#Pass1", "password")]
        public void ValidatePassword_Valid_DoesNotThrow(string value, string paramName)
        {
            Validators.ValidatePassword(value, paramName);
        }

        [Theory]
        [InlineData("", "password")]               // vacía
        [InlineData("short1!", "password")]        // longitud < 8
        [InlineData("alllowercase1!", "password")] // sin mayúscula
        [InlineData("NoSymbol123", "password")]    // sin símbolo
        public void ValidatePassword_Invalid_Throws(string value, string paramName)
        {
            Assert.Throws<ArgumentException>(
                () => Validators.ValidatePassword(value, paramName));
        }
    }
}

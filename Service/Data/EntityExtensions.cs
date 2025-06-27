using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Data.Resources;

namespace Data
{
    public static class Validators
    {
        private static readonly Regex LettersOnly =
            new Regex("^[A-Za-z]{1,20}$", RegexOptions.Compiled);

        private static readonly Regex PhoneOnly =
            new Regex("^\\d{1,10}$", RegexOptions.Compiled);

        private static readonly Regex EmailPattern =
            new Regex(@"^[^@\s]+@(?:(?:estudiantes\.)?uv\.mx|gmail\.com|outlook\.com|hotmail\.com)$",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MatriculaPattern =
            new Regex(@"^[A-Za-z]{2}\d{8}$", RegexOptions.Compiled);

        private static readonly Regex PasswordPattern =
            new Regex(@"^(?=.{8,20}$)(?=.*[A-Z])(?=.*[^A-Za-z0-9]).+$", RegexOptions.Compiled);

        private static readonly Regex TutorIdPattern =
            new Regex(@"^T\d{3}$", RegexOptions.Compiled);

        private static readonly Regex NamePattern =
            new Regex(@"^[A-Za-z]+(?:\s[A-Za-z]+)*$", RegexOptions.Compiled);

        public static void ValidateFirstName(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Resources.Validators.Name_Required, paramName);

            var trimmed = value.Trim();
            if (trimmed.Length > 20 || !NamePattern.IsMatch(trimmed))
                throw new ArgumentException(Resources.Validators.Name_InvalidCharacters, paramName);
        }

        public static void ValidateName(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Resources.Validators.Name_Required, paramName);

            if (!LettersOnly.IsMatch(value.Trim()))
                throw new ArgumentException(Resources.Validators.Name_InvalidCharacters, paramName);
        }

        public static void ValidateTutorId(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Resources.Validators.TutorId_Required, paramName);

            if (!TutorIdPattern.IsMatch(value.Trim()))
                throw new ArgumentException(Resources.Validators.TutorId_InvalidFormat, paramName);
        }

        public static void ValidatePhone(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Resources.Validators.Phone_Required, paramName);

            if (!PhoneOnly.IsMatch(value.Trim()))
                throw new ArgumentException(Resources.Validators.Phone_InvalidFormat, paramName);
        }

        public static void ValidateEmail(string email, string paramName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(Resources.Validators.Email_Required, paramName);

            if (!EmailPattern.IsMatch(email.Trim()))
                throw new ArgumentException(Resources.Validators.Email_InvalidFormat, paramName);
        }

        public static void ValidateMatricula(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Resources.Validators.Matricula_Required,paramName);

            if (!MatriculaPattern.IsMatch(value.Trim()))
                throw new ArgumentException(Resources.Validators.Matricula_InvalidFormat,paramName);
        }

        public static void ValidatePassword(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Resources.Validators.Password_Required, paramName);

            if (!PasswordPattern.IsMatch(value))
                throw new ArgumentException(Resources.Validators.Password_InvalidFormat, paramName);
        }
    }

    public partial class Usuarios
    {
        public void UpdateProfile(
            string nombre,
            string apellidoPaterno,
            string apellidoMaterno,
            string email)
        {
            Validators.ValidateName(nombre, nameof(nombre));
            Validators.ValidateName(apellidoPaterno, nameof(apellidoPaterno));
            Validators.ValidateName(apellidoMaterno, nameof(apellidoMaterno));
            Validators.ValidateEmail(email, nameof(email));

            Nombre = nombre.Trim();
            ApellidoPaterno = apellidoPaterno.Trim();
            ApellidoMaterno = apellidoMaterno.Trim();
            Email = email.Trim();
        }
    }

    public partial class Tutores
    {
        public void UpdateContact(string email, string telefono)
        {
            Validators.ValidateEmail(email, nameof(email));
            Validators.ValidatePhone(telefono, nameof(telefono));

            Email = email.Trim();
            Telefono = telefono.Trim();
        }
    }

    public partial class Tutorias
    {
        public string SessionInfo
            => string.Format(
                   CultureInfo.CurrentCulture,
                   Resources.Validators.SessionInfoFormat,
                   Nombre, Fecha, HoraInicio, HoraFin);

        public int GetDuration()
            => (HoraFin - HoraInicio).Minutes;
    }

    public partial class Turnos
    {
        public string StatusName
            => TurnoEstados?.EstadoName
               ?? Resources.Validators.UnknownStatus;

        public DateTime CreatedAt => FechaCreacion;

        public string Info
            => string.Format(
                   CultureInfo.CurrentCulture,
                   Resources.Validators.InfoFormat,
                   Matricula, NumeroPersonal, FechaCreacion);
    }

    public partial class EvaluacionFinal
    {
        public string Summary
            => string.Format(
                   CultureInfo.CurrentCulture,
                   Resources.Validators.SummaryFormat,
                   Comentario, FechaEval);
    }
}
using System;

namespace Data
{
    public partial class Usuarios
    {
       public string FullName => $"{Nombre} {ApellidoPaterno} {ApellidoMaterno}";

       public void UpdateProfile(string nombre, string apellidoPaterno, string apellidoMaterno, string email)
        {
            Nombre = nombre;
            ApellidoPaterno = apellidoPaterno;
            ApellidoMaterno = apellidoMaterno;
            Email = email;
        }
    }

    public partial class Tutores
    {
        public string FullName => $"{Nombre} {ApellidoPaterno} {ApellidoMaterno}";

        public void UpdateContact(string email, string telefono)
        {
            Email = email;
            Telefono = telefono;
        }
    }

    public partial class Tutorias
    {
        
        public string SessionInfo => $"{Nombre} ({Fecha:dd/MM/yyyy}) {HoraInicio:hh/:mm}–{HoraFin:hh/:mm}";

        public int GetDuration() => DuracionMinutos;
    }

    public partial class Turnos
    {
        
        public string StatusName => TurnoEstados?.EstadoName ?? "Desconocido";

        public DateTime CreatedAt => FechaCreacion;

        public string Info => $"Estudiante: {Matricula}, Tutor: {NumeroPersonal} @ {FechaCreacion:dd/MM/yyyy HH:mm}";
    }

    public partial class EvaluacionFinal
    {
        public string Summary => $"{Comentario} (Evaluado: {FechaEval:dd/MM/yyyy HH:mm})";
    }
}

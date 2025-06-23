using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoriasServer
{
    public class ServerCommunicationException : Exception
    {
        public ServerCommunicationException() : base("Error de comunicación con el servidor.") { }
    }

    public class ServerTimeoutException : Exception
    {
        public ServerTimeoutException() : base("El servidor no respondió en el tiempo esperado.") { }
    }

    public class ServerStartupException : Exception
    {
        public ServerStartupException() : base("Error al iniciar el servidor.") { }
    }

    public class DuplicateUserException : Exception
    {
        public DuplicateUserException() : base("Ya existe un usuario con el mismo nombre de usuario o correo electrónico.") { }
    }

    public class RegistrationException : Exception
    {
        public RegistrationException() : base("Error al registrar el jugador.") { }
    }

    public class EmailConfigurationException : Exception
    {
        public EmailConfigurationException() : base("Error en la configuración de correo electrónico.") { }
    }

    public class EmailSendingException : Exception
    {
        public EmailSendingException() : base("Error al enviar el correo electrónico.") { }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Credenciales incorrectas.") { }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("Usuario no encontrado.") { }
    }
}

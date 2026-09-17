using System;

namespace Entidades
{
    public class Respuesta
    {
        public Boolean resultado { get; set; } = false;
        public string mensaje { get; set; } = "";
        public string mensajeError { get; set; } = "";
        public string innerException { get; set; } = "";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public enum EstadoResponse
    {
        NoProcesado = 1,
        Pendiente = 2,
        Devuelta = 3,
        Validado = 4,
        Autorizado = 5
    }

    public enum EstadoEmail
    {
        NoEnviado = 1,
        CorreoNoExiste = 2,
        CorreoInvalido = 3,
        Enviado = 4
    }

    public class FacturacionResponse
    {
        public EstadoResponse Estado { get; set; } // NoProcesado, Pendiente, Devuelta, Validado, Autorizado
        public EstadoEmail EstadoMail { get; set; } //CorreoNoExiste, Enviado, CorreoInvalido
        public string Tipo { get; set; }
        public string Mensaje { get; set; }
        public object archivos { get; set; }
    }

    public class Archivos
    {
        public byte[] Xml { get; set; }
        public byte[] Pdf { get; set; }
    }
}

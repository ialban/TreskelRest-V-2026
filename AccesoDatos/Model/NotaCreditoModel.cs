using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public class NotaCreditoModel
    {
        public long? id { get; set; }
        public string ntcrsec { get; set; }
        public string claveAcceso { get; set; }
        public string ruc { get; set; }
        public string empresaid { get; set; }
        public string serial { get; set; }
        public string codigoLocal { get; set; }
        public string nombreLocal { get; set; }
        public string direccionLocal { get; set; }
        public string idCliente { get; set; }
        public string nombreCliente { get; set; }
        public string emailCliente { get; set; }
        public string data { get; set; }
        public byte[] bData { get; set; }
        public string pcName { get; set; }
        public string ipAddress { get; set; }
        public string claveDownload { get; set; }
    }
}

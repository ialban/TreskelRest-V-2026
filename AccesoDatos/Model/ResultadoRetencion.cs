using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public class ResultadoRetencion
    {
        public long id { get; set; }
        public string empresaid { get; set; }
        public string retesec { get; set; }
        public byte[] xmlfile { get; set; }
        public byte[] pdffile { get; set; }
        public string estado { get; set; }
        public string estadomail { get; set; }
        public string error { get; set; }
        public string mensaje { get; set; }
    }
}

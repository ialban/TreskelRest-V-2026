using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public class OpcionesMail
    {
        public List<string> destino { get; set; }
        public string titulo { get; set; }
        public string mensaje { get; set; }
    }
}

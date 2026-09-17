using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public class AuthenticationRequest
    {
        public string Ruc { get; set; }
        public string EmpresaId { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}

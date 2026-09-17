using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Ruc { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public string environment { get; set; }
    }
}

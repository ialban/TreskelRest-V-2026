using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDatos.Model
{
    public class StandardJsonResponse
    {
        public bool success { get; set; }
        public string reason { get; set; }
        public object data { get; set; }
    }
}

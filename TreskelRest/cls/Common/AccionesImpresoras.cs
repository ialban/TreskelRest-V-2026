using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRest.cls.Common
{
    public class AccionesImpresoras
    {
        public string tipo { get; set; }
        public string marca { get; set; }
        public string usaEspacios { get; set; }
        public string letraGrande { get; set; }
    }

    public class ImpresoraDestino
    {
        public AccionesImpresoras Facturacion = new AccionesImpresoras();
        public AccionesImpresoras Cocina = new AccionesImpresoras();
        public AccionesImpresoras Parrilla = new AccionesImpresoras();
        public AccionesImpresoras Bar = new AccionesImpresoras();
    }
}

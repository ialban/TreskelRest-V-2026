using System;

namespace Entidades.modelos
{
    public class mDocsSri
    {
        public DateTime fecha { get; set; }
        public string tipo { get; set; }
        public string tipoImpuesto { get; set; }
        public string numeroComprobante { get; set; }
        public string numeroAutorizacion { get; set; }
        public string identificacion { get; set; }
        public string cliente { get; set; }
        public string proveedor { get; set; }
        public double subNoIva { get; set; }
        public double subConIva { get; set; }
        public double iva { get; set; }
        public double total { get; set; }
        public double baseImponible { get; set; }
        public double porcentajeRetencion { get; set; }
        public double impuestoRetenido { get; set; }
        public string numeroComprobanteAplica { get; set; }
    }
}

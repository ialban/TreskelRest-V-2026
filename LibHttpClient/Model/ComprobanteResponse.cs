using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHttpClient.Model
{
    public class ComprobanteResponse
    {
        public string ClaveAcceso { get; set; }
        // NoProcesado = N, Pendiente = P, Devuelta = D, Validado = V, Autorizado = A
        public string Secuencial { get; set; }
        public string Estado { get; set; }
        public bool EstadoMail { get; set; }
        public string Tipo { get; set; } // Factura = F, NotaCredito = C, NotaDebito = D, Retencion = R, LiquidacionCompra = L
        public string Mensaje { get; set; }
        public object archivos { get; set; }
    }
}

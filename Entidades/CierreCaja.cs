using System.Collections.Generic;

namespace Entidades
{
    public class ResultControl
    { 
        public string origen { get; set; }
        public string hora { get; set; }
        public string secuencial { get; set; }
        public string localidad { get; set; }
        public double valor { get; set; }
        public double diferencia { get; set; }
        public string tipo { get; set; }
    }


    public class FormaPago
    {
        public string tipo { get; set; }
        public double valor { get; set; }
    }

    public class VentasConsolidada
    {
        public double locales { get; set; }
        public double domicilio { get; set; }
        public double delivery { get; set; }
        public double llevar { get; set; }
        public double retirar { get; set; }
        public double total { get; set; }

        public List<FormaPago> pago { get; set; }
    }

    public class ComprasConsolidada
    {
        public double valor { get; set; }

        public List<FormaPago> pago { get; set; }
    }

    public class Vales
    {
        public double valorIngreso { get; set; }
        public double valorEgreso { get; set; }
    }

    public class CierreCaja
    {
        public string caja { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string horaCiere { get; set; }
        public string secuencial { get; set; }
        public double inicio { get; set; }
        public double cierre { get; set; }
        public double efectivo { get; set; }
        public double tarjeta { get; set; }
        public double cheque { get; set; }
        public double delivery { get; set; }
        public double transfer { get; set; }
        public double eliminado { get; set; }
        public double anulado { get; set; }
        public double salarios { get; set; }

        public VentasConsolidada ventas { get; set; }
        public ComprasConsolidada compras { get; set; }
        public Vales vales { get; set; }
    }
}

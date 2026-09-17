using System;

namespace Entidades.modelos
{
    public class mControl
    {
        public String secuencial { get; set; } = "";
        public Double ingresos { get; set; } = 0.00;
        public Double egresos { get; set; } = 0.00;
        public Double salarios { get; set; } = 0.00;
        public Double compras{ get; set; } = 0.00;
        public Double ventas { get; set; } = 0.00;
        public Double inicio { get; set; } = 0.00;
        public Double cierre { get; set; } = 0.00;

        public double efectivo { get; set; } = 0.00;
        public double tarjeta { get; set; } = 0.00;
        public double cheque { get; set; } = 0.00;
        public double transfer { get; set; } = 0.00;
        public double delivery { get; set; } = 0.00;
    }
}

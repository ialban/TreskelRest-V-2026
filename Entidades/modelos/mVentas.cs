namespace Entidades.modelos
{
    public class mVentas
    {
        public string fecha { get; set; }
        public string tipo { get; set; }
        public double subtotal { get; set; }
        public double descuento { get; set; }
        public double iva { get; set; }
        public double total { get; set; }
        public double efectivo { get; set; }
        public double tarjeta { get; set; }
        public double cheque { get; set; }
        public double transferencia { get; set; }
        public double vuelto { get; set; }
    }
}

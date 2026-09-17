using System;

namespace Entidades.modelos
{
    public class mCompra
    {
        public string compscl { get; set; }
        public string codcaja { get; set; }
        public string codempr { get; set; }
        public string numfact { get; set; }
        public string fecha { get; set; }
        public double subtot0 { get; set; }
        public double subtot { get; set; }
        public double iva { get; set; }
        public double descto { get; set; }
        public double total { get; set; }
        public Boolean compatv { get; set; } = true;
    }
}

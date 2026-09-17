using System;

namespace Entidades.modelos
{
    public class mPago
    {
        public Boolean isefectivo { get; set; } = true;
        public Double valor { get; set; } = 0;
        public Double vuelto { get; set; } = 0;
        public Boolean ischeque { get; set; } = false;
        public String numcheque { get; set; } = "";
        public bool istrans { get; set; } = false;
        public string numtrans { get; set; } = "";
        public Boolean iscorriente { get; set; } = false;
        public Boolean isdiferido { get; set; } = false;
        public String banco { get; set; } = "";
        public String lote { get; set; } = "";
        public String referencia { get; set; } = "";
        public int meses { get; set; } = 0;
        public String tarjeta { get; set; } = "";
    }
}

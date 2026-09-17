using System;

namespace Entidades
{
    public class PedidoDetalle
    {
        public string codprd { get; set; }
        public string nbrprd { get; set; }
        public Double prcprd { get; set; }
        public Double ivaprd { get; set; }
        public Boolean usaiva { get; set; }
        public bool itemgiva { get; set; }
        public int cntprd { get; set; }
        public Double ptoprd { get; set; }
        public Boolean peddatv { get; set; } = true;
        public Boolean itemdelcoc { get; set; } = false;
    }
}

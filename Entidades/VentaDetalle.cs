using System;

namespace Entidades
{
    public class VentaDetalle : ICloneable
    {

        public string itemcod { get; set; }
        public int itemcnt { get; set; }
        public double itemprc { get; set; }
        public double itemiva { get; set; }
        public double itemdto { get; set; }
        public double itempto { get; set; }
        public string nbrprd { get; set; }
        public bool usaiva { get; set; }
        public bool itemsel { get; set; }
        public bool itemEnable { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

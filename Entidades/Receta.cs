using System;

namespace Entidades
{
    public class Receta
    {
        public String prdcod { get; set; }
        public double rctacnt { get; set; }
        public String rctaund { get; set; }
        public String rctaobv { get; set; } = "";
        public Boolean rctainv { get; set; } = true;
    }
}

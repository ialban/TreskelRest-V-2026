using System;

namespace Entidades.modelos
{
    class cPedido
    {
        public String usuacod { get; set; } = null;
        public String msapedd { get; set; }
        public String ltapedd { get; set; } = "A";
        public String tippedd { get; set; } = "L";
        public String codcrri { get; set; } = "0000";
        public String crricod { get; set; } = "";
        public String crriobv { get; set; } = "";

        public String clienid { get; set; } = "";
        public String clienbr { get; set; } = "";
        public String cliedir { get; set; } = "";
        public String clietlf { get; set; } = "";
        public String clieref { get; set; } = "";
    }

    class mPedido
    {
        public String sclpedd { get; set; } = "-1";
        public String usuacod { get; set; } = null;
        public String msapedd { get; set; }
        public String ltapedd { get; set; } = "A";
        public String tippedd { get; set; } = "L";
        public String obvpedd { get; set; } = "";
        public String codcrri { get; set; } = "0000";
    }

    public class rPedido
    {
        public String fecha { get; set; }
        public String tipo { get; set; }
        public int lcnt { get; set; }
        public double lpago { get; set; }
        public int dcnt { get; set; }
        public double dpago { get; set; }
        public int ccnt { get; set; }
        public double cpago { get; set; }
        public int vcnt { get; set; }
        public double vpago { get; set; }
        public int rcnt { get; set; }
        public double rpago { get; set; }
        public int cantidad { get; set; }
        public double total { get; set; }
    }
}

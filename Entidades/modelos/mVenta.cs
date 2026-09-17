using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Entidades.modelos
{
    public class cFactura
    {
        public String cajacod { get; set; }
        public String empdnid { get; set; }

        public String nidnid { get; set; }
        public String clienbr { get; set; } = "";
        public String cliedir { get; set; } = "";
        public String clietlf { get; set; } = "";
        public String clieref { get; set; } = "";

        public String venttip { get; set; } = "L";
        public String ventccr { get; set; } = "0000";
        public string ventccd { get; set; }
        public string ventcro { get; set; }
        public String ventmsa { get; set; }
        public String ventlta { get; set; } = "A";
        public String peddscl { get; set; }
        public Double ventsbt { get; set; }
        public Double ventdsc { get; set; }
        public Double ventsbn { get; set; }
        public int ventdpc { get; set; }
        public Double ventiva { get; set; }
        public Double venttot { get; set; }
        public Double ventrcb { get; set; }
        public Double ventvto { get; set; }
        public String ventobv { get; set; }
        public Boolean ventatv { get; set; }
        public Boolean ventfac { get; set; }
        public string ventsep { get; set; }
    }

    public class cVenta
    {
        public String cajacod { get; set; }
        public String empdnid { get; set; }
        public String nidnid { get; set; }
        public String venttip { get; set; }
        public String ventccr { get; set; }
        public String ventmsa { get; set; }
        public String ventlta { get; set; }
        public String peddscl { get; set; }
        public Double ventsbt { get; set; }
        public Double ventsbn { get; set; }
        public Double ventdsc { get; set; }
        public int ventdpc { get; set; }
        public Double ventiva { get; set; }
        public Double venttot { get; set; }
        public Double ventrcb { get; set; }
        public Double ventvto { get; set; }
        public String ventobv { get; set; }
        public Boolean ventatv { get; set; }
        public Boolean ventfac { get; set; }
        public string ventsep { get; set; }
    }

    public class mVenta
    {
        public String ventscl { get; set; }
        public String cajacod { get; set; }
        public String empdnid { get; set; }
        public String nidnid { get; set; }
        public String venttip { get; set; }
        public String ventccr { get; set; }
        public string ventccd { get; set; }
        public string ventcro { get; set; }
        public String ventmsa { get; set; }
        public String ventlta { get; set; }
        public String peddscl { get; set; }
        public Double ventsbt { get; set; }
        public Double ventsbn { get; set; }
        public Double ventdsc { get; set; }
        public int ventdpc { get; set; }
        public Double ventiva { get; set; }
        public Double venttot { get; set; }
        public Double ventrcb { get; set; }
        public Double ventvto { get; set; }
        public String ventobv { get; set; }
        public Boolean ventatv { get; set; }
        public Boolean ventfac { get; set; }
        public string ventsep { get; set; }
    }

    public class PedidoBsc
    {
        public string codigo { get; set; }
        public string ventscl { get; set; }
        public string tipo { get; set; }
        public string mesa { get; set; }
        public Boolean facturado { get; set; }
        public Image<Rgba32> imagen { get; set; }
        public string etdofac { get; set; }
        public string secuencial { get; set; }
    }

    public class mDetalleVenta
    {
        public string ventscl { get; set; }
        public string itemcod { get; set; }
        public string itemnbr { get; set; }
        public int itemcnt { get; set; }
        public double itemprc { get; set; }
        public double itemiva { get; set; }
        public double itempto { get; set; }
    }
}

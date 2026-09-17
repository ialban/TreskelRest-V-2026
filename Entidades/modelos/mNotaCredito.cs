using System;
using System.Collections.Generic;

namespace Entidades.modelos
{
    public class mNotaCredito
    {
        public string ntcrscl { get; set; }
        public string ventscl { get; set; }
        public string numefac { get; set; }
        public string ntcrmtv { get; set; }
        public string ntcrmti { get; set; } // Imagen o icono del motivo
        public string ntcrfch { get; set; }
        public string nidnid { get; set; }
        public string nidnbr { get; set; }
        public string etdonce { get; set; }
        public string etdonbr { get; set; }
    }

    public class cNotaCredito
    {
        public string ventscl { get; set; }
        public string empdnid { get; set; }
        public string cajacod { get; set; }
        public string ventfac { get; set; }
        public string ventaut { get; set; }
        public string ntcrmtv { get; set; }

        public decimal ntcrsbt { get; set; }
        public decimal ntcrdsc { get; set; }
        public decimal ntcriva { get; set; }
        public decimal ntcrtot { get; set; }

        public string ntcrobv { get; set; }
    }

    public class vNotaCredito
    {
        public string ntcrscl { get; set; }
        public string usrnid { get; set; }
        public string cajacod { get; set; }
        public string nidnid { get; set; }
        public string nidnbr { get; set; }
        public string ventfac { get; set; }
        public string ventaut { get; set; }
        public string ntcraut { get; set; }
        public string numfact { get; set; }
        public string ntcrusu { get; set; }
        public string ntcrmtv { get; set; }
        public DateTime ntcrfch { get; set; }

        public double ntcrsbt { get; set; }
        public double ntcrdsc { get; set; }
        public double ntcriva { get; set; }
        public double ntcrtot { get; set; }

        public string ntcrobv { get; set; }

        public List<vNotaCreditoDetail> notaCreditoDetalle { get; set; }
    }

    //public class vNotaCreditoDetail
    //{
    //    public string ntcrscl { get; set; }
    //    public string itemcod { get; set; }
    //    public int itemcnt { get; set; }
    //    public double itemprc { get; set; }
    //    public double itempiv { get; set; }
    //    public double itempto { get; set; }
    //}
}

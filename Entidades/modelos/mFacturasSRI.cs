using System;

namespace Entidades.modelos
{
    public class mFacturasSRI
    {
        public string ventscl { get; set; }
        public string cajacod { get; set; }
        public string usuacod { get; set; }
        public string ventfch { get; set; }
        public string nidnid { get; set; }
        public string nidtip { get; set; }
        public string nidnbr { get; set; }
        public string niddir { get; set; }
        public string nideml { get; set; }
        public string nidtlf { get; set; }
        public string venttip { get; set; }
        public string ventmsa { get; set; }
        public string ventlta { get; set; }
        public string peddscl { get; set; }
        public string venthra { get; set; }
        public double ventsbt { get; set; }
        public double ventsbn { get; set; }
        public double ventdsc { get; set; }
        public double ventiva { get; set; }
        public double venttot { get; set; }
        public double ventrcb { get; set; }
        public double ventvto { get; set; }
        public string ventobv { get; set; }
        public string ventatv { get; set; }
        public string ventfac { get; set; }
        public string ventdpc { get; set; }
        public string ventdpg { get; set; }
        public string ventcrr { get; set; }
        public string ventcpt { get; set; }
        public string ventcvl { get; set; }
        public string ventstd { get; set; }
        public string ventsep { get; set; }
        public string etdofac { get; set; }
        public string etdofacdet { get; set; }
        public string etdofacImg { get; set; }
        public string etdoeml { get; set; }
        public string docutip { get; set; }
        public string docuTipImg { get; set; }
        public string docuToolTip { get; set; }
        public string docuTipRet { get; set; }
        public string docuRetImg { get; set; }
        public string errCode { get; set; }
        public string autosri { get; set; }
        public string secuencial { get; set; }
        public string numefac { get; set; }
        public DateTime fechemi { get; set; }
        public string claveDownload { get; set; }

        public bool isChecked { get; set; } = false;

        /*
         Para Retenciones
         */

        public string docuSri { get; set; }
        public string rucDev { get; set; }
    }

    public class mDocSriAutorizados
    {
        public string secPeddRet { get; set; }

        public string ventscl { get; set; }

        public string autosri { get; set; }

        public string secuencial { get; set; }

        public string tipoImg { get; set; }

        public string tipo { get; set; }

        public string fecha { get; set; }

        public string nidnid { get; set; }
        public string nombre { get; set; }
        public double total { get; set; }
        public string docref { get; set; }
    }
}

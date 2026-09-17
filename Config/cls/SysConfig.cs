using System;

namespace LibConfig.cls
{
    public class SysConfig
    {
        private Boolean usaTouch = false;
        public Boolean UsaTouch
        {
            get
            {
                return usaTouch;
            }
            set
            {
                usaTouch = value;
            }
        }

        private String printerFactura = "";
        public String PrinterFactura
        {
            get
            {
                return printerFactura;
            }
            set
            {
                printerFactura = value;
            }
        }

        private String printerProforma = "";
        public String PrinterProforma
        {
            get
            {
                return printerProforma;
            }
            set
            {
                printerProforma = value;
            }
        }

        private String printerCocina = "";
        public String PrinterCocina
        {
            get
            {
                return printerCocina;
            }
            set
            {
                printerCocina = value;
            }
        }

        private String printerParrilla = "";
        public String PrinterParrilla
        {
            get
            {
                return printerParrilla;
            }
            set
            {
                printerParrilla = value;
            }
        }

        private String printerBar = "";
        public String PrinterBar
        {
            get
            {
                return printerBar;
            }
            set
            {
                printerBar = value;
            }
        }

        private String caja = "";
        public String Caja
        {
            get
            {
                return caja;
            }
            set
            {
                caja = value;
            }
        }

    }
}

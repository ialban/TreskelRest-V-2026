using System;
using System.Windows;
using TreskelRest.cls;

namespace TreskelRest.frms
{
    /// <summary>
    /// Interaction logic for winGeneral.xaml
    /// </summary>
    public partial class winGeneral : Window
    {
        public event IniciaReporte iniciaReporte;
        public delegate void IniciaReporte();

        public event EjecutaWindowsActivate ejecutaWindowsActivate;
        public delegate void EjecutaWindowsActivate(object ventana);

        public bool esReporte = true;
        public object ventana;

        public winGeneral()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                if (esReporte)
                {
                    General.EJECUTA_GOTFOCUS = false;
                    if (iniciaReporte != null && General.PROCESANDO == false)
                    {
                        General.PROCESANDO = true;
                        iniciaReporte();
                    }
                }
                else
                {
                    General.EJECUTA_GOTFOCUS = true;
                    if (ventana != null)
                        ejecutaWindowsActivate(ventana);
                }
            }
            catch (Exception er)
            {

            }
        }
    }
}

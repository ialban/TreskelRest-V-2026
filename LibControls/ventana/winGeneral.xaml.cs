using System;
using System.Windows;

namespace LibControls.ventana
{
    /// <summary>
    /// Interaction logic for winGeneral.xaml
    /// </summary>
    public partial class winGeneral : Window
    {
        public event IniciaReporte iniciaReporte;
        public delegate void IniciaReporte();

        public winGeneral()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
           
        }
    }
}

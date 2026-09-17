using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlChkPrinters.xaml
    /// </summary>
    public partial class ctlChkPrinters : UserControl
    {
        public ctlChkPrinters()
        {
            InitializeComponent();
        }

        public void setPrintersStatus(bool Cocina, bool Parrilla, bool Facturacion, bool Proforma)
        {
            if (!Cocina) PrnCocina.Source = new BitmapImage(new Uri("pack://application:,,,/images/printerOffLine.png"));
            if (!Parrilla) PrnParrilla.Source = new BitmapImage(new Uri("pack://application:,,,/images/printerOffLine.png"));
            if (!Proforma) PrnProforma.Source = new BitmapImage(new Uri("pack://application:,,,/images/printerOffLine.png"));
            if (!Facturacion) PrnFactura.Source = new BitmapImage(new Uri("pack://application:,,,/images/printerOffLine.png"));

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

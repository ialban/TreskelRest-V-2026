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
    /// Lógica de interacción para ctlCreaPlanCuenta.xaml
    /// </summary>
    public partial class ctlCreaPlanCuenta : UserControl
    {
        public ctlCreaPlanCuenta()
        {
            InitializeComponent();

            //txtCodigo.Mask = "####.####.####.####";
            //txtCodigo.ShowPlaceHolder = true;
            txtCodigo.Separador = ".";
        }

        private void cboNivel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboNivel != null && txtCodigo != null)
            {
                switch (cboNivel.SelectedIndex)
                {
                    case 0:
                        txtCodigo.Mask = "##";
                        break;
                    case 1:
                        txtCodigo.Mask = "##.##";
                        break;
                    case 2:
                        txtCodigo.Mask = "##.##.###";
                        break;
                    case 3:
                        txtCodigo.Mask = "##.##.###.###";
                        break;
                    case 4:
                        txtCodigo.Mask = "##.##.###.###.####";
                        break;
                }
            }
            
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

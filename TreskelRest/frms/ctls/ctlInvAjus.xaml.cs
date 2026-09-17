using Entidades;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Data;
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
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlInvAjus.xaml
    /// </summary>
    public partial class ctlInvAjus : UserControl
    {
        public Window contenedor { get; set; }

        private clsTextBox clsCnt;

        public ctlInvAjus()
        {
            InitializeComponent();

            clsCnt = new clsTextBox(txtCantidad, General.USA_TECLADO_PANTALLA);

            clsCnt.setMaxLength(4);
            clsCnt.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;

            MateriaPrima productos = new MateriaPrima(General.connectionString);
            DataTable mp = productos.listarProductos();
            General.fillComboBoxFromDataTable(cboProducto, mp, "S");
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (txtCantidad.Text.Trim().Length == 0)
            {
                ObjComportamiento.showMsgError("Debe ingresar una cantidad");
                return;
            }

            if(ObjComportamiento.showMsgYesNo("¿Desea realizar el ajuste de inventario?") == MessageBoxResult.Yes)
            {
                MateriaPrima mprima = new MateriaPrima(General.connectionString);

                String producto = ((ComboBoxItem)cboProducto.SelectedItem).Tag.ToString();
                String unidad = ((ComboBoxItem)cboUnidad.SelectedItem).Tag.ToString();
                String tipo = ((ComboBoxItem)cboTipo.SelectedItem).Tag.ToString();

                Respuesta respuesta = mprima.ajustarInventario(producto,unidad, General.StringToDouble( txtCantidad.Text.ToString()),tipo,"");

                ObjComportamiento.showMuestraMensaje(respuesta);
            }

            btnCancelar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}

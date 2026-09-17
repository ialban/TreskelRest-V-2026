using Entidades;
using Entidades.modelos;
using LibKeys;
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
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{    
    /// <summary>
    /// Interaction logic for ctlDelivery.xaml
    /// </summary>
    public partial class ctlCarrierData : UserControl
    {
        public Window contenedor { get; set; }
        //public ctlPedidos parent { get; set; } = null;
        public object parent { get; set; } = null;

        public event GeneraPedidoCarrier pedidoCarrier;
        public delegate void GeneraPedidoCarrier(String cliNid, String cliObv);

        private clsTextBox clsNid;
        private clsTextBox clsDir;
        
        public ctlCarrierData()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsDir = new clsTextBox(txtDir, General.USA_TECLADO_PANTALLA);

            clsNid.permiteEspacios = false;
            clsDir.permiteEspacios = true;

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsNid.MaxLength = 10;

            clsDir.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsDir.MaxLength = 10;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtNid.Text.Trim().Length == 0)
            {
                msg += "El campo Código no puede estar en blanco" + ((char) 13);
            }

            if (msg.Equals(""))
            {
                pedidoCarrier(txtNid.Text, txtDir.Text);
                contenedor.Close();
            }
            else
            {                
                ObjComportamiento.showMsgError(msg);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }
    }
}

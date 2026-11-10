using Entidades;
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
    /// Interaction logic for ctlCreaProveedor.xaml
    /// </summary>
    public partial class ctlCreaProveedor : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlProveedor parent { get; set; } = null;

        private clsTextBox clsRuc;
        private clsTextBox clsProv;
        private clsTextBox clsDir;
        private clsTextBox clsTlf;
        private clsTextBox clsEmail;
        private clsTextBox clsCto;

        public ctlCreaProveedor()
        {
            InitializeComponent();

            clsRuc = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsProv = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);
            clsDir = new clsTextBox(txtDireccion, General.USA_TECLADO_PANTALLA);
            clsTlf = new clsTextBox(txtTfl, General.USA_TECLADO_PANTALLA);
            clsEmail = new clsTextBox(txtEmail, General.USA_TECLADO_PANTALLA);
            clsCto = new clsTextBox(txtContacto, General.USA_TECLADO_PANTALLA);

            clsRuc.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsProv.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsDir.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsTlf.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsEmail.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.CORREO;
            clsCto.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;

            clsProv.permiteEspacios = true;
            clsProv.permitePunto = true;
            clsDir.permiteEspacios = true;
            clsDir.permitePunto = true;
            clsDir.otrosCaracteresPermitidos = "()/:,-#;";
            clsCto.permiteEspacios = true;

            clsRuc.setMaxLength(13);
            clsProv.setMaxLength(50);
            clsDir.setMaxLength(100);
            clsTlf.setMaxLength(20);
            clsEmail.setMaxLength(50);
            clsCto.setMaxLength(50);

            txtNid.IsEnabled = true;
        }

        public void setNid(string nid)
        {
            Proveedor proveedor = new Proveedor(General.connectionString);

            proveedor.provNid = nid;
            if (proveedor.getById() == true)
            {
                txtNid.Text = proveedor.provNid;               
                txtNombre.Text = proveedor.provNbr;
                txtDireccion.Text = proveedor.provDir;
                txtTfl.Text = proveedor.provTlf;
                txtEmail.Text = proveedor.provEml;
                txtContacto.Text = proveedor.provCto;
                chkEstado.IsChecked = proveedor.provAtv;

                txtNid.IsEnabled = false;
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtNid.Text.Trim().Length == 0)
            {
                msg += "- El campo Identificador no puede estar en blanco" + ((char)13);
            }

            if (txtNombre.Text.Trim().Length == 0)
            {
                msg += "- El campo Proveedor no puede estar en blanco" + ((char)13);
            }

            if (txtDireccion.Text.Trim().Length == 0)
            {
                msg += "- El campo Dirección no puede estar en blanco" + ((char)13);
            }

            if (txtTfl.Text.Trim().Length == 0)
            {
                msg += "- El campo Teléfono no puede estar en blanco" + ((char)13);
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del Proveedor?");
            if (res == MessageBoxResult.Yes)
            {
                Proveedor proveedor = new Proveedor(General.connectionString);

                proveedor.provNid = txtNid.Text;
                proveedor.provNbr = txtNombre.Text;
                proveedor.provDir = txtDireccion.Text;
                proveedor.provTlf = txtTfl.Text;
                proveedor.provEml = txtEmail.Text;
                proveedor.provCto = txtContacto.Text;
                proveedor.provAtv = chkEstado.IsChecked == true ? true : false;

                Respuesta respuesta = proveedor.grabar(this.accion);
                if (respuesta.resultado)
                {
                    ObjComportamiento.showMsgInfo(respuesta.mensaje);
                    if (this.accion.Equals("M"))
                    {
                        this.parent.Actualiza_Trigger();
                    }
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError(respuesta.mensaje);
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnMercado_Click(object sender, RoutedEventArgs e)
        {
            txtNid.Text = "9999999999999";
            txtNombre.Text = "MERCADO";
            txtDireccion.Text = "NINGUNO";
            txtTfl.Text = "9999999999";
            chkEstado.IsChecked = true;
        }
    }
}

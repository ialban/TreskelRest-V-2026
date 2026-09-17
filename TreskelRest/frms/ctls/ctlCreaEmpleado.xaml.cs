using Entidades;
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
using LibControls;
using LibKeys;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCreaEmpleado.xaml
    /// </summary>
    public partial class ctlCreaEmpleado : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlEmpleados parent { get; set; } = null;

        private clsTextBox clsNid;
        private clsTextBox clsNom;
        private clsTextBox clsApe;
        private clsTextBox clsDir;
        private clsTextBox clsTco;
        private clsTextBox clsTce;
        private clsTextBox clsSld;

        public ctlCreaEmpleado()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsNom = new clsTextBox(txtNombre, General.USA_TECLADO_PANTALLA);
            clsApe = new clsTextBox(txtApellido, General.USA_TECLADO_PANTALLA);
            clsDir = new clsTextBox(txtDireccion, General.USA_TECLADO_PANTALLA);
            clsTco = new clsTextBox(txtTfl, General.USA_TECLADO_PANTALLA);
            clsTce = new clsTextBox(txtCel, General.USA_TECLADO_PANTALLA);
            clsSld = new clsTextBox(txtSalario, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsNom.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsApe.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsDir.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsTco.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsTce.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsSld.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;

            clsNom.permiteEspacios = true;
            clsApe.permiteEspacios = true;
            clsDir.permiteEspacios = true;

            clsNid.setMaxLength(13);
            clsNom.setMaxLength(50);
            clsApe.setMaxLength(50);
            clsDir.setMaxLength(100);
            clsTco.setMaxLength(20);
            clsTce.setMaxLength(20);
            clsSld.setMaxLength(10);

            txtNid.IsEnabled = true;
        }

        public void setNid(string nid)
        {
            Empleados empleados = new Empleados(General.connectionString);

            empleados.empdNid = nid;
            if (empleados.getById()==true)
            {
                txtNid.Text = empleados.empdNid;

                General.selectedByTagCombo(cboTipo, empleados.empdTip);

                txtNombre.Text = empleados.empdNom;
                txtApellido.Text = empleados.empdApe;
                txtDireccion.Text = empleados.empdDir;
                txtTfl.Text = empleados.empdTlf;
                txtCel.Text = empleados.empdCel;
                dtpFecha.Text = empleados.empdFna;
                chkEstado.IsChecked = empleados.empdAtv;
                txtHijos.setValue(empleados.empdChj.ToString());
                txtSalario.Text = string.Format("{0:0.00}", empleados.empdSal);

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

            if (((ComboBoxItem)cboTipo.SelectedItem).Tag.ToString() == "C" && txtNid.Text.Length > 10)
            {
                msg += "- El campo Identificador es de tipo CÉDULA y no puede tener más de 10 caracteres" + ((char)13);
            }

            if (((ComboBoxItem)cboTipo.SelectedItem).Tag.ToString() == "R" && txtNid.Text.Length < 13)
            {
                msg += "- El campo Identificador es de tipo RUC y no puede tener más de 10 caracteres" + ((char)13);
            }

            if (txtNombre.Text.Trim().Length == 0)
            {
                msg += "- El campo Nombre no puede estar en blanco" + ((char)13);
            }

            if (txtApellido.Text.Trim().Length == 0)
            {
                msg += "- El campo Apellido no puede estar en blanco" + ((char)13);
            }

            if (txtDireccion.Text.Trim().Length == 0)
            {
                msg += "- El campo Dirección no puede estar en blanco" + ((char)13);
            }

            if (dtpFecha.Text.Equals(""))
            {
                msg += "- El campo Fecha de Nacimiento no puede estar en blanco" + ((char)13);
            }

            if (txtSalario.Text.Trim().Equals(""))
            {
                txtSalario.Text = "0.00";
            }

            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del Empleado?");
            if (res == MessageBoxResult.Yes)
            {
                Empleados empleados = new Empleados(General.connectionString);

                empleados.empdNid = txtNid.Text;
                empleados.empdTip = ((ComboBoxItem)(cboTipo.SelectedItem)).Tag.ToString();
                empleados.empdNom = txtNombre.Text;
                empleados.empdApe = txtApellido.Text;
                empleados.empdDir = txtDireccion.Text;
                empleados.empdTlf = txtTfl.Text;
                empleados.empdCel = txtCel.Text;
                empleados.empdChj = int.Parse(txtHijos.getValue());
                empleados.empdFna = dtpFecha.Text;
                empleados.empdSal = double.Parse(txtSalario.Text);
                empleados.empdAtv = chkEstado.IsChecked==true?true:false;

                var result = empleados.grabar(this.accion);
                if (result.resultado)
                {
                    ObjComportamiento.showMsgInfo(result.mensaje);
                    this.parent.Actualiza_Trigger();
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError("Los datos del empleado no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!" 
                        + System.Environment.NewLine + "- " + result.mensaje);
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }        
    }
}

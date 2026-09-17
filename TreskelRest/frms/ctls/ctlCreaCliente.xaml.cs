using Entidades;
using LibKeys;
using System;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCreaCliente.xaml
    /// </summary>
    public partial class ctlCreaCliente : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlClientes parent { get; set; } = null;
        public Boolean callFromFact { get; set; } = false;

        public event CreacionCliente creacionCliente;
        public delegate void CreacionCliente(string cliente, string cedula, string direccion, string telefono, string tipo = "R", string email = "", bool clieCdto = false, string emprruc = "", string emprnbr = "", bool cdtopem = false);

        private clsTextBox clsNid = null;
        private clsTextBox clsCliente = null;
        private clsTextBox clsDireccion = null;
        private clsTextBox clsTlf = null;
        private clsTextBox clsEmail = null;

        public ctlCreaCliente()
        {
            InitializeComponent();
            txtNid.IsEnabled = true;

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsCliente = new clsTextBox(txtCliente, General.USA_TECLADO_PANTALLA);
            clsDireccion = new clsTextBox(txtDireccion, General.USA_TECLADO_PANTALLA);
            clsTlf = new clsTextBox(txtTfl, General.USA_TECLADO_PANTALLA);
            clsEmail = new clsTextBox(txtEmail, General.USA_TECLADO_PANTALLA);

            clsNid.setMaxLength(20);
            clsCliente.setMaxLength(80);
            clsDireccion.setMaxLength(100);
            clsTlf.setMaxLength(20);
            clsEmail.setMaxLength(100);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsCliente.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsDireccion.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsTlf.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsEmail.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.CORREO;

            clsCliente.permiteEspacios = true;
            clsCliente.permitePunto = true;
            clsCliente.otrosCaracteresPermitidos = "-";
            clsDireccion.permiteEspacios = true;
            clsDireccion.otrosCaracteresPermitidos = ".-()#/";

            clsNid.setMaxLength(10);

            chkEstado.isChecked = true;

        }

        public void setNid(string nid)
        {
            Clientes clientes = new Clientes(General.connectionString);

            clientes.nidNid = nid;
            if (clientes.getById() == true)
            {
                txtNid.Text = clientes.nidNid;

                General.selectedByTagCombo(cboTipo, clientes.nidTip);

                txtCliente.Text = clientes.nidNbr;
                txtDireccion.Text = clientes.nidDir;
                txtTfl.Text = clientes.nidTlf;
                txtEmail.Text = clientes.nidEml;
                chkEstado.isChecked = clientes.nidStdo == "A" ? true : false;
                chkCredito.isChecked = clientes.clieCdto == true ? true : false;
                chkCobraEmpresa.isChecked = clientes.emprPaga == true ? true : false;
                txtRucEmp.Text = clientes.emprNbr;
                txtRucEmp.Tag = clientes.rucEmpr;

                txtNid.IsEnabled = false;
            }
            else
            {
                txtNid.Text = clientes.nidNid;
                switch(clientes.nidNid.Length)
                {
                    case 13:
                        General.selectedByTagCombo(cboTipo, "R");
                        break;
                    case 10:
                        General.selectedByTagCombo(cboTipo, "C");
                        break;
                    default:
                        General.selectedByTagCombo(cboTipo, "P");
                        break;
                }
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtNid.Text.Trim().Length == 0)
            {
                msg += "- El campo Identificador no puede estar en blanco" + ((char)13);
            }

            if (txtNid.Text.Length > 0)
            {
                int identificador = txtNid.Text.Trim().Length;

                if (((ComboBoxItem)(cboTipo.SelectedItem)).Tag.ToString() == "C" && identificador != 10)
                    msg += "El número de identificación del cliente debe ser de 10 dígitos o contiene ESPACIOS";
                if (((ComboBoxItem)(cboTipo.SelectedItem)).Tag.ToString() == "R" && identificador != 13)
                    msg += "El número de identificación del cliente debe ser de 13 dígitos o contiene ESPACIOS";
            }

            if (txtCliente.Text.Trim().Length == 0)
            {
                msg += "- El campo Cliente no puede estar en blanco" + ((char)13);
            }
            
            if (!msg.Equals(""))
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            MessageBoxResult res = ObjComportamiento.showMsgYesNo("¿Desar grabar los datos del Cliente?");
            if (res == MessageBoxResult.Yes)
            {
                Clientes clientes = new Clientes(General.connectionString);

                clientes.nidNid = txtNid.Text;
                clientes.nidTip = ((ComboBoxItem)(cboTipo.SelectedItem)).Tag.ToString();
                clientes.nidNbr = txtCliente.Text;
                clientes.nidDir = txtDireccion.Text;
                clientes.nidTlf = txtTfl.Text;
                clientes.nidEml = txtEmail.Text.ToLower();
                clientes.nidStdo = chkEstado.isChecked == true ? "A" : "I";
                clientes.clieCdto = chkCredito.isChecked == true ? true : false;
                clientes.rucEmpr = txtRucEmp.Tag != null ? txtRucEmp.Tag.ToString() : null;
                clientes.emprPaga = chkCobraEmpresa.isChecked == true ? true : false;

                if (clientes.grabar(this.accion))
                {
                    ObjComportamiento.showMsgInfo("Datos del Cliente grabados correctamente");
                    if (this.accion.Equals("M"))
                    {
                        if (this.parent != null)
                        {
                            this.parent.Actualiza_Trigger();
                        }
                    }

                    if (callFromFact)
                    {
                        creacionCliente(clientes.nidNbr, clientes.nidNid, clientes.nidDir, clientes.nidTlf, clientes.nidTip, clientes.nidEml, clientes.clieCdto, clientes.rucEmpr, clientes.emprNbr, clientes.emprPaga);
                    }
                    contenedor.Close();
                }
                else
                {
                    if (callFromFact)
                    {
                        creacionCliente("", "", "", "", "", "", false, "", "", false);
                    }
                    ObjComportamiento.showMsgError("Los datos del Cliente no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!");
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void cboTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtNid == null) return;

            try
            {
                string tipo = ((ComboBoxItem)cboTipo.SelectedItem).Tag.ToString();

                switch (tipo)
                {
                    case "C":
                        if (txtNid.Text.Length > 10) txtNid.Text = txtNid.Text.Substring(0, 10);
                        if (clsNid != null) clsNid.setMaxLength(10);
                        break;
                    case "R":
                        if (txtNid.Text.Length > 13) txtNid.Text = txtNid.Text.Substring(0, 13);
                        if (clsNid != null) clsNid.setMaxLength(13);
                        break;
                    case "P":
                        if (clsNid != null) clsNid.setMaxLength(20);
                        break;
                }
            }
            catch (Exception er)
            {

            }
            
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Búsqueda de Cliente (Empresa)";
            win.Width = 680;
            win.Height = 480;

            ctlBscEmpresas crea = new ctlBscEmpresas(txtRucEmp.Text);
            crea.aceptaEmpresa += Crea_aceptaEmpresa;
            crea.contenedor = win;
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Crea_aceptaEmpresa(string codigo, string nombre)
        {
            txtRucEmp.Text = nombre;
            txtRucEmp.Tag = codigo;
        }

        private void chkCredito_ctlChecked(object sender, RoutedEventArgs e)
        {
            if (chkCredito != null && chkCobraEmpresa != null)
            {
                if (chkCredito.isChecked == true)
                {
                    chkCobraEmpresa.isEnabled = true;
                }
            }
        }

        private void chkCredito_ctlUnChecked(object sender, RoutedEventArgs e)
        {
            if (chkCredito != null && chkCobraEmpresa != null)
            {
                if (chkCredito.isChecked == false)
                {
                    chkCobraEmpresa.isEnabled = false;
                    chkCobraEmpresa.isChecked = false;
                }
            }
        }
    }
}

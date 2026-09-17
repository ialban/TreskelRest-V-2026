using Entidades;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlDelivery.xaml
    /// </summary>
    public partial class ctlDelivery : UserControl
    {
        private Window contenedor { get; set; }
        public ctlPedidos parent { get; set; } = null;
        public ctlFacturacion parentFact { get; set; } = null;

        public event GeneraPedidoDomicilio pedidoDomicilio;
        public delegate void GeneraPedidoDomicilio(String cliNid, String cliNbr, String cliDir, String cliTlf, String cliRef);

        private clsTextBox clsNid;
        private clsTextBox clsCli;
        private clsTextBox clsDir;
        private clsTextBox clsTlf;
        private clsTextBox clsRef;

        public Boolean paraCarrier { get; set; } = false;

        public Window Contenedor
        {
            set
            {
                contenedor = value;
                if (contenedor != null)
                {
                    contenedor.Closed += Contenedor_Closed;
                }
            }
        }

        public ctlDelivery()
        {
            InitializeComponent();

            clsNid = new clsTextBox(txtNid, General.USA_TECLADO_PANTALLA);
            clsCli = new clsTextBox(txtNbr, General.USA_TECLADO_PANTALLA);
            clsDir = new clsTextBox(txtDir, General.USA_TECLADO_PANTALLA);
            clsTlf = new clsTextBox(txtTlf, General.USA_TECLADO_PANTALLA);
            clsRef = new clsTextBox(txtRef, General.USA_TECLADO_PANTALLA);

            clsCli.permiteEspacios = true;
            clsDir.permiteEspacios = true;
            clsTlf.permiteEspacios = true;
            clsRef.permiteEspacios = true;

            if (paraCarrier)
            {
                clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
                clsNid.MaxLength = 6;
            }
            else
            {
                clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            }
            
            clsCli.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsDir.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsTlf.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsRef.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;

            if (contenedor != null)
            {
                contenedor.Closed += Contenedor_Closed;
            }
            
        }

        private void Contenedor_Closed(object sender, EventArgs e)
        {
            if (parent != null)
            {
                parent.Focus();
                parent.barcode.Focus();
            }
            
            if (parentFact != null)
            {
                parentFact.Focus();
                parentFact.barcode.Focus();
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";
            if (txtNid.Text.Trim().Length == 0)
            {
                msg += "El campo Identificación no puede estar en blanco" + ((char) 13);
            }

            if(txtNbr.Text.Trim().Length == 0 && paraCarrier==false)
            {
                msg += "El campo Cliente no puede estar en blanco" + ((char)13);
            }

            if (txtDir.Text.Trim().Length == 0 && paraCarrier == false)
            {
                msg += "El campo Dirección no puede estar en blanco" + ((char)13);
            }

            if (msg.Equals(""))
            {
                pedidoDomicilio(txtNid.Text, txtNbr.Text, txtDir.Text, txtTlf.Text, txtRef.Text);
                HabilitaBotonesBorrar(true);
                contenedor.Close();

                if (parent != null)
                    parent.Focus();

                if (parentFact != null)
                    parentFact.Focus();
            }
            else
            {                
                ObjComportamiento.showMsgError(msg);
            }
        }

        private void HabilitaBotonesBorrar(Boolean habilita)
        {
            btnBorrarDir.IsEnabled = habilita;
            btnBorrarTlf.IsEnabled = habilita;
            btnBorrarRef.IsEnabled = habilita;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
            
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            ClienteDomicilio cliente= new ClienteDomicilio(General.connectionString);

            if (txtNid.Text.Length == 0)
            {
                ObjComportamiento.showMsgInfo("Debe ingresar el número de identificación del cliente para realizar la búsqueda");
                return;
            }

            List<ClienteDomicilio> clientes = cliente.getClienteByNid(txtNid.Text);

            if (clientes.Count > 1)
            {
                winGeneral win = new winGeneral();
                win.Title = "Datos de pedido a domicilio";
                win.Width = 680; //620
                win.Height = 420;

                ctlClientesDomicilio ctl = new ctlClientesDomicilio(clientes);
                ctl.contenedor = win;
                ctl.parent = this;
                ctl.aceptaCliente += new ctlClientesDomicilio.AceptaCliente(AceptaCliente);

                win.Contenido.Children.Add(ctl);
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
            else if(clientes.Count == 1)
            {
                txtNbr.Text = clientes[0].clieNbr;
                txtDir.Text = clientes[0].clieDir;
                txtTlf.Text = clientes[0].clieTlf;
                txtRef.Text = clientes[0].clieRef;

                HabilitaBotonesBorrar(true);
            }
            else
            {
                ObjComportamiento.showMsgInfo("El cliente no tiene pedidos previos");
            }
        }

        private void btnTlfCli_Click(object sender, RoutedEventArgs e)
        {
            ClienteDomicilio cliente = new ClienteDomicilio(General.connectionString);

            if(txtTlf.Text.Length == 0)
            {
                ObjComportamiento.showMsgInfo("Debe ingresar un número de teléfono para realizar la búsqueda");
                return;
            }

            List<ClienteDomicilio> clientes = cliente.getClienteByTlf(txtTlf.Text);

            if (clientes.Count > 1)
            {
                winGeneral win = new winGeneral();
                win.Title = "Datos de pedido a domicilio";
                win.Width = 680;
                win.Height = 420;

                ctlClientesDomicilio ctl = new ctlClientesDomicilio(clientes);
                ctl.contenedor = win;
                ctl.parent = this;
                ctl.aceptaCliente += new ctlClientesDomicilio.AceptaCliente(AceptaCliente);

                win.Contenido.Children.Add(ctl);
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
            else if (clientes.Count == 1)
            {
                txtNid.Text = clientes[0].clieNid;
                txtNbr.Text = clientes[0].clieNbr;
                txtDir.Text = clientes[0].clieDir;
                txtTlf.Text = clientes[0].clieTlf;
                txtRef.Text = clientes[0].clieRef;

                HabilitaBotonesBorrar(true);
            }
            else
            {
                ObjComportamiento.showMsgInfo("El cliente no tiene pedidos previos");
            }
        }

        private void btnBorrarDir_Click(object sender, RoutedEventArgs e)
        {
            txtDir.Text = "";
        }

        private void btnBorrarTlf_Click(object sender, RoutedEventArgs e)
        {
            txtTlf.Text = "";
        }

        private void btnBorrarRef_Click(object sender, RoutedEventArgs e)
        {
            txtRef.Text = "";
        }

        private void AceptaCliente(ClienteDomicilio cliente)
        {
            txtNbr.Text = cliente.clieNbr;
            txtDir.Text = cliente.clieDir;
            txtTlf.Text = cliente.clieTlf;
            txtRef.Text = cliente.clieRef;
            txtNid.Text = cliente.clieNid;

            HabilitaBotonesBorrar(true);
        }
    }
}

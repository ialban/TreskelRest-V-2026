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
    /// Lógica de interacción para ctlIngPagosCrdto.xaml
    /// </summary>
    public partial class ctlIngPagosCrdto : UserControl
    {
        private clsTextBox clsNid;

        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }

        Clientes cliente = new Clientes(General.connectionString);

        public ctlIngPagosCrdto()
        {
            InitializeComponent();
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            clsNid = new clsTextBox(txtCedula, General.USA_TECLADO_PANTALLA);

            clsNid.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsNid.permiteEspacios = true;
            clsNid.setMaxLength(20);
        }

        private void sbLimpiar()
        {
            txtCedula.Text = "";
            txtCliente.Text = "";
            txtValor.Text = "0.00";
            txtRecibido.Text = "0.00";
            txtSaldo.Text = "0.00";

            dgvDetalle.ItemsSource = null;
            dgvDetalle.Items.Refresh();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mDeuda deuda = (mDeuda)dgvDetalle.CurrentItem;

            deuda.itemsel = true;
            calculaTotal();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mDeuda deuda = (mDeuda)dgvDetalle.CurrentItem;

            deuda.itemsel = false;
            calculaTotal();
        }

        private void calculaTotal()
        {
            double total = 0;
            foreach(mDeuda deuda in dgvDetalle.Items)
            {
                if (deuda.itemsel)
                {
                    total = total + deuda.cdtoval;
                }
            }

            txtValor.Text = string.Format("{0:0.00}", total);
        }

        private void btnToPay_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Pago de Créditos";
            win.Width = 1010;
            win.Height = 550;

            ctlPagos crea = new ctlPagos(txtValor.Text.ToString());
            crea.contenedor = win;
            crea.pagoRealizado += new ctlPagos.PagoRealizado(Pago_Realizado);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void Pago_Realizado(List<mPago> pagos)
        {
            Double valorPagado = 0;
            double valor = General.StringToDouble(txtValor.Text.ToString());

            txtRecibido.Text = "0.00";
            txtSaldo.Text = "0.00";

            foreach (mPago pago in pagos)
            {
                valorPagado += pago.valor;
            }

            if (valorPagado > valor)
            {
                ObjComportamiento.showMsgError("El valor a pagar no puede ser mayor al valor adeudado.");
                return;
            }

            txtRecibido.Text = String.Format("{0:0.00}", valorPagado);

            txtSaldo.Text = String.Format("{0:0.00}", valor - valorPagado);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            double valor = General.StringToDouble(txtRecibido.Text.ToString());
            if (valor == 0)
            {
                ObjComportamiento.showMsgError("El valor del pago recibido no puede ser cero, VERIFIQUE");
                return;
            }

            double valorPagar = General.StringToDouble(txtRecibido.Text);
            bool respuesta = false;
            double saldo = 0.0;

            List<bool> respuestas = new List<bool>();

            foreach (mDeuda row in dgvDetalle.Items)
            {
                if (row.itemsel)
                {
                    saldo = row.cdtoval - valorPagar;

                    if (saldo == 0 || saldo > 0)
                    {
                        respuesta = cliente.PagaDeuda(row.cdtoscl, valorPagar);
                    }

                    if (saldo < 0)
                    {
                        respuesta = cliente.PagaDeuda(row.cdtoscl, saldo);
                        valorPagar = Math.Abs(saldo);
                    }
                    respuestas.Add(respuesta);
                }
            }

            if (respuestas.Where(x => x == false).Count() > 0)
            {
                ObjComportamiento.showMsgError("No se pudo registrar un pago, VERIFIQUE");
                return;
            }
            else
            {
                ObjComportamiento.showMsgInfo("Pago del crédito ha sido realizado con éxito.");
                return;
            }
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnBscCli_Click_1(object sender, RoutedEventArgs e)
        {
            dgvDetalle.ItemsSource = null;
            dgvDetalle.Items.Refresh();

            if (string.IsNullOrEmpty(txtCedula.Text))
            {
                ObjComportamiento.showMsgError("Debe ingresar un criterio de búsqueda del cliente");
                return;
            }

            //cliente.nidNid = txtCedula.Text.ToString();
            var lClientes = cliente.getByIdOrName(txtCedula.Text.ToString());
            if (lClientes.Count == 1)
            {
                txtCedula.Text = lClientes.First().nidNid;
                txtCliente.Text = lClientes.First().nidNbr;

                loadDeuda(lClientes.First().nidNid);

            }
            else if (lClientes.Count > 1)
            {
                // Muestra listado de clientes que cumplen con el filtro ingresado

                winGeneral win = new winGeneral();
                win.Title = "Selección de cliente";
                win.Width = 810;
                win.Height = 480;

                ctlBscClientes crea = new ctlBscClientes(lClientes);
                crea.contenedor = win;
                crea.aceptaCliente += new ctlBscClientes.AceptaCliente(cliente_Seleccionado);

                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
            else
            {
                ObjComportamiento.showMsgError("No existe el cliente con crédito buscado");
                return;
            }
        }

        private void cliente_Seleccionado(string codigo, string nombre, string direccion, string telefono, string email, string tipo, bool credito, string emprruc, string emprnbr, bool emprpaga)
        {
            txtCliente.Text = nombre;
            txtCedula.Text = codigo;

            // Llenar el grid con la deuda del cliente
            loadDeuda(codigo);
        }

        private void loadDeuda(string clieNid)
        {
            var deuda = cliente.GetDeuda(clieNid);

            dgvDetalle.ItemsSource = null;
            dgvDetalle.ItemsSource = deuda;
            dgvDetalle.Items.Refresh();

            double total = 0;
            foreach (mDeuda row in dgvDetalle.Items)
            {
                total = total + row.cdtoval;
            }

            txtSaldo.Text = string.Format("{0:0.00}", total);
        }
    }
}

using Entidades;
using LibControls.ventana;
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

namespace LibControls.FactCliente
{
    /// <summary>
    /// Lógica de interacción para FactCliente.xaml
    /// </summary>
    public partial class FactCliente : UserControl
    {
        private Image _delete = null;
        private Image _search = null;
        private Image _clifin = null;

        public double total = 0;
        public string tipoVenta = "E";
        private string connectionString = "";

        public delegate void RetornaData(string cedula, string nombre, string direccion, string telefono);
        public event RetornaData _ctlTrigerError;

        private UIElement _buscaCliente;
        private UIElement _creaCliente;

        public Image DeleteImage {
            get{
                return _delete;
            }
            set
            {
                _delete = value;
            }
        }

        public Image SearchImage
        {
            get
            {
                return _search;
            }
            set
            {
                _search = value;
            }
        }

        public Image ClienteFinalImage
        {
            get
            {
                return _clifin;
            }
            set
            {
                _clifin = value;
            }
        }

        public FactCliente(string connectionString, UIElement buscaCliente, UIElement creaCliente)
        {
            InitializeComponent();

            this.connectionString = connectionString;
            _buscaCliente = buscaCliente;
            _creaCliente = creaCliente;
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            Clientes cliente = new Clientes(connectionString);

            if (string.IsNullOrEmpty(txtCedula.Text))
            {
                MessageBox.Show("Debe ingresar un criterio de búsqueda del cliente", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var lClientes = cliente.getByIdOrName(txtCedula.Text.ToString());

            if (lClientes.Count == 1)
            {
                txtCedula.Text = lClientes.First().nidNid;
                txtCliente.Text = lClientes.First().nidNbr;
                _ctlTrigerError(txtCedula.Text, txtCliente.Text, lClientes.First().nidDir, lClientes.First().nidTlf);
            }
            else if (lClientes.Count > 1)
            {
                // Muestra listado de clientes que cumplen con el filtro ingresado
                winGeneral win = new winGeneral();
                win.Title = "Selección de cliente";
                win.Width = 810;
                win.Height = 480;

                //ctlBscClientes crea = new ctlBscClientes(lClientes);
                //crea.contenedor = win;
                //crea.aceptaCliente += new ctlBscClientes.AceptaCliente(cliente_Seleccionado);

                win.Contenido.Children.Add(_buscaCliente);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
            else
            {
                winGeneral win = new winGeneral();
                win.Title = "Creación de cliente";
                win.Width = 710;
                win.Height = 430;

                //ctlCreaCliente crea = new ctlCreaCliente();
                //crea.contenedor = win;
                //crea.callFromFact = true;
                //crea.accion = "N";
                //crea.creacionCliente += new ctlCreaCliente.CreacionCliente(Cliente_Creado);
                //crea.setNid(cliente.nidNid);

                win.Contenido.Children.Add(_creaCliente);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
        }

        private void btnCliFnl_Click(object sender, RoutedEventArgs e)
        {
            if (total > 50 && tipoVenta != "E")
            {
                MessageBox.Show("La compra supera los $50, por cuanto no puede ser definida como CLIENTE FINAL", 
                    "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            txtCedula.Text = "9999999999999";
            txtCliente.Text = "CLIENTE FINAL";

            _ctlTrigerError(txtCedula.Text, txtCliente.Text, "", "");
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            txtCedula.Text = "";
            txtCliente.Text = "";

            _ctlTrigerError(txtCedula.Text, txtCliente.Text, "", "");
        }
    }
}

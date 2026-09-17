using Entidades.modelos;
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
    /// Lógica de interacción para ctlBscClientes.xaml
    /// </summary>
    public partial class ctlBscClientes : UserControl
    {
        public Window contenedor { get; set; }

        public event AceptaCliente aceptaCliente;
        public delegate void AceptaCliente(string codigo, string nombre, string direccion, string telefono, string email, string tipo, bool credito, string emprruc = "", string emprnbr = "", bool cdtopem = false);

        public ctlBscClientes(List<mClientes> clientes)
        {
            InitializeComponent();

            dgvDetalle.ItemsSource = clientes;

            dgvDetalle.Items.Refresh();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            mClientes cliente = (mClientes)dgvDetalle.SelectedValue;

            if (cliente != null)
            {
                aceptaCliente(cliente.nidNid, cliente.nidNbr, cliente.nidDir, cliente.nidTlf, cliente.nidEml, cliente.nidTip, cliente.cliecdto, cliente.nidEmpr, cliente.emprNbr, cliente.emprPaga);
            }
            else
            {
                ObjComportamiento.showMsgError("Debe seleccionar una empresa");
                return;
            }

            contenedor.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            aceptaCliente("", "", "", "", "", "", false, "", "", false);
            contenedor.Close();
        }
    }
}

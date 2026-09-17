using Entidades;
using Entidades.modelos;
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
    /// Lógica de interacción para ctlBscEmpresas.xaml
    /// </summary>
    public partial class ctlBscEmpresas : UserControl
    {
        private string filtro { get; set; }
        public Window contenedor { get; set; }

        public event AceptaEmpresa aceptaEmpresa;
        public delegate void AceptaEmpresa(string codigo, string nombre);

        public ctlBscEmpresas(string filtro)
        {
            InitializeComponent();

            this.filtro = filtro;
            dgvDetalle.RowHeight = 40;

            generar();
        }

        public void generar()
        {
            Clientes clientes = new Clientes(General.connectionString);

            var data = clientes.buscar(filtro, "R");

            List<mClientes> mclientes = new List<mClientes>();

            foreach(DataRow row in data.Rows)
            {
                mclientes.Add(new mClientes
                {
                    nidNid = row["codigo"].ToString(),
                    nidNbr = row["cliente"].ToString()
                });
            }

            dgvDetalle.ItemsSource = null;
            dgvDetalle.ItemsSource = mclientes;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            aceptaEmpresa("", "");
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            mClientes cliente = (mClientes) dgvDetalle.SelectedValue;

            if (cliente != null)
            {
                aceptaEmpresa(cliente.nidNid, cliente.nidNbr);
            }
            else
            {
                ObjComportamiento.showMsgError("Debe seleccionar una empresa");
                return;
            }
            
            contenedor.Close();
        }
    }
}

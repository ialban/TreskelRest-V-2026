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
    /// Lógica de interacción para ctlBscEmpleado.xaml
    /// </summary>
    public partial class ctlBscEmpleado : UserControl
    {
        public Window contenedor { get; set; }

        public event AceptaEmpleado aceptaEmpleado;
        public delegate void AceptaEmpleado(string codigo, string empleado);

        public ctlBscEmpleado(List<msEmpleado> empleados)
        {
            InitializeComponent();

            dgvDetalle.ItemsSource = empleados;

            dgvDetalle.Items.Refresh();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            msEmpleado empleado = (msEmpleado)dgvDetalle.SelectedValue;

            if (empleado != null)
            {
                aceptaEmpleado(empleado.codigo, empleado.empleado);
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
            aceptaEmpleado("", "");
            contenedor.Close();
        }
    }
}

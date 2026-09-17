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

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlQuitaIngredientes.xaml
    /// </summary>
    public partial class ctlQuitaIngredientes : UserControl
    {
        public Window contenedor { get; set; }
        public DataTable ingredientes { get; set; }

        private List<mIngredientes> noIngrediente = new List<mIngredientes>();

        public event AgregaItemSinIngredientes agregaItem;
        public delegate void AgregaItemSinIngredientes(String codigo, int cantidad, List<mIngredientes> lista, string observacion);

        public ctlQuitaIngredientes(String codigo, String item, DataTable dt)
        {
            InitializeComponent();
            this.ingredientes = dt;

            txtItem.Content = item;
            txtItem.Tag = codigo;
            txtCantidad.ValorMaximo = 20;
            txtCantidad.ValorMinimo = 1;
            txtCantidad.setValue("1");

            foreach(DataRow dr in dt.Rows)
            {
                mIngredientes row = new mIngredientes
                {
                    codigo = dr["prdCod"].ToString(),
                    nombre = dr["prdNbr"].ToString()
                };

                Resultado.Items.Add(row);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mIngredientes row = (mIngredientes)((FrameworkElement)sender).DataContext;
            String codigo = row.codigo;
            String nombre = row.nombre;

            noIngrediente.Add(row);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mIngredientes row = (mIngredientes)((FrameworkElement)sender).DataContext;
            String codigo = row.codigo;

            foreach(mIngredientes ingrediente in noIngrediente)
            {
                if (ingrediente.codigo.Equals(codigo))
                {
                    noIngrediente.Remove(ingrediente);
                    break;
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            agregaItem(txtItem.Tag.ToString(), int.Parse(txtCantidad.getValue()), noIngrediente, txtObserv.Text);
            btnCancelar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}

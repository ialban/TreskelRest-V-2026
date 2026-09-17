using Entidades;
using LibKeys;
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
    /// Interaction logic for ctlReceta.xaml
    /// </summary>
    public partial class ctlReceta : UserControl
    {
        private clsTextBox clsRct;

        public ctlReceta()
        {
            InitializeComponent();

            clsRct = new clsTextBox(txtProducto, General.USA_TECLADO_PANTALLA);

            clsRct.setMaxLength(20);
            clsRct.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsRct.permiteEspacios = true;
            clsRct.otrosCaracteresPermitidos = "./,()-:";

            btnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Items items = new Items(General.connectionString);
            DataTable dt = items.itemsReceta(txtProducto.Text, ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;
            Resultado.ItemsSource = dt.DefaultView;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nueva Receta";
            win.Width = 1000;
            win.Height = 550;

            ctlCreaReceta crea = new ctlCreaReceta();
            crea.contenedor = win;
            crea.accion = "N";
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((FrameworkElement)sender).DataContext;
            String codigo = row[0].ToString();

            winGeneral win = new winGeneral();
            win.Title = "Modificación de Receta";
            win.Width = 1000;
            win.Height = 550;

            ctlCreaReceta crea = new ctlCreaReceta();
            crea.contenedor = win;
            crea.accion = "M";
            crea.setNid(codigo);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }
    }
}

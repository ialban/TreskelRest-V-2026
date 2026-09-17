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
using Entidades;
using LibKeys;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlSubgrupos.xaml
    /// </summary>
    public partial class ctlSubgrupos : UserControl, IDisposable
    {
        private clsTextBox clsBsc;

        public ctlSubgrupos()
        {
            InitializeComponent();

            clsBsc = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsBsc.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsBsc.setMaxLength(20);

            Grupos grupos = new Grupos(General.connectionString);

            DataTable dt = grupos.listar();

            General.fillComboBoxFromDataTable(cboGrupo, dt, "T");            
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            SubGrupos subgrupos = new SubGrupos(General.connectionString);
            DataTable dt = subgrupos.buscar(txtApellidos.Text, ((ComboBoxItem)(cboGrupo.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;

            Resultado.ItemsSource = dt.DefaultView;
        }

        public void Actualiza_Trigger()
        {
            btnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void Editar_Click(Object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((FrameworkElement)sender).DataContext;
            String codigo = row[0].ToString();

            winGeneral win = new winGeneral();
            win.Title = "Modificación de Subgrupo";
            win.Width = 710;
            win.Height = 430;

            ctlCreaSubgrupo crea = new ctlCreaSubgrupo();
            crea.contenedor = win;
            crea.accion = "M";
            crea.parent = this;
            crea.setCodigo(codigo);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevo subgrupo";
            win.Width = 710;
            win.Height = 430;

            ctlCreaSubgrupo crea = new ctlCreaSubgrupo();
            crea.contenedor = win;
            crea.accion = "N";
            crea.parent = this;
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ctlSubgrupos() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

using Entidades;
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
    /// Interaction logic for ctlGrupos.xaml
    /// </summary>
    public partial class ctlGrupos : UserControl, IDisposable
    {
        public ctlGrupos()
        {
            InitializeComponent();

            Listar();
        }

        public void Listar(int opcion = 0)
        {
            Grupos grupos = new Grupos(General.connectionString);
            DataTable dt = grupos.listar(opcion);

            if(Resultado != null)
            {
                Resultado.ItemsSource = null;

                Resultado.ItemsSource = dt.DefaultView;
            }
        }

        private void cboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Listar(((ComboBox)sender).SelectedIndex);
        }

        private void Editar_Click(Object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((FrameworkElement)sender).DataContext;
            String codigo = row[0].ToString();

            winGeneral win = new winGeneral();
            win.Title = "Modificación de grupo";
            win.Width = 710;
            win.Height = 430;

            ctlCreaGrupo crea = new ctlCreaGrupo();
            crea.contenedor = win;
            crea.accion = "M";
            crea.parent = this;
            crea.setCod(codigo);
            crea.setEstado(cboEstado.SelectedIndex);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevo grupo";
            win.Width = 710;
            win.Height = 430;

            ctlCreaGrupo crea = new ctlCreaGrupo();
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
        // ~ctlGrupos() {
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

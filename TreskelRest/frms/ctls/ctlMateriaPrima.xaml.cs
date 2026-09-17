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
    /// Interaction logic for ctlMateriaPrima.xaml
    /// </summary>
    public partial class ctlMateriaPrima : UserControl, IDisposable
    {
        private clsTextBox clsPrd;

        public ctlMateriaPrima()
        {
            InitializeComponent();

            clsPrd = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsPrd.setMaxLength(20);
            clsPrd.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;

        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            MateriaPrima empleados = new MateriaPrima(General.connectionString);
            DataTable dt = empleados.buscar(txtApellidos.Text.ToUpper(), ((ComboBoxItem)(cboUnidad.SelectedItem)).Tag.ToString());

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
            win.Title = "Modificación de Materia Prima";
            win.Width = 920;
            win.Height = 420;

            ctlCreaMateriaPrima crea = new ctlCreaMateriaPrima();
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
            win.Title = "Crea nueva Materia Prima";
            win.Width = 920;
            win.Height = 420;

            ctlCreaMateriaPrima crea = new ctlCreaMateriaPrima();
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
        // ~ctlMateriaPrima() {
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

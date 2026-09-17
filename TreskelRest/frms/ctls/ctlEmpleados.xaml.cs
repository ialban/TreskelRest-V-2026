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
    /// Interaction logic for ctlEmpleados.xaml
    /// </summary>
    public partial class ctlEmpleados : UserControl, IDisposable
    {
        private clsTextBox clsApe;

        public ctlEmpleados() 
        {
            InitializeComponent();

            clsApe = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsApe.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
            clsApe.setMaxLength(50);
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Empleados empleados = new Empleados(General.connectionString);
            DataTable dt = empleados.buscar(txtApellidos.Text,((ComboBoxItem) (cboEstado.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;
            
            Resultado.ItemsSource = dt.DefaultView;
        }

        public void Actualiza_Trigger()
        {
            btnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void Editar_Click(Object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView) ((FrameworkElement)sender).DataContext;
            String cedula = row[0].ToString();

            winGeneral win = new winGeneral();
            win.Title = "Modificación de empleado";
            win.Width = 850;
            win.Height = 580;

            ctlCreaEmpleado crea = new ctlCreaEmpleado();
            crea.contenedor = win;
            crea.accion = "M";
            crea.parent = this;
            crea.setNid(cedula);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevo empleado";
            win.Width = 850;
            win.Height = 580;

            ctlCreaEmpleado crea = new ctlCreaEmpleado();
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
        // ~ctlEmpleados() {
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

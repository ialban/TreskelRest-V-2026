using Entidades;
using LibKeys;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlClientes.xaml
    /// </summary>
    public partial class ctlClientes : UserControl, IDisposable
    {
        private clsTextBox clsCli;

        public ctlClientes()
        {
            InitializeComponent();

            clsCli = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsCli.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsCli.permiteEspacios = true;
            clsCli.otrosCaracteresPermitidos = "%";
            
            clsCli.setMaxLength(20);
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Clientes clientes = new Clientes(General.connectionString);
            DataTable dt = clientes.buscar(txtApellidos.Text.ToUpper(), ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

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
            String cedula = row[0].ToString();

            winGeneral win = new winGeneral();
            win.Title = "Modificación de cliente";
            win.Width = 710;
            win.Height = 430;

            ctlCreaCliente crea = new ctlCreaCliente();
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
            win.Title = "Crea nuevo cliente";
            win.Width = 710;
            win.Height = 430;

            ctlCreaCliente crea = new ctlCreaCliente();
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
        // ~ctlClientes() {
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

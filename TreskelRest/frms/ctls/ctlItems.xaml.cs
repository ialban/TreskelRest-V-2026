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
    /// Interaction logic for ctlItems.xaml
    /// </summary>
    public partial class ctlItems : UserControl, IDisposable
    {
        private clsTextBox clsPrd;

        public ctlItems()
        {
            InitializeComponent();

            clsPrd = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsPrd.setMaxLength(20);
            clsPrd.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsPrd.permiteEspacios = true;
            clsPrd.otrosCaracteresPermitidos = "./,()-:";

            Grupos grupo = new Grupos(General.connectionString);

            DataTable dt = grupo.listar();
            General.fillComboBoxFromDataTable(cboGrupo, dt, "T");
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Items empleados = new Items(General.connectionString);
            DataTable dt = empleados.buscar(txtApellidos.Text.ToUpper(), ((ComboBoxItem)(cboGrupo.SelectedItem)).Tag.ToString());

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
            win.Title = "Modificación de Items para Venta";
            win.Width = 970;
            win.Height = 610;

            ctlCreaItems crea = new ctlCreaItems();
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
            win.Title = "Crea nuevos Items para Venta";
            win.Width = 970;
            win.Height = 610;

            ctlCreaItems crea = new ctlCreaItems();
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
        // ~ctlItems() {
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

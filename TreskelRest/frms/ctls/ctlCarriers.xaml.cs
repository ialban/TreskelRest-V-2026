using Entidades;
using Entidades.modelos;
using LibKeys;
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
    /// Interaction logic for ctlCarriers.xaml
    /// </summary>
    public partial class ctlCarriers : UserControl, IDisposable
    {
        private clsTextBox clsCrr;

        public ctlCarriers()
        {
            InitializeComponent();

            clsCrr = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsCrr.setMaxLength(20);
            clsCrr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.LETRAS;
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            mCarrier row = (mCarrier)((FrameworkElement)sender).DataContext;
            String codigo = row.codigo;

            winGeneral win = new winGeneral();
            win.Title = "Modificación de Carrier";
            win.Width = 690;
            win.Height = 430;

            ctlCreaCarrier crea = new ctlCreaCarrier();
            crea.contenedor = win;
            crea.parent = this;
            crea.setCodigo(codigo);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Carriers carriers = new Carriers(General.connectionString);

            List<mCarrier> data = carriers.buscar(txtApellidos.Text, ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;

            Resultado.ItemsSource = data;
        }

        public void Actualiza_Trigger()
        {
            btnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevos Carriers";
            win.Width = 690;
            win.Height = 420;

            ctlCreaCarrier crea = new ctlCreaCarrier();
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
        // ~ctlCarriers() {
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

using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlUsuarios.xaml
    /// </summary>
    public partial class ctlUsuarios : UserControl, IDisposable
    {
        private clsTextBox clsUsr;

        public ctlUsuarios()
        {
            InitializeComponent();

            clsUsr = new clsTextBox(txtApellidos, General.USA_TECLADO_PANTALLA);

            clsUsr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.USUARIOS;
            clsUsr.setMaxLength(20);
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Usuarios usuarios = new Usuarios(General.connectionString);

            List<mUsuario> data = usuarios.getUsuarios(txtApellidos.Text.ToUpper(), ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;

            Resultado.ItemsSource = data;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevo Usuario";
            win.Width = 915;
            win.Height = 610;

            ctlCreaUsuario crea = new ctlCreaUsuario();
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

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            mUsuario row = (mUsuario)((FrameworkElement)sender).DataContext;
            String codigo = row.codigo;

            winGeneral win = new winGeneral();
            win.Title = "Modificación de Usuario";
            win.Width = 915;
            win.Height = 610;

            ctlCreaUsuario crea = new ctlCreaUsuario();
            crea.contenedor = win;
            crea.accion = "M";
            crea.parent = this;
            crea.setCodigo(codigo);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
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
        // ~ctlUsuarios() {
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

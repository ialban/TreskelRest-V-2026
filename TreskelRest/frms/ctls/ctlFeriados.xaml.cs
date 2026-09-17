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
    /// Lógica de interacción para ctlFeriados.xaml
    /// </summary>
    public partial class ctlFeriados : UserControl, IDisposable
    {
        private bool disposedValue;
        private clsTextBox clsCli;

        public ctlFeriados()
        {
            InitializeComponent();

            clsCli = new clsTextBox(txtDesc, General.USA_TECLADO_PANTALLA);

            clsCli.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsCli.permiteEspacios = true;
            clsCli.otrosCaracteresPermitidos = "%";

            clsCli.setMaxLength(180);
        }

        public void Actualiza_Trigger()
        {
            btnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void Editar_Click(Object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((FrameworkElement)sender).DataContext;
            string codigo = row[0].ToString();

            winGeneral win = new winGeneral();
            win.Title = "Modificación de Feriado";
            win.Width = 810;
            win.Height = 480;

            ctlCreaFeriado crea = new ctlCreaFeriado();
            crea.contenedor = win;
            crea.accion = "M";
            crea.parent = this;
            crea.setNid(codigo);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Creación de Feriado";
            win.Width = 810;
            win.Height = 480;

            ctlCreaFeriado crea = new ctlCreaFeriado();
            crea.contenedor = win;
            crea.accion = "N";
            crea.parent = this;
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Feriado feriados = new Feriado(General.connectionString);
            DataTable dt = feriados.buscar(txtDesc.Text.ToUpper(), ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;

            Resultado.ItemsSource = dt.DefaultView;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~ctlFeriados()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

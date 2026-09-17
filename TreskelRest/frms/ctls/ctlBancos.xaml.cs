using Entidades;
using Entidades.modelos;
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
    /// Lógica de interacción para ctlBancos.xaml
    /// </summary>
    public partial class ctlBancos : UserControl, IDisposable
    {
        private bool disposedValue;

        public ctlBancos()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Bancos bancos = new Bancos(General.connectionString);
            List<lBancos> lista = bancos.buscar(txtApellidos.Text.ToUpper(), ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

            var institucionales = lista.Where(x => x.uso == "I").ToList();
            institucionales.ForEach(o => o.imagenUso = "../../images/institucion.png");

            var personales = lista.Where(x => x.uso == "P").ToList();
            personales.ForEach(o => o.imagenUso = "../../images/persona.png");

            Resultado.ItemsSource = null;
            Resultado.ItemsSource = lista;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevo Banco";
            win.Width = 820;
            win.Height = 490;

            ctlCreaBancos crea = new ctlCreaBancos();
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

        private void Editar_Click(Object sender, RoutedEventArgs e)
        {
            lBancos row = (lBancos)((FrameworkElement)sender).DataContext;
            int codBanco = row.codigo;

            winGeneral win = new winGeneral();
            win.Title = "Modificación de Bancos";
            win.Width = 820;
            win.Height = 490;

            ctlCreaBancos crea = new ctlCreaBancos();
            crea.contenedor = win;
            crea.accion = "M";
            crea.parent = this;
            crea.setNid(codBanco);
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
        }

        public void Actualiza_Trigger()
        {
            btnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
        // ~ctlBancos()
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

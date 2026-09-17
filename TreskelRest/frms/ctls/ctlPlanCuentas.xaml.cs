using Entidades;
using Entidades.modelos;
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
    /// Lógica de interacción para ctlPlanCuentas.xaml
    /// </summary>
    public partial class ctlPlanCuentas : UserControl, IDisposable
    {
        private bool disposedValue;

        public ctlPlanCuentas()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            PlanCuentas carriers = new PlanCuentas(General.connectionString);

            List<mPlanCuenta> data = carriers.buscar(txtPlan.Text, ((ComboBoxItem)(cboEstado.SelectedItem)).Tag.ToString());

            Resultado.ItemsSource = null;

            Resultado.ItemsSource = data;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Crea nuevos Planes de Cuenta Contable";
            win.Width = 1100;
            win.Height = 630;

            ctlCreaPlanCuenta crea = new ctlCreaPlanCuenta();
            //crea.contenedor = win;
            //crea.accion = "N";
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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
        // ~ctlPlanCuentas()
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

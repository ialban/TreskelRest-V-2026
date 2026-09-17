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
    /// Lógica de interacción para ctlNotasCredito.xaml
    /// </summary>
    public partial class ctlNotasCredito : UserControl, IDisposable
    {
        private clsTextBox clsCrr;
        private bool disposedValue;

        public ctlNotasCredito()
        {
            InitializeComponent();

            clsCrr = new clsTextBox(txtCriterio, General.USA_TECLADO_PANTALLA);

            clsCrr.setMaxLength(17);
            clsCrr.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsCrr.otrosCaracteresPermitidos = "-";

            dtpDesde.SelectedDate = General.GetPrimeraDiaFecha();
            dtpHasta.SelectedDate = General.GetUltimoDiaFecha();
            
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            winGeneral win = new winGeneral();
            win.Title = "Creación de Nota de Crédito";
            win.Width = 920;
            win.Height = 700;

            ctlCreaNotaCredito crea = new ctlCreaNotaCredito();
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            NotaCredito notaCredito = new NotaCredito(General.connectionString);

            DateTime desde = (DateTime)dtpDesde.SelectedDate;
            DateTime hasta = (DateTime)dtpHasta.SelectedDate;

            if (DateTime.Compare(desde, hasta) > 0)
            {
                ObjComportamiento.showMsgError("Error en el rango de fecha seleccionado, VERIFIQUE");
                return;
            }

            var resultado = notaCredito.GetNotasCredito(txtCriterio.Text, desde, hasta);

            if (resultado.Count > 0)
            {
                var ntcrAnl = resultado.Where(x => x.ntcrmtv == "ANULACIÓN").ToList();
                ntcrAnl.ForEach(o => o.ntcrmti = "../../images/anulacion.png");

                var ntcrDev = resultado.Where(x => x.ntcrmtv == "DEVOLUCIÓN").ToList();
                ntcrDev.ForEach(o => o.ntcrmti = "../../images/devolucion.png");
            }

            Resultado.ItemsSource = null;

            if (resultado != null && resultado.Count > 0)
            {
                Resultado.ItemsSource = resultado;
            }
            else
            {
                ObjComportamiento.showMsgInfo("No existen Notas de Crédito, con el criterio buscado");
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            mNotaCredito row = (mNotaCredito)((FrameworkElement)sender).DataContext;

            winGeneral win = new winGeneral();
            win.Title = "Creación de Nota de Crédito";
            win.Width = 1010;
            win.Height = 700;

            ctlCreaNotaCredito crea = new ctlCreaNotaCredito();
            crea.contenedor = win;
            crea.accion = "M";
            crea.aprobado = row.etdonce == "A";
            crea.anulado = row.etdonce == "L";
            crea.ntcrscl = row.ntcrscl;
            crea.ventscl = row.ventscl;
            //crea.Mostrar();
            crea.ObtenerNotaCredito();
            win.Contenido.Children.Add(crea);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowDialog();
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
        // ~ctlNotasCredito()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

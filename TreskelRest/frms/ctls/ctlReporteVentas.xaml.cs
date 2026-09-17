using LibReports;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlReporteVentas.xaml
    /// </summary>
    public partial class ctlReporteVentas : UserControl
    {
        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }

        public ctlReporteVentas()
        {
            InitializeComponent();

            txtAnio.ValorMaximo = 3000;
            txtAnio.ValorMinimo = 1990;

            txtAnio.setValue(DateTime.Now.Year.ToString());

            General.selectedByTagCombo(cboMes, DateTime.Now.Month.ToString());

            win.WindowStyle = WindowStyle.None;
            win.iniciaReporte += new winGeneral.IniciaReporte(EjecutaReporte);
            win.Width = 300;
            win.Height = 175;

            ctlEsperar esperar = new ctlEsperar();
            win.Contenido.Children.Add(esperar);

            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ResizeMode = ResizeMode.NoResize;
            win.Topmost = true;
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            win.ShowDialog();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void EjecutaReporte()
        {
            int anio = int.Parse(txtAnio.getValue().ToString());
            int mes = int.Parse(((ComboBoxItem)(cboMes.SelectedItem)).Tag.ToString());
            bool detalle = chkDetalle.IsChecked == true ? true : false;

            Thread t = new Thread(() => { ReporteVentas(win, anio, mes, detalle); });

            t.Start();
        }

        public void ReporteVentas(winGeneral win, int anio, int mes, bool detalle)
        {
            RptVentas rpt = new RptVentas(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(anio, mes, detalle);

            this.Dispatcher.Invoke(() =>
            {
                win.Visibility = Visibility.Hidden;
                General.PROCESANDO = false;

                if (!resp)
                {
                    ObjComportamiento.showMsgError("El reporte no pudo ser generado, verifique que no se encuentre abierto!");
                }
            });
        }
    }
}

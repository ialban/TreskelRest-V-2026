using LibReports;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlReportePedidos.xaml
    /// </summary>
    public partial class ctlReporteRegalos : UserControl
    {
        public Window contenedor { get; set; }
        private winGeneral win = new winGeneral();

        public ctlReporteRegalos()
        {
            InitializeComponent();

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
            string Desde = desde.Text;
            string Hasta = hasta.Text;
            Boolean detalle = chkDetalle.IsChecked == true ? true : false;

            Thread t = new Thread(() => { ReporteRegalos(Desde, Hasta, detalle); });

            t.Start();

        }

        public void ReporteRegalos(string desde, string hasta, Boolean detalle)
        {
            RptRegalos rpt = new RptRegalos(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = TreskelRest.cls.General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(desde, hasta, detalle);

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

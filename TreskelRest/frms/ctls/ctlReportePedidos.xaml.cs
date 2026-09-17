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
    public partial class ctlReportePedidos : UserControl
    {
        public Window contenedor { get; set; }
        private winGeneral win = new winGeneral();

        public ctlReportePedidos()
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

            Thread t = new Thread(() => { ReportePedidos(Desde, Hasta); });

            t.Start();

        }

        public void ReportePedidos(string desde, string hasta)
        {
            //RptPedidosPdf rpt = new RptPedidosPdf(General.connectionString, 1);
            //rpt.NombreEmpresa = TreskelRest.cls.General.EMPRESA;
            //rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            //Boolean resp = rpt.crear(desde, hasta);

            //this.Dispatcher.Invoke(() =>
            //{
            //    win.Visibility = Visibility.Hidden;
            //    General.PROCESANDO = false;

            //    if (!resp)
            //    {
            //        ObjComportamiento.showMsgError("El reporte no pudo ser generado, verifique que no se encuentre abierto!");
            //    }
            //});


            RptPedidos rpt = new RptPedidos(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = TreskelRest.cls.General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(desde, hasta);

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

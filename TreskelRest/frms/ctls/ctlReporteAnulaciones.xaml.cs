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
    public partial class ctlReporteAnulaciones : UserControl
    {
        public Window contenedor { get; set; }
        private winGeneral win = new winGeneral();

        public ctlReporteAnulaciones()
        {
            InitializeComponent();

            txtAnio.ValorMaximo = 3000;
            txtAnio.ValorMinimo = 1990;

            txtAnio.setValue(DateTime.Now.Year.ToString());

            FactElect.isChecked = true;
            fecha.SelectedDate = DateTime.Now;

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
            if (FactElect.isChecked == true)
            {
                DateTime dtfecha = (DateTime)fecha.SelectedDate;
                int anio = dtfecha.Year;
                int mes = dtfecha.Month;
                int dia = dtfecha.Day;

                Thread t = new Thread(() => { ReporteAnulacionesFE(win, anio, mes, dia); });

                t.Start();
            }
            else
            {
                int anio = int.Parse(txtAnio.getValue().ToString());
                int mes = int.Parse(((ComboBoxItem)(cboMes.SelectedItem)).Tag.ToString());

                Thread t = new Thread(() => { ReporteAnulaciones(win, anio, mes); });

                t.Start();
            }
        }

        public void ReporteAnulaciones(winGeneral win, int anio, int mes)
        {
            RptAnulaciones rpt = new RptAnulaciones(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(anio, mes);

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

        public void ReporteAnulacionesFE(winGeneral win, int anio, int mes, int dia)
        {
            RptAnulaciones rpt = new RptAnulaciones(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crearFE(anio, mes, dia);

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

        private void FactElect_ctlChecked(object sender, RoutedEventArgs e)
        {
            if(FactElect != null)
            {
                FE.IsEnabled = true;
                NoFE.IsEnabled = false;
            }
        }

        private void FactElect_ctlUnChecked(object sender, RoutedEventArgs e)
        {
            if (FactElect != null)
            {
                FE.IsEnabled = false;
                NoFE.IsEnabled = true;
            }
        }
    }
}

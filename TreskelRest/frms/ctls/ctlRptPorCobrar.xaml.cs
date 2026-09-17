using Entidades;
using Entidades.modelos;
using LibReports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlRptPorCobrar.xaml
    /// </summary>
    public partial class ctlRptPorCobrar : UserControl
    {
        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }

        Clientes clientes = new Clientes(General.connectionString);

        public ctlRptPorCobrar()
        {
            InitializeComponent();

            DateTime actual = DateTime.Now;

            DateTime inicio = new DateTime(actual.Year, actual.Month, 1);
            DateTime fin = inicio.AddMonths(1).AddDays(-1);

            desde.SelectedDate = inicio;
            hasta.SelectedDate = fin;

            cargaCombo(1, cboCliente);

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

        private void cargaCombo(int opcion, ComboBox combo)
        {
            if (combo == null) return;

            List<mClientes> cls = clientes.getClientesConCredito(opcion);

            combo.Items.Clear();

            foreach (mClientes cli in cls)
            {
                ComboBoxItem cboItem = new ComboBoxItem();

                cboItem.Tag = cli.nidNid;
                cboItem.Content = cli.nidNbr;
                cboItem.Height = 40;
                cboItem.VerticalContentAlignment = VerticalAlignment.Center;
                combo.Items.Add(cboItem);
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (desde.SelectedDate == null)
            {
                ObjComportamiento.showMsgError("Debe seleccionar una fecha de inicio");
                return;
            }

            if (hasta.SelectedDate == null)
            {
                ObjComportamiento.showMsgError("Debe seleccionar una fecha de fin");
                return;
            }

            if (hasta.SelectedDate < desde.SelectedDate)
            {
                ObjComportamiento.showMsgError("la fecha FIN debe ser mayor o igual a la fecha DESDE");
                return;
            }

            if (cboCliente.SelectedIndex < 0)
            {
                ObjComportamiento.showMsgError("Debe seleccionar un cliente");
                return;
            }
                
            //EjecutaReporte();
            win.ShowDialog();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void EjecutaReporte()
        {
            string Desde = DateTime.Parse(desde.Text).ToString("yyyy-MM-dd");
            string Hasta = DateTime.Parse(hasta.Text).ToString("yyyy-MM-dd");

            string idcliente = cboCliente.SelectedValue.ToString();
            string nbrcliente = cboCliente.Text.ToString();

            int tipo = PorCliente.IsChecked == true ? 0 : PorEmpresa.IsChecked == true ? 1 : 2;

            Thread t = new Thread(() => { ReportePedidosPorPagar(Desde, Hasta, idcliente, nbrcliente, tipo); });

            t.Start();
        }

        public void ReportePedidosPorPagar(string desde, string hasta, string id, string cliente, int tipo)
        {
            RptPedidosCobrar rpt = new RptPedidosCobrar(General.connectionString, General.TEMA_REPORTE);
            rpt.NombreEmpresa = TreskelRest.cls.General.EMPRESA;
            rpt.FormatoNumerico = General.FORMATO_NUMERICO_REPORTE;

            Boolean resp = rpt.crear(desde, hasta, id, cliente, tipo);

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

        private void PorCliente_Checked(object sender, RoutedEventArgs e)
        {
            cargaCombo(0, cboCliente);
        }

        private void PorEmpresa_Checked(object sender, RoutedEventArgs e)
        {
            cargaCombo(1, cboCliente);
        }

        private void ConsolidadoClientes_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

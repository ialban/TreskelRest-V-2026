using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para ctlMovBcos.xaml
    /// </summary>
    public partial class ctlMovBcos : UserControl
    {
        private class OpcionesTipo
        {
            public string codigo { get; set; }
            public string tipo { get; set; }
            public string descripcion { get; set; }
        }

        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }

        List<OpcionesTipo> opcionesTipo = null;
        private int origenSelected = -1;

        public ctlMovBcos()
        {
            InitializeComponent();

            opcionesTipo = new List<OpcionesTipo>
            {
                new OpcionesTipo { codigo = "DE", tipo = "I", descripcion = "Depósitos"},
                new OpcionesTipo { codigo = "TI", tipo = "I", descripcion = "Transferencia"},
                new OpcionesTipo { codigo = "LI", tipo = "I", descripcion = "Liquidación de Intereses"},
                new OpcionesTipo { codigo = "RT", tipo = "E", descripcion = "Retiros"},
                new OpcionesTipo { codigo = "TE", tipo = "E", descripcion = "Transferencia"},
                new OpcionesTipo { codigo = "RE", tipo = "E", descripcion = "Pago de Facturas"},
                new OpcionesTipo { codigo = "TJ", tipo = "E", descripcion = "Tarjetas de Crédito"},
                new OpcionesTipo { codigo = "CG", tipo = "E", descripcion = "Cobros de Gestión Bancaria"},
                new OpcionesTipo { codigo = "AJ", tipo = "O", descripcion = "Ajustes"},
                new OpcionesTipo { codigo = "MI", tipo = "O", descripcion = "Transferencias entre Cuentas"}
            };

            llenaComboTipo(cboTipo);

            Bancos bancos = new Bancos(General.connectionString);

            List<Entidades.modelos.lBancos> lBancos = bancos.buscar("", "A");
            foreach (var bco in lBancos)
            {
                string imagen = "";
                switch (bco.acronimo)
                {
                    case "BA":
                        imagen = "baustro.png";
                        break;
                    case "BB":
                        imagen = "bbolivariano.png";
                        break;
                    case "BG":
                        imagen = "bgquil.png";
                        break;
                    case "BI":
                        imagen = "binternacional.png";
                        break;
                    case "BM":
                        imagen = "bmachala.png";
                        break;
                    case "PB":
                        imagen = "bpacifico.png";
                        break;
                    case "BP":
                        imagen = "bpichincha.png";
                        break;
                    case "BD":
                        imagen = "produbanco.png";
                        break;
                    case "BR":
                        imagen = "bruminahui.png";
                        break;
                    case "BS":
                        imagen = "bsolidario.png";
                        break;
                    case "MP":
                        imagen = "mutPichincha.png";
                        break;
                    case "MA":
                        imagen = "mutAmbato.png";
                        break;
                    case "CP":
                        imagen = "coopPN.png";
                        break;
                    case "CO":
                        imagen = "coopOscus.png";
                        break;
                    case "29":
                        imagen = "coop29Oct.png";
                        break;
                    case "BE":
                        imagen = "banecuador.png";
                        break;
                }
                GeneraRadioButton(OpcOrigen, System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bco.nombre.ToLower()),
                    bco.acronimo + "-" + bco.codigo, imagen);
                
            }
        }

        private void cboTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cboTipo != null)
                {
                    llenaComboTipo((ComboBox)sender);
                }
            }
            catch(Exception er)
            {

            }
        }

        public void GeneraRadioButton(WrapPanel wpnl, string nombre, string acronimo, string imagen)
        {
            ToggleButton rb = new ToggleButton();
            rb.Tag = acronimo;
            rb.Height = 70;
            rb.Width = 90;
            rb.Checked += new RoutedEventHandler(ToggleOrigen_Checked);
            rb.Unchecked += new RoutedEventHandler(ToggleOrigen_UnChecked);

            StackPanel pnl = new StackPanel();
            pnl.Orientation = Orientation.Vertical;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri($"pack://application:,,,/images/{imagen}")); ;
            img.Width = 40;
            img.Height = 40;

            Label lbl = new Label();
            lbl.Content = nombre;
            lbl.FontSize = 9;

            pnl.Children.Add(img);
            pnl.Children.Add(lbl);
            rb.Content = pnl;

            wpnl.Children.Add(rb);
        }

        private void ToggleOrigen_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tgb = ((ToggleButton)sender);
                string[] acrocod = tgb.Tag.ToString().Split('-');
                string acronimo = acrocod[0];
                origenSelected = int.Parse(acrocod[1]);
                WrapPanel wpn = (WrapPanel)tgb.Parent;

                foreach (ToggleButton t in wpn.Children)
                {
                    if (t.Tag.ToString() != acronimo + "-" + origenSelected)
                        t.IsChecked = false;
                }
            }
            catch (Exception er)
            {

            }
        }

        private void ToggleOrigen_UnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                int cantidad = 0;
                foreach (ToggleButton t in OpcOrigen.Children)
                {
                    if (t.IsChecked == true)
                        cantidad++;
                }

                if (cantidad == 0)
                {
                    origenSelected = -1;
                }
            }
            catch (Exception er)
            {

            }
        }

        private void llenaComboTipo(ComboBox sender)
        {
            string tipo = ((ComboBoxItem)(sender).SelectedItem).Tag.ToString();
            var opciones = opcionesTipo.Where(x => x.tipo == tipo).ToList();

            cboTipo2.ItemsSource = null;
            cboTipo2.ItemsSource = opciones;
        }

        private void btnClsDsc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClsVal_Click(object sender, RoutedEventArgs e)
        {
            txtValor.Text = "0.00";
            txtValor.SelectAll();
            txtValor.Focus();
        }

        private void btnClsNid_Click(object sender, RoutedEventArgs e)
        {
            txtNid.Text = "";
            lblDestinatario.Content = "";
            txtNid.Focus();
        }

        private void btnBscNid_Click(object sender, RoutedEventArgs e)
        {
            lblDestinatario.Content = "Nombre del Cliente/Proveedor";
            switch (cboDestinatario.SelectedIndex)
            {
                case 0:
                    lblDestinatario.Visibility = Visibility.Visible;
                    Clientes clientes = new Clientes(General.connectionString);
                    clientes.nidNid = txtNid.Text;
                    if (clientes.getById())
                    {
                        lblDestinatario.Content = clientes.nidNbr;
                        lblDestinatario.Tag = clientes.nidNid;
                    }
                    else
                    {
                        ObjComportamiento.showMsgError("Cliente no encontrado");
                    }
                    
                    break;
                case 1:
                    lblDestinatario.Visibility = Visibility.Visible;
                    Proveedor proveedor = new Proveedor(General.connectionString);
                    proveedor.provNid = txtNid.Text;
                    if (proveedor.getById())
                    {
                        lblDestinatario.Content = proveedor.provNbr;
                        lblDestinatario.Tag = proveedor.provNid;
                    }
                    else
                    {
                        ObjComportamiento.showMsgError("Proveedor no encontrado");
                    }

                    break;
                case 2:
                    lblDestinatario.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void cboTipo2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cboTipo2 != null)
            //{
            //    if (cboTipo2.SelectedItem != null)
            //    {
            //        cboTipo2.Text = cboTipo2.SelectedItem.ToString();
            //    }
            //}
        }

        private void cboDestinatario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblDestinatario != null && txtNid != null)
            {
                txtNid.Text = "";
                if (cboDestinatario.SelectedIndex == 2) {
                    lblDestinatario.Visibility = Visibility.Hidden;
                    txtNid.Visibility = Visibility.Hidden;
                    txtOtro.Visibility = Visibility.Visible;
                    btnBscNid.Visibility = Visibility.Hidden;
                    btnClsNid.Margin = new Thickness(726, 280, 0, 0);
                }
                else
                {
                    lblDestinatario.Visibility = Visibility.Visible;
                    txtNid.Visibility = Visibility.Visible;
                    txtOtro.Visibility = Visibility.Hidden;
                    btnBscNid.Visibility = Visibility.Visible;
                    btnClsNid.Margin = new Thickness(576, 280, 0, 0);
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        public void limpiar()
        {
            origenSelected = -1;
            txtDescripcion.Text = "";
            cboTipo.SelectedIndex = 0;
            cboTipo2.SelectedIndex = -1;
            cboDestinatario.SelectedIndex = 0;
            txtValor.Text = "0.00";
            txtNid.Text = "";
            lblDestinatario.Tag = "";
            lblDestinatario.Content = "Nombre de Cliente/Proveedor";
            txtOtro.Text = "";

            foreach (ToggleButton t in OpcOrigen.Children)
            {
                t.IsChecked = false;
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "";
            if (origenSelected == -1)
            {
                mensaje += "- No se ha seleccionado un banco/cuenta" + Environment.NewLine;
            }
            if (txtDescripcion.Text.Length == 0)
            {
                mensaje += "- El campo descripción no puede estar en blanco" + Environment.NewLine;
            }

            if (double.Parse(txtValor.Text) == 0)
            {
                mensaje += "- El campo valor debe ser mayor a cero" + Environment.NewLine;
            }

            if (cboTipo2.SelectedIndex == -1)
            {
                mensaje += "- No se ha seleccionado un tipo de movimiento bancario" + Environment.NewLine;
            }

            int destino = cboDestinatario.SelectedIndex;
            if (destino == 0 || destino == 1)
            {
                string dest = lblDestinatario.Tag.ToString();
                if (dest == "")
                {
                    if (destino == 0)
                        mensaje += "- No se ha seleccionado un cliente" + Environment.NewLine;
                    else
                        mensaje += "- No se ha seleccionado un proveedor" + Environment.NewLine;
                }
            }
            else
            {
                if (txtOtro.Text.Trim() == "")
                {
                    mensaje += "- No se ha ingresado datos referentes al destinatario" + Environment.NewLine;
                }
            }

            if (mensaje != "")
            {
                ObjComportamiento.showMsgError(mensaje);
                return;
            }

            // Grabando la información
            if (ObjComportamiento.showMsgYesNo("¿Desea grabar la Transacción o Movimiento Bancario?") == MessageBoxResult.Yes)
            {
                // Grabando
                Bancos bancos = new Bancos(General.connectionString);

                double valor = General.StringToDouble(txtValor.Text);

                string cliente = "";
                string proveedor = "";
                string otro = "";

                if (cboDestinatario.SelectedIndex == 0)
                {
                    cliente = lblDestinatario.Tag.ToString();
                }
                else if (cboDestinatario.SelectedIndex == 1)
                {
                    proveedor = lblDestinatario.Tag.ToString();
                }
                else
                {
                    otro = txtOtro.Text;
                }

                Respuesta resp = bancos.grabarTransaccion(origenSelected, txtDescripcion.Text, cboTipo2.SelectedValue.ToString(), valor, cliente, proveedor, otro);

                if (resp.resultado)
                {
                    ObjComportamiento.showMsgInfo(resp.mensaje);
                    limpiar();
                }
                else
                {
                    ObjComportamiento.showMsgError(resp.mensaje);
                }
            }
        }
    }
}

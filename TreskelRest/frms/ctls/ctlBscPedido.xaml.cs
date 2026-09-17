using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlBscPedido.xaml
    /// </summary>
    public partial class ctlBscPedido : UserControl
    {
        public Window contenedor { get; set; }
        private string selectScl = "";
        private bool facturado = false;

        public event PedidoSeleccionado pedidoSeleccionado;
        public delegate void PedidoSeleccionado(string pedidoScl, bool facturado);

        public event CancelaBusqueda cancelaBusqueda;
        public delegate void CancelaBusqueda();

        public clsTextBox clsPed;

        public ctlBscPedido()
        {
            InitializeComponent();

            clsPed = new clsTextBox(txtPEfectivo, General.USA_TECLADO_PANTALLA);

            clsPed.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsPed.setMaxLength(10);

            button0.Click += new RoutedEventHandler(Button_Click);
            button1.Click += new RoutedEventHandler(Button_Click);
            button2.Click += new RoutedEventHandler(Button_Click);
            button3.Click += new RoutedEventHandler(Button_Click);
            button4.Click += new RoutedEventHandler(Button_Click);
            button5.Click += new RoutedEventHandler(Button_Click);
            button6.Click += new RoutedEventHandler(Button_Click);
            button7.Click += new RoutedEventHandler(Button_Click);
            button8.Click += new RoutedEventHandler(Button_Click);
            button9.Click += new RoutedEventHandler(Button_Click);
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ventas venta = new Ventas(General.connectionString);
                String codigo = txtPEfectivo.Text;
                DataTable dt = venta.buscarPedidoFacturado(codigo);

                txtPEfectivo.Text = "";
                selectScl = "";
                if (dt.Rows.Count == 0)
                {
                    ObjComportamiento.showMsgInfo("Pedido con secuencial " + codigo.PadLeft(10, '0') + " no existe o fue eliminado.");
                }
                else
                {
                    Resultado.Items.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {
                        PedidoBsc pedido = new PedidoBsc
                        {
                            codigo = dr["codigo"].ToString(),
                            ventscl = dr["ventscl"].ToString(),
                            mesa = dr["mesa"].ToString(),
                            tipo = dr["tipo"].ToString(),
                            facturado = (Boolean)dr["facturado"],
                            etdofac = dr["etdofac"].ToString() != "" ? "SI":"NO",
                            imagen = General.ConvertBitmapImageToImageSharp(((Boolean)dr["ventfac"]) == true ? "billing.png" : "proforma.png"), //new BitmapImage(new Uri("pack://application:,,,/images/" + (((Boolean)dr["ventfac"]) == true ? "billing.png" : "proforma.png"))),
                            secuencial = dr["secuencial"].ToString()
                        };

                        Resultado.Items.Add(pedido);

                        this.selectScl = pedido.ventscl;
                        this.facturado = pedido.facturado;
                    }

                    Resultado.Items.Refresh();
                }
            }
            catch(Exception ex)
            {

            }                   
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectScl != "")
                {
                    pedidoSeleccionado(selectScl, facturado);
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError("No ha seleccionado ningún pedido");
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            cancelaBusqueda();
            contenedor.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtPEfectivo.Text = "";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            String _txt = txtPEfectivo.Text.ToString();
            int _idx = txtPEfectivo.SelectionStart;

            if (txtPEfectivo.SelectionLength > 0)
            {
                txtPEfectivo.Text = txtPEfectivo.Text.Remove(_idx, txtPEfectivo.SelectionLength);
                txtPEfectivo.Focus();
                return;
            }

            if (_txt.Length > 0)
            {
                if (_idx > 0 && _idx < txtPEfectivo.Text.Length)
                {
                    String txt1 = txtPEfectivo.Text.Substring(0, _idx);
                    String txt2 = txtPEfectivo.Text.Substring(_idx);

                    txt1 = txt1.Remove(txt1.Length - 1, 1);
                    txtPEfectivo.Text = txt1 + txt2;
                }
                else
                {
                    if (_idx == 0) return;
                    txtPEfectivo.Text = _txt.Substring(0, _txt.Length - 1);
                }
                txtPEfectivo.SelectionStart = (_idx - 1) > 0 ? _idx - 1 : 0;
            }
            txtPEfectivo.Focus();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtPEfectivo.Text.ToString().LongCount(punto => punto.ToString() == ".") > 0 &&
                    ((Button)sender).Content.ToString().Equals("."))
            {
                txtPEfectivo.Focus();
                return;
            }

            if (txtPEfectivo.Text.Length == 10)
            {
                txtPEfectivo.Focus();
                return;
            }

            int _idx = txtPEfectivo.SelectionStart;
            String txt1 = "";
            String txt2 = "";

            int posDot = txtPEfectivo.Text.ToString().IndexOf(".");
            if (posDot >= 0 && _idx > posDot)
            {
                if (txtPEfectivo.Text.ToString().Substring(posDot + 1).Length == 2)
                {
                    txtPEfectivo.Focus();
                    return;
                }
            }

            if (_idx >= 0 && _idx < txtPEfectivo.Text.Length)
            {
                txt1 = txtPEfectivo.Text.Substring(0, _idx);
                txt2 = txtPEfectivo.Text.Substring(_idx);

                txtPEfectivo.Text = txt1 + ((Button)sender).Content.ToString() + txt2;
                txtPEfectivo.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
            }
            else
            {
                txtPEfectivo.Text += ((Button)sender).Content.ToString();
                txtPEfectivo.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
            }
            txtPEfectivo.Focus();
        }

        private void Resultado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PedidoBsc pedido = (PedidoBsc)e.AddedItems[0];

                this.selectScl = pedido.ventscl;
                this.facturado = pedido.facturado;
            }
            catch(Exception ex)
            {

            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ventas venta = new Ventas(General.connectionString);
                DataTable dt = venta.buscarUltimo();

                txtPEfectivo.Text = "";
                selectScl = "";
                if (dt.Rows.Count == 0)
                {
                    ObjComportamiento.showMsgInfo("No existen pedidos facturados por el momento.");
                }
                else
                {
                    Resultado.Items.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {
                        PedidoBsc pedido = new PedidoBsc
                        {
                            codigo = dr["codigo"].ToString(),
                            ventscl = dr["ventscl"].ToString(),
                            mesa = dr["mesa"].ToString(),
                            tipo = dr["tipo"].ToString(),
                            facturado = (Boolean)dr["facturado"],
                            etdofac = dr["etdofac"].ToString() != "" ? "SI" : "NO",
                            imagen = General.ConvertBitmapImageToImageSharp(((Boolean)dr["ventfac"]) == true ? "billing.png" : "proforma.png"), //new BitmapImage(new Uri("pack://application:,,,/images/" + (((Boolean)dr["ventfac"]) == true ? "billing.png" : "proforma.png"))),
                            secuencial = dr["secuencial"].ToString(),
                        };

                        Resultado.Items.Add(pedido);

                        this.selectScl = pedido.ventscl;
                        this.facturado = pedido.facturado;
                    }

                    Resultado.Items.Refresh();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

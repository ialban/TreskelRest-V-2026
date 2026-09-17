using Entidades;
using Entidades.modelos;
using LibKeys;
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
    /// Lógica de interacción para ctlBscReservacion.xaml
    /// </summary>
    public partial class ctlBscReservacion : UserControl
    {
        public clsTextBox clsCli;
        private String reservaScl = "";
        //-------------------------------------------------
        public Window contenedor { get; set; }
        public event CancelaBusqueda cancelaBusqueda;
        public delegate void CancelaBusqueda();

        public event ReservaSeleccionada reservaSeleccionada;
        public delegate void ReservaSeleccionada(String reservaScl);

        public ctlBscReservacion()
        {
            InitializeComponent();

            clsCli = new clsTextBox(txtPEfectivo, General.USA_TECLADO_PANTALLA);
            clsCli.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsCli.permiteEspacios = true;
            clsCli.setMaxLength(100);

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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            cancelaBusqueda();
            contenedor.Close();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (reservaScl != "")
                {
                    reservaSeleccionada(reservaScl);
                    contenedor.Close();
                }
                else
                {
                    ObjComportamiento.showMsgError("No ha seleccionado ninguna reserva");
                }
            }
            catch (Exception ex)
            {

            }
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

            if (txtPEfectivo.Text.Length == 100)
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reservacion reserva = new Reservacion(General.connectionString);
                string filtro = txtPEfectivo.Text;
                DataTable dt = reserva.getReservasBsc(filtro);

                txtPEfectivo.Text = "";
                reservaScl = "";
                Resultado.Items.Clear();
                if (dt != null)
                {
                    if (dt.Rows.Count == 0)
                    {
                        ObjComportamiento.showMsgInfo("Reserva para el criterio de búsqueda solicitado no existe");
                    }
                    else
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string img = "";
                            switch (dr["resemot"])
                            {
                                case "F":
                                    img = "holliday.png";
                                    break;
                                case "R":
                                    img = "romantico.png";
                                    break;
                                case "C":
                                    img = "cumpleanios.png";
                                    break;
                                case "L":
                                    img = "celebracion.png";
                                    break;
                                case "P":
                                    img = "propuestaMatrimonio.png";
                                    break;
                                case "M":
                                    img = "matrimonio.png";
                                    break;
                                default:
                                    img = "cancel.png";
                                    break;
                            }
                            mReservacion pedido = new mReservacion
                            {
                                reseNid = dr["resenid"].ToString(),
                                nidNid = dr["nidnid"].ToString(),
                                nidNbr = dr["nidnbr"].ToString(),
                                reseCnt = int.Parse(dr["resecnt"].ToString()),
                                reseFec = dr["resefec"].ToString(),
                                reseDes = dr["resedes"].ToString(),
                                reseHas = dr["resehas"].ToString(),
                                reseNot = dr["resenot"].ToString(),
                                reseMes = dr["resemes"].ToString(),
                                imagen = General.ConvertBitmapImageToImageSharp(img),//new BitmapImage(new Uri("pack://application:,,,/images/" + img))
                            };

                            Resultado.Items.Add(pedido);
                        }

                        Resultado.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Resultado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mReservacion reserva = (mReservacion)e.AddedItems[0];

                reservaScl = reserva.reseNid;
            }
            catch (Exception ex)
            {

            }
        }
    }
}

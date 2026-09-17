using Entidades.modelos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlPagos.xaml
    /// </summary>
    public partial class ctlPagos : UserControl
    {
        public Window contenedor { get; set; }
        private Boolean bPagoEfectivo = true;
        private Boolean bPagoCorriente = false;
        private Boolean bPagoDiferido = false;
        private int iTmpPago = 0;
        private string tarjeta = "N";
        private double valor = 0;
        private string Banco = "";
        private double saldo = 0;

        private TextBox _txt = new TextBox();
        private int opcion = 1;
        private int oldopcion = 1;

        private List<mPago> pagos = new List<mPago>();

        public event PagoRealizado pagoRealizado;
        public delegate void PagoRealizado(List<mPago> pagos);

        private clsTextBox clsVal;
        private clsTextBox clsChe;
        private clsTextBox clsLot;
        private clsTextBox clsRef;

        public ctlPagos(string valor)
        {
            InitializeComponent();

            clsVal = new clsTextBox(txtPEfectivo, false);
            clsChe = new clsTextBox(txtNCheque, false);
            clsLot = new clsTextBox(txtLote, false);
            clsRef = new clsTextBox(txtRef, false);

            clsVal.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsChe.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsLot.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsRef.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;

            clsVal.setMaxLength(6);
            clsChe.setMaxLength(6);
            clsLot.setMaxLength(6);
            clsRef.setMaxLength(10);

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
            buttonP.Click += new RoutedEventHandler(Button_Click);

            shcut2.Click += new RoutedEventHandler(Button_Click);
            shcut3.Click += new RoutedEventHandler(Button_Click);
            shcut4.Click += new RoutedEventHandler(Button_Click);
            shcut5.Click += new RoutedEventHandler(Button_Click);
            shcut6.Click += new RoutedEventHandler(Button_Click);
            shcut7.Click += new RoutedEventHandler(Button_Click);
            shcut8.Click += new RoutedEventHandler(Button_Click);
            shcut9.Click += new RoutedEventHandler(Button_Click);
            shcut10.Click += new RoutedEventHandler(Button_Click);
            shcut11.Click += new RoutedEventHandler(Button_Click);
            shcut12.Click += new RoutedEventHandler(Button_Click);

            lblTotal.Content = valor;
            saldo = General.StringToDouble(valor);
            this.valor = General.StringToDouble(valor);

            _txt.TextChanged += new TextChangedEventHandler(_txt_TextChange);
            txtPEfectivo.Focus();
            ubicaCursor(-1);
        }

        public void Limpiar()
        {
            if (txtPEfectivo != null) txtPEfectivo.Text = "";
            if (txtLote != null) txtLote.Text = "";
            if (txtRef != null) txtRef.Text = "";
            if (txtNCheque != null) txtNCheque.Text = "";
            if (_txt != null) _txt.Text = "";
        }

        public void _txt_TextChange(object sender, TextChangedEventArgs e)
        {
            if (opcion == 1 && oldopcion == opcion) txtPEfectivo.Text = _txt.Text;
            if (opcion == 2 && oldopcion == opcion) txtLote.Text = _txt.Text;
            if (opcion == 3 && oldopcion == opcion) txtRef.Text = _txt.Text;
            if (opcion == 4 && oldopcion == opcion) txtNCheque.Text = _txt.Text;
        }

        private void ubicaCursor(int posStart)
        {
            if (posStart == -1)
            {
                if (opcion == 1) { if (txtPEfectivo != null) txtPEfectivo.Focus(); }
                if (opcion == 2) { if (txtLote != null) txtLote.Focus(); }
                if (opcion == 3) { if (txtRef != null) txtRef.Focus(); }
                if (opcion == 4) { if (txtNCheque != null) txtNCheque.Focus(); }
            }
            else
            {
                if (opcion == 1) { txtPEfectivo.SelectionStart=posStart;  txtPEfectivo.Focus(); }
                if (opcion == 2) { txtLote.SelectionStart = posStart; txtLote.Focus(); }
                if (opcion == 3) { txtRef.SelectionStart = posStart; txtRef.Focus(); }
                if (opcion == 4) { txtNCheque.SelectionStart = posStart; txtNCheque.Focus(); }
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_txt.Text.ToString().LongCount(punto => punto.ToString() == ".") > 0 &&
                    ((Button)sender).Content.ToString().Equals("."))
            {
                ubicaCursor(-1);
                return;
            }

            if (_txt.Text.Length == _txt.MaxLength)
            {
                ubicaCursor(-1);
                return;
            }

            int _idx = _txt.SelectionStart;
            string txt1 = "";
            string txt2 = "";

            int posDot = _txt.Text.ToString().IndexOf(".");
            if (posDot >= 0 && _idx > posDot)
            {
                if (_txt.Text.ToString().Substring(posDot + 1).Length == 2)
                {
                    //_txt.Focus();
                    ubicaCursor(-1);
                    return;
                }
            }

            if (_idx >= 0 && _idx < _txt.Text.Length)
            {
                txt1 = _txt.Text.Substring(0, _idx);
                txt2 = _txt.Text.Substring(_idx);

                _txt.Text = txt1 + ((Button)sender).Content.ToString() + txt2;
                _txt.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
            }
            else
            {
                _txt.Text += ((Button)sender).Content.ToString();
                _txt.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
            }
            ubicaCursor(_txt.SelectionStart);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (opcion == 1) txtPEfectivo.Text = "";
            if (opcion == 2) txtLote.Text = "";
            if (opcion == 3) txtRef.Text = "";
            if (opcion == 4) txtNCheque.Text = "";

            //txtPEfectivo.Text = "";
            _txt.Text = "";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            String _vtxt = _txt.Text.ToString();
            int _idx = _txt.SelectionStart;

            if (txtPEfectivo.SelectionLength > 0)
            {
                _txt.Text = txtPEfectivo.Text.Remove(_idx, _txt.SelectionLength);
                ubicaCursor(-1);
                return;
            }

            if (_vtxt.Length > 0)
            {
                if (_idx > 0 && _idx < txtPEfectivo.Text.Length)
                {
                    String txt1 = _txt.Text.Substring(0, _idx);
                    String txt2 = _txt.Text.Substring(_idx);

                    txt1 = txt1.Remove(txt1.Length - 1, 1);
                    _txt.Text = txt1 + txt2;
                }
                else
                {
                    if (_idx == 0) return;
                    _txt.Text = _vtxt.Substring(0, _vtxt.Length - 1);
                }
                _txt.SelectionStart = (_idx - 1) > 0 ? _idx - 1 : 0;
            }
            //txtPEfectivo.Focus();
            ubicaCursor(_txt.SelectionStart);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            double valor = Math.Round(General.StringToDouble(txtPEfectivo.Text.ToString() == "" ? "0" : txtPEfectivo.Text.ToString()), 2);
            double total = General.StringToDouble(lblTotal.Content.ToString());

            double saldo = Math.Round(this.saldo,2) - valor;
            
            mPago pago;

            if (txtPEfectivo.Text.Length == 0)
            {
                ObjComportamiento.showMsgError("Debe ingresar el valor a pagar");
                return;
            }

            if (General.StringToDouble(txtPEfectivo.Text.ToString()) == 0)
            {
                ObjComportamiento.showMsgError("El valor a pagar debe ser mayo a 0");
                return;
            }

            if (bPagoEfectivo)
            {
                pago = new mPago
                {
                    iscorriente = false,
                    ischeque = false,
                    istrans = false,
                    isefectivo = true,
                    isdiferido = false,
                    numcheque = "",
                    numtrans = "",
                    banco = "",
                    meses = 0,
                    tarjeta = "N",
                    valor = valor,
                    vuelto = saldo > 0 ? 0 : valor - total
                };

                if (saldo > 0)
                {
                    this.saldo -= valor;
                }
            }
            else
            {
                if (saldo < 0)
                {
                    ObjComportamiento.showMsgError("El valor a pagar ingresado no debe superar al saldo");
                    return;
                }

                if (btnTarjeta.IsChecked == true)
                {
                    string lote = txtLote.Text.ToString();
                    string referencia = txtRef.Text.ToString();

                    if (valor > this.saldo)
                    {
                        txtPEfectivo.Text = this.saldo.ToString();
                        valor = this.saldo;
                        this.saldo = 0;
                    }

                    if (this.tarjeta.Equals("N"))
                    {
                        ObjComportamiento.showMsgError("Debe seleccionar una Tarjeta de Crédito con la cual se realizará el pago");
                        return;
                    }

                    if (lote == "")
                    {
                        ObjComportamiento.showMsgError("Escribir un número de Lote para la transacción con Tarjeta de Crédito");
                        return;
                    }

                    if (referencia == "")
                    {
                        ObjComportamiento.showMsgError("Escribir un número de Referencia para la transacción con Tarjeta de Crédito");
                        return;
                    }

                    Boolean pCorriente = pgCorriente.IsChecked == true ? true : false;
                    Boolean pDiferido = pgDiferido.IsChecked == true ? true : false;                    

                    pago = new mPago
                    {
                        iscorriente = pCorriente,
                        isefectivo = false,
                        ischeque = false,
                        istrans = false,
                        numcheque = "",
                        numtrans = "",
                        banco = "",
                        lote = lote.PadLeft(10,'0'),
                        referencia = referencia.PadLeft(10, '0'),
                        isdiferido = pDiferido,
                        meses = iTmpPago,
                        tarjeta = this.tarjeta,
                        valor = valor
                    };
                }
                else if (btnTransf.IsChecked == true)
                {
                    if (this.Banco == "")
                    {
                        ObjComportamiento.showMsgError("Debe seleccionar una Entidad Bancaria de origen de la Transferencia");
                        return;
                    }

                    if (txtNCheque.Text.Trim().Length == 0)
                    {
                        ObjComportamiento.showMsgError("Debe ingresar un número de la transferencia");
                        return;
                    }

                    pago = new mPago
                    {
                        iscorriente = false,
                        isefectivo = false,
                        isdiferido = false,
                        ischeque = false,
                        istrans = true,
                        numcheque = "",
                        numtrans = txtNCheque.Text.ToString().PadLeft(6, '0'),
                        banco = this.Banco,
                        lote = "",
                        referencia = "",
                        meses = iTmpPago,
                        tarjeta = "N",
                        valor = valor
                    };
                }
                else
                {
                    if (this.Banco == "")
                    {
                        ObjComportamiento.showMsgError("Debe seleccionar una Entidad Bancaria Emisora del cheque");
                        return;
                    }

                    if (txtNCheque.Text.Trim().Length == 0)
                    {
                        ObjComportamiento.showMsgError("Debe ingresar un número de cheque");
                        return;
                    }

                    pago = new mPago
                    {
                        iscorriente = false,
                        isefectivo = false,
                        isdiferido = false,
                        istrans = false,
                        ischeque = true,
                        numcheque = txtNCheque.Text.ToString().PadLeft(6, '0'),
                        numtrans = "",
                        banco = this.Banco,
                        lote = "",
                        referencia = "",
                        meses = iTmpPago,
                        tarjeta = "N",
                        valor = valor
                    };
                }

            }
            pagos.Add(pago);
            dgvPagos.Items.Add(pago);
            txtPEfectivo.Text = "";
            txtLote.Text = "";
            txtRef.Text = "";
            txtNCheque.Text = "";
            _txt.Text = "";

            if (dgvPagos.Items.Count == 0)
            {
                lblSaldo.Content = string.Format("{0:0.00}", 0);
            }else
            {
                double tot = 0;
                foreach(mPago lpago in dgvPagos.Items)
                {
                    tot += lpago.valor;
                }

                lblSaldo.Content = string.Format("{0:0.00}", total - tot);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void activaTarjetas(Boolean value)
        {
            if (btnMasterCard != null) btnMasterCard.IsEnabled = value;
            if (btnVisa != null) btnVisa.IsEnabled = value;
            if (btnAmerican != null) btnAmerican.IsEnabled = value;
            if (btnDiners != null) btnDiners.IsEnabled = value;
            if (btnDiscovery != null) btnDiscovery.IsEnabled = value;
        }

        private void muestraGrids(int value)
        {
            if(gridCC != null && gridCheque != null)
            {
                if (value == 1)
                {
                    gridCC.Visibility = Visibility.Hidden;
                    gridCheque.Visibility = Visibility.Hidden;
                }
                else if (value == 2)
                {
                    gridCC.Visibility = Visibility.Visible;
                    gridCheque.Visibility = Visibility.Hidden;
                }
                else
                {
                    gridCC.Visibility = Visibility.Hidden;
                    gridCheque.Visibility = Visibility.Visible;
                }
            }
            
        }

        private void TarjetasNoChecked()
        {
            if (btnVisa != null) btnVisa.IsChecked = false;
            if (btnMasterCard != null) btnMasterCard.IsChecked = false;
            if (btnDiners != null) btnDiners.IsChecked = false;
            if (btnDiscovery != null) btnDiscovery.IsChecked = false;
            if (btnAmerican != null) btnAmerican.IsChecked = false;
        }

        private void btnCash_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if(btnTarjeta != null) btnTarjeta.IsChecked = false;
                if(btnCheque != null) btnCheque.IsChecked = false;
                if (btnTransf != null) btnTransf.IsChecked = false;
                if (lblTitulo != null) lblTitulo.Content = "Pago en Efectivo";
                this.bPagoEfectivo = true;
                muestraGrids(1);

                TarjetasNoChecked();

                tarjeta = "N";
                Banco = "";
                activaTarjetas(false);
                Limpiar();
                ubicaCursor(-1);
            }
            catch(Exception ex)
            {

            }            
        }

        private void btnTarjeta_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnCash.IsChecked = false;
                btnCheque.IsChecked = false;
                btnTransf.IsChecked = false;
                lblTitulo.Content = "Pago con Tarjeta";
                muestraGrids(2);
                activaTarjetas(true);
                tarjeta = "N";
                Banco = "";
                TarjetasNoChecked();
                this.bPagoEfectivo = false;
                pgCorriente.IsChecked = true;
                pgDiferido.IsChecked = false;
                Limpiar();
                ubicaCursor(-1);
            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnCheque_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnCash.IsChecked = false;
                btnTransf.IsChecked = false;
                btnTarjeta.IsChecked = false;
                lblTitulo.Content = "Pago con Cheque";
                lblContCheque.Content = "Numero de Cheque:";
                muestraGrids(3);
                activaTarjetas(false);
                TarjetasNoChecked();
                tarjeta = "N";
                Banco = "";
                this.bPagoEfectivo = false;
                Limpiar();
                ubicaCursor(-1);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnTransf_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnCash.IsChecked = false;
                btnCheque.IsChecked = false;
                btnTarjeta.IsChecked = false;
                lblTitulo.Content = "Pago con Transferencia";
                lblContCheque.Content = "Numero de Comprobante:";
                muestraGrids(3);
                activaTarjetas(false);
                TarjetasNoChecked();
                tarjeta = "N";
                Banco = "";
                this.bPagoEfectivo = false;
                Limpiar();
                ubicaCursor(-1);
            }
            catch (Exception ex)
            {

            }
        }


        private void pgCorriente_Checked(object sender, RoutedEventArgs e)
        {
            pgDiferido.IsChecked = false;

            grdMeses.Visibility = Visibility.Hidden;

            pg3meses.IsChecked = false;
            pg6meses.IsChecked = false;
            pg9meses.IsChecked = false;

            iTmpPago = 0;
        }

        private void pgDiferido_Checked(object sender, RoutedEventArgs e)
        {
            pgCorriente.IsChecked = false;
            if (pgDiferido.IsChecked == true)
            {
                grdMeses.Visibility = Visibility.Visible;
            }
        }

        private void pg3meses_Checked(object sender, RoutedEventArgs e)
        {
            pg6meses.IsChecked = false;
            pg9meses.IsChecked = false;

            if(pg3meses.IsChecked == true)
            {
                this.iTmpPago = 3;
            }else
            {
                this.iTmpPago = 0;
            }
        }

        private void pg6meses_Checked(object sender, RoutedEventArgs e)
        {
            pg3meses.IsChecked = false;
            pg9meses.IsChecked = false;

            if (pg6meses.IsChecked == true)
            {
                this.iTmpPago = 6;
            }
            else
            {
                this.iTmpPago = 0;
            }
        }

        private void pg9meses_Checked(object sender, RoutedEventArgs e)
        {
            pg6meses.IsChecked = false;
            pg3meses.IsChecked = false;

            if (pg6meses.IsChecked == true)
            {
                this.iTmpPago = 9;
            }
            else
            {
                this.iTmpPago = 0;
            }
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPagos.Items.Count == 0)
            {
                ObjComportamiento.showMsgError("Debe ingresar el o los valores a pagar");
                return;
            }

            double saldo = General.StringToDouble(lblSaldo.Content.ToString());
            double total = General.StringToDouble(lblTotal.Content.ToString());

            if (saldo > 0)
            {
                ObjComportamiento.showMsgError("El valor a cancelar debe ser por lo menos " + lblTotal.Content.ToString());
                return;
            }

            pagos.Clear();

            foreach (mPago pago in dgvPagos.Items)
            {
                pagos.Add(pago);
            }
            
            pagoRealizado(pagos);
            contenedor.Close();
        }

        private void btnVisa_Checked(object sender, RoutedEventArgs e)
        {
            btnMasterCard.IsChecked = false;
            btnAmerican.IsChecked = false;
            btnDiscovery.IsChecked = false;
            btnDiners.IsChecked = false;

            pgCorriente.IsChecked = true;
            pgDiferido.IsChecked = false;
            this.tarjeta = "V";
            ubicaCursor(-1);
        }

        private void btnMasterCard_Checked(object sender, RoutedEventArgs e)
        {
            btnVisa.IsChecked = false;
            btnAmerican.IsChecked = false;
            btnDiscovery.IsChecked = false;
            btnDiners.IsChecked = false;

            this.tarjeta = "M";
            ubicaCursor(-1);
        }

        private void btnAmerican_Checked(object sender, RoutedEventArgs e)
        {
            btnMasterCard.IsChecked = false;
            btnVisa.IsChecked = false;
            btnDiscovery.IsChecked = false;
            btnDiners.IsChecked = false;

            this.tarjeta = "A";
            ubicaCursor(-1);
        }

        private void btnDiscovery_Checked(object sender, RoutedEventArgs e)
        {
            btnMasterCard.IsChecked = false;
            btnAmerican.IsChecked = false;
            btnVisa.IsChecked = false;
            btnDiners.IsChecked = false;

            this.tarjeta = "D";
            ubicaCursor(-1);
        }

        private void btnDiners_Checked(object sender, RoutedEventArgs e)
        {
            btnMasterCard.IsChecked = false;
            btnAmerican.IsChecked = false;
            btnDiscovery.IsChecked = false;
            btnVisa.IsChecked = false;

            this.tarjeta = "C";
            ubicaCursor(-1);
        }

        private void btnSaldo_Click(object sender, RoutedEventArgs e)
        {
            double tot = General.StringToDouble(lblTotal.Content.ToString());
            double val = General.StringToDouble(lblSaldo.Content.ToString());
            this.opcion = 1;
            this.oldopcion = 1;
            txtPEfectivo.Text = String.Format("{0:0.00}",tot - val);
            ubicaCursor(-1);
        }

        private void Banco_Checked(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Tag.ToString())
            {
                case "BA":                    
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoAustro.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BA";
                    break;
                case "BB":
                    BcoAustro.IsChecked = false;                    
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoBolivariano.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BB";
                    break;
                case "BG":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;                    
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoGquil.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BG";
                    break;
                case "BI":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;                    
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoInternacional.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BI";
                    break;
                case "BM":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;                    
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoMachala.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BM";
                    break;
                case "PB":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;                    
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoPacifico.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "PB";
                    break;
                case "BP":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;                    
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;                    
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoPichincha.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BP";
                    break;                                                                
                case "BD":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;                    
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoProdubanco.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BD";
                    break;                
                case "BR":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;                    
                    BcoSolidario.IsChecked = false;
                    BcoRuminahui.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BR";
                    break;
                case "BS":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BS";
                    break;
                case "MP":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = true;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "MP";
                    break;
                case "MA":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = true;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "MA";
                    break;
                case "CP":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = true;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "CP";
                    break;
                case "CO":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = true;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "CO";
                    break;
                case "CJ":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = true;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "CJ";
                    break;
                case "29":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = true;
                    BanEc.IsChecked = false;
                    this.Banco = "29";
                    break;
                case "BE":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = true;
                    this.Banco = "BE";
                    break;
            }
            ubicaCursor(-1);
        }

        private void Banco_Unchecked(object sender, RoutedEventArgs e)
        {
            BcoAustro.IsChecked = false;
            BcoBolivariano.IsChecked = false;
            BcoGquil.IsChecked = false;
            BcoInternacional.IsChecked = false;
            BcoMachala.IsChecked = false;
            BcoPacifico.IsChecked = false;
            BcoPichincha.IsChecked = false;
            BcoProdubanco.IsChecked = false;
            BcoRuminahui.IsChecked = false;
            BcoSolidario.IsChecked = false;
            this.Banco = "";
            ubicaCursor(-1);
        }

        private void txtPEfectivo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (oldopcion != 1) { oldopcion = 1; _txt.Text = txtPEfectivo.Text; }
            opcion = 1;
            _txt.MaxLength = txtPEfectivo.MaxLength;
            _txt.SelectionStart = txtPEfectivo.SelectionStart;
            buttonP.IsEnabled = true;
            btnAdd.IsEnabled = true;
        }

        private void txtLote_GotFocus(object sender, RoutedEventArgs e)
        {
            if (oldopcion != 2) { oldopcion = 2; _txt.Text = txtLote.Text;  }
            opcion = 2;            
            _txt.MaxLength = txtLote.MaxLength;
            _txt.SelectionStart = txtLote.SelectionStart;
            buttonP.IsEnabled = false;
            btnAdd.IsEnabled = false;
        }

        private void txtRef_GotFocus(object sender, RoutedEventArgs e)
        {
            if (oldopcion != 3) { oldopcion = 3; _txt.Text = txtRef.Text; }
            opcion = 3;
            _txt.MaxLength = txtRef.MaxLength;
            _txt.SelectionStart = txtRef.SelectionStart;
            buttonP.IsEnabled = false;
            btnAdd.IsEnabled = false;
        }

        private void txtNCheque_GotFocus(object sender, RoutedEventArgs e)
        {
            if(oldopcion!=4) { oldopcion = 4; _txt.Text = txtNCheque.Text; }
            opcion = 4;
            _txt.MaxLength = txtNCheque.MaxLength;
            _txt.SelectionStart = txtNCheque.SelectionStart;            
            buttonP.IsEnabled = false;
            btnAdd.IsEnabled = false;
        }

        private void txtRef_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _txt.SelectionStart = txtRef.SelectionStart;
            _txt.SelectionLength = txtRef.SelectionLength;
        }

        private void txtLote_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _txt.SelectionStart = txtLote.SelectionStart;
            _txt.SelectionLength = txtLote.SelectionLength;
        }

        private void txtPEfectivo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _txt.SelectionStart = txtPEfectivo.SelectionStart;
            _txt.SelectionLength = txtPEfectivo.SelectionLength;
        }

        private void txtNCheque_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _txt.SelectionStart = txtNCheque.SelectionStart;
            _txt.SelectionLength = txtNCheque.SelectionLength;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            mPago row = (mPago)((FrameworkElement)sender).DataContext;

            dgvPagos.Items.Remove(row);

            var pagos = dgvPagos.Items.OfType<mPago>().ToList().Sum(x => x.valor);
            var total = General.StringToDouble(lblTotal.Content.ToString());

            this.saldo = total - pagos;
            
            lblSaldo.Content = String.Format("{0:0.00}", this.saldo);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ubicaCursor(-1);
        }
    }
}

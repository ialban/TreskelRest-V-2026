using Entidades;
using Entidades.modelos;
using LibKeys;
using LibPrint;
using System;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlCierreCaja.xaml
    /// </summary>
    public partial class ctlCierreCaja : UserControl
    {
        private String tipo = "I";
        private String caja = "";
        private String sec = "";
        public Window contenedor { get; set; }

        public clsTextBox clsIni;
        public clsTextBox clsFin;

        public clsTextBox clsEfectivo;
        public clsTextBox clsTarjeta;
        public clsTextBox clsCheque;
        public clsTextBox clsDelivery;
        public clsTextBox clsTransfer;

        private double efectivo = 0;
        private double tarjeta = 0;
        private double cheque = 0;
        private double delivery = 0;
        private double transfer = 0;

        public void limpiar()
        {
            Inicio.Text = "0.00";
            Cierre.Text = "0.00";
            Sistema.Text = "0.00";
            Diferencia.Text = "0.00";
            Efectivo.Text = "0.00";
            Tarjeta.Text = "0.00";
            Cheque.Text = "0.00";
            Delivery.Text = "0.00";
            Transfer.Text = "0.00";
            dtpFecha.Text = DateTime.Today.ToString();
        }

        private void fnActivaInicio(Boolean valor)
        {
            Inicio.IsEnabled = valor;
            Cierre.IsEnabled = !valor;
            Efectivo.IsEnabled = !valor;
            Tarjeta.IsEnabled = !valor;
            Cheque.IsEnabled = !valor;
            Transfer.IsEnabled = !valor;
            Delivery.IsEnabled = !valor;
        }


        private Double calculaDiferencia(Double cierre, Double sistema)
        {
            return sistema - cierre;
        }


        public ctlCierreCaja(String tipo, String caja)
        {
            InitializeComponent();

            clsIni = new clsTextBox(Inicio, General.USA_TECLADO_PANTALLA);
            clsFin = new clsTextBox(Cierre, General.USA_TECLADO_PANTALLA);
            clsEfectivo = new clsTextBox(Efectivo, General.USA_TECLADO_PANTALLA);
            clsTarjeta = new clsTextBox(Tarjeta, General.USA_TECLADO_PANTALLA);
            clsCheque = new clsTextBox(Cheque, General.USA_TECLADO_PANTALLA);
            clsTransfer = new clsTextBox(Transfer, General.USA_TECLADO_PANTALLA);
            clsDelivery = new clsTextBox(Delivery, General.USA_TECLADO_PANTALLA);

            clsIni.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsFin.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsEfectivo.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsTarjeta.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsCheque.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsTransfer.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;
            clsDelivery.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES;


            clsIni.setMaxLength(8);
            clsFin.setMaxLength(8);
            clsEfectivo.setMaxLength(8);
            clsTarjeta.setMaxLength(8);
            clsCheque.setMaxLength(8);
            clsTransfer.setMaxLength(8);
            clsDelivery.setMaxLength(8);

            this.tipo = tipo;
            this.caja = caja;

            this.limpiar();
            lblCaja.Content = caja;

            if (tipo.Equals("I"))
            {
                fnActivaInicio(true);
                Inicio.Focus();
            }else
            {
                fnActivaInicio(false);
                Cierre.Focus();
            }

            Sistema.IsEnabled = false;
            Diferencia.IsEnabled = false;

            MovimientoCaja _caja = new MovimientoCaja(General.connectionString);

            hinicio.Text = _caja.getFechaBase();
            string[] hora = hinicio.Text.Split(':');
            lHora.Content = hora[0];
            lMinuto.Content = hora[1];
            sHora.Value = General.StringToDouble(hora[0]);
            sMinuto.Value = General.StringToDouble(hora[1]);

            string fecha = string.Format("{0:yyyy-MM-dd}", DateTime.Parse(dtpFecha.Text));

            mControl control = _caja.ControlCaja(fecha, General.CAJA);


            if (tipo == "I")
            {
                Imprimir.Visibility = Visibility.Hidden;
            }
            else
            {
                Imprimir.Visibility = Visibility.Visible;
            }

            Double _sistema = control.ingresos + control.ventas + control.inicio - control.compras - control.egresos - control.salarios;
            Inicio.Text = String.Format("{0:0.00}", control.inicio);
            Cierre.Text = String.Format("{0:0.00}", control.cierre);
            Sistema.Text = String.Format("{0:0.00}", _sistema);
            Diferencia.Text = String.Format("{0:0.00}", calculaDiferencia(control.cierre, _sistema));
            Efectivo.Text = String.Format("{0:0.00}", control.efectivo);
            Tarjeta.Text = String.Format("{0:0.00}", control.tarjeta);
            Cheque.Text = String.Format("{0:0.00}", control.cheque);
            Delivery.Text = String.Format("{0:0.00}", control.delivery);
            Transfer.Text = String.Format("{0:0.00}", control.transfer);
            this.sec = control.secuencial;
        }

        private void horaUp_Click(object sender, RoutedEventArgs e)
        {
            int hora = int.Parse(lHora.Content.ToString());

            if(hora + 1 > 23)
            {
                return;
            }

            hora++;
            lHora.Content = String.Format("{0:00}", hora);
            sHora.Value = hora;
        }

        private void MinutoUp_Click(object sender, RoutedEventArgs e)
        {
            int minuto = int.Parse(lMinuto.Content.ToString());

            if (minuto + 1 > 59)
            {
                return;
            }

            minuto++;
            lMinuto.Content = String.Format("{0:00}", minuto);
            sMinuto.Value = minuto;
        }

        private void horaDown_Click(object sender, RoutedEventArgs e)
        {
            int hora = int.Parse(lHora.Content.ToString());

            if (hora - 1 < 0)
            {
                return;
            }

            hora--;
            lHora.Content = String.Format("{0:00}", hora);
            sHora.Value = hora;
        }

        private void MinutoDown_Click(object sender, RoutedEventArgs e)
        {
            int minuto = int.Parse(lMinuto.Content.ToString());

            if (minuto - 1 < 0)
            {
                return;
            }

            minuto--;
            lMinuto.Content = String.Format("{0:00}", minuto);
            sMinuto.Value = minuto;
        }

        private void hInicioOk_Click(object sender, RoutedEventArgs e)
        {
            hinicio.Text = lHora.Content + ":" + lMinuto.Content;
            popupControl.IsOpen = false;
        }

        private void sHora_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lHora.Content = String.Format("{0:00}", sHora.Value);
        }

        private void sMinuto_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lMinuto.Content = String.Format("{0:00}", sMinuto.Value);
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            String msg = "";

            if (Efectivo.Text == "") Efectivo.Text = "0.00";
            if (Tarjeta.Text == "") Tarjeta.Text = "0.00";
            if (Cheque.Text == "") Cheque.Text = "0.00";
            if (Transfer.Text == "") Transfer.Text = "0.00";
            if (Delivery.Text == "") Delivery.Text = "0.00";

            if (Cierre.Text!= "")
            {
                if (double.Parse(Cierre.Text) <= 0 && !this.tipo.Equals("I"))
                {
                    var resp = ObjComportamiento.showMsgYesNo(
                        "El valor de cierre es cero, ¿Está seguro(a) de grabar la información con éste valor?");

                    if (resp == MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }


            if (this.tipo.Equals("I")){
                if (Inicio.Text.Trim().Equals(""))
                {
                    msg = "El Valor de inicio no puede estar en blanco";
                }else
                {
                    if (General.StringToDouble(Inicio.Text) < 0)
                    {
                        msg = "El Valor de inicio debe ser mayor o igual a cero";
                    }
                }

                if (!msg.Equals(""))
                {
                    ObjComportamiento.showMsgError(msg);
                    return;
                }

                MovimientoCaja _caja = new MovimientoCaja(General.connectionString);

                _caja.tipo = this.tipo;
                _caja.caja = this.caja;

                string fecha = string.Format("{0:yyyy-MM-dd}", DateTime.Parse(dtpFecha.Text));

                Respuesta respuesta= _caja.InicioCierre(fecha, hinicio.Text, Inicio.Text.Replace(",","."), "", "", "", "","", "", "");

                ObjComportamiento.showMuestraMensaje(respuesta);
            }else
            {
                if (Cierre.Text.Trim().Equals(""))
                {
                    msg = "El Valor de cierre no puede estar en blanco";
                }
                else
                {
                    if (General.StringToDouble(Inicio.Text) < 0)
                    {
                        msg = "El Valor de cierre debe ser mayor o igual a cero";
                    }
                }

                if (!msg.Equals(""))
                {
                    ObjComportamiento.showMsgError(msg);
                    return;
                }

                MovimientoCaja _caja = new MovimientoCaja(General.connectionString);

                _caja.tipo = this.tipo;
                _caja.caja = this.caja;
                _caja.secuencial = this.sec;

                string fecha = string.Format("{0:yyyy-MM-dd}", DateTime.Parse(dtpFecha.Text));

                Respuesta respuesta = _caja.InicioCierre(fecha, hinicio.Text, Cierre.Text.Replace(",","."), Sistema.Text.Replace(",","."), Diferencia.Text.Replace(",","."),
                    Efectivo.Text.Replace(",","."), Tarjeta.Text.Replace(",", "."), Cheque.Text.Replace(",", "."), Transfer.Text.Replace(",","."), Delivery.Text.Replace(",", "."));
   
                if (General.IMPRIME_TICKET_CIERRE_CAJA)
                {
                    /*
                     * Obteniendo data para impresión del ticket. 
                     * Imprimiendo ticket de Cierre de caja
                     */
                    CierreCaja cierre = _caja.ResumenCierreCaja(fecha, caja);

                    EposPrint cls = new EposPrint();
                    cls.TipoImpresora = General.TIPO_IMPRESORAS;
                    Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);
                    cls.CabeceraNegrita = true;
                    if (resp.resultado == false)
                    {
                        ObjComportamiento.showMsgError(resp.mensaje);
                        return;
                    }

                    cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;

                    cls.DefineSeparador = char.Parse(General.SEPARADOR);
                    cls.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
                    cls.ImprimeNombreEmpresa = General.IMPRIME_NOMBRE_EMPRESA;
                    cls.ImprimeDireccionEmpresa = General.IMPRIME_DIRECCION_EMPRESA;
                    cls.ImprimeRucEmpresa = General.IMPRIME_RUC_EMPRESA;
                    cls.CabeceraNegrita = General.NEGRITA_EN_CABECERA;
                    cls.AlineacionCabecera = General.ALINEACION_CABECERA;
                    cls.ImprimeSeparadorEndCabecera = General.IMPRIME_SEPARADO_FIN_CABECERA;
                    cls.ImprimeNegritaCabeceraTabla = General.IMPRIME_NEGRITA_CABECERA_CONTENIDO;
                    cls.CantidadLineasSaltarEndTabla = General.CANTIDAD_LINEAS_FINAL_ITEMS;
                    cls.ImprimeSeparadorTablas = General.IMPRIME_SEPARADOR_FIN_ITEMS;
                    cls.CantidadDecimalesItems = General.CANTIDAD_DECIMALES_EN_ITEMS;
                    cls.ImprimeFormaDePago = General.IMPRIME_FORMA_PAGO;
                    cls.IncluyeDataMesero = General.IMPRIME_NOMBRE_MESERO;
                    cls.IncluyeAgradecimiento = General.IMPRIME_AGRADECIMIENTO;

                    cls.CierreTicket(General.EMPRESA, General.DIRECCION_LOCAL, General.RUC_EMPRESA, cierre, General.MUESTRA_ELIMINADOS_ANULADOS);
                    cls.sbIpr();
                    cls.Dispose();
                }

                
                /*if (!this.tipo.Equals("I") && respuesta.resultado)
                {
                    String mensaje = "<B>" + General.EMPRESA + "</B><br><HR>";
                    mensaje += "<p>Se ha realizado el cierre de la caja " + this.caja + "</p>";
                    mensaje += "<B>Detalle del " + dtpFecha.Text + "</B><BR>";
                    mensaje += "<TABLE>";
                    mensaje += "<TR><TD>INICIO</TD><TD>" + Inicio.Text + "</TD><TD></TD></TR>";
                    mensaje += "<TR><TD>CIERRE</TD><TD>" + Cierre.Text + "</TD><TD></TD></TR>";
                    mensaje += "<TR><TD>SISTEMA</TD><TD>" + Sistema.Text + "</TD><TD></TD></TR>";
                    mensaje += "<TR><TD></TD><TD>COMPRAS</TD><TD></TD></TR>";
                    mensaje += "<TR><TD></TD><TD>VENTAS</TD><TD></TD></TR>";
                    mensaje += "<TR><TD>DIFERENCIA</TD><TD>" + Diferencia.Text + "</TD><TD></TD></TR>";
                    mensaje += "</TABLE>";

                    Pedido pedido = new Pedido(General.connectionString);
                    List<rPedido> pedidos = pedido.reportePedidos(dtpFecha.Text, dtpFecha.Text);

                    mensaje += "<br>";
                    mensaje += "<TABLE border=1>";
                    mensaje += "<TR><TD colspan=2></TD><TD colspan=2><b>LOCAL</b></TD><TD colspan=2><b>DOMICILIO</b></TD><TD colspan=2><b>LLEVAR</b></TD><TD rowspan=2><b>CNT TOTAL</b></TD><TD rowspan=2><b>TOTAL</b></TD></TR>";
                    mensaje += "<TR><TD><b>FECHA</b></TD><TD><b>TIPO</b></TD><TD><b>CANTIDAD</b></TD><TD><b>VALOR</b></TD><TD><b>CANTIDAD</b></TD><TD><b>VALOR</b></TD><TD><b>CANTIDAD</b></TD><TD><b>VALOR</b></TD></TR>";

                    foreach (rPedido rpedido in pedidos)
                    {
                        mensaje += "<TR>";
                        mensaje += "<TD>" + rpedido.fecha + "</TD>";
                        mensaje += "<TD>" + rpedido.tipo + "</TD>";
                        mensaje += "<TD align=right>" + rpedido.lcnt + "</TD>";
                        mensaje += "<TD align=right>" + String.Format("{0:0.00}", rpedido.lpago) + "</TD>";
                        mensaje += "<TD align=right>" + rpedido.dcnt + "</TD>";
                        mensaje += "<TD align=right>" + String.Format("{0:0.00}", rpedido.dpago) + "</TD>";
                        mensaje += "<TD align=right>" + rpedido.ccnt + "</TD>";
                        mensaje += "<TD align=right>" + String.Format("{0:0.00}", rpedido.cpago) + "</TD>";
                        mensaje += "<TD align=right>" + rpedido.cantidad + "</TD>";
                        mensaje += "<TD align=right>" + String.Format("{0:0.00}", rpedido.total) + "</TD>";
                        mensaje += "</TR>";
                    }
                    
                    mensaje += "</TABLE>";

                    Email email = new Email(General.CORREO_HOST, General.CORREO_PORT, General.CORREO_USER, General.CORREO_PASSWORD, General.CORREO_AUTH, General.CORREO_UTLS, "");

                    email.subject = "Cierre de Caja de " + General.EMPRESA;
                    email.body = mensaje;

                    email.enviar();
                }*/

                ObjComportamiento.showMuestraMensaje(respuesta);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void Cierre_LostFocus(object sender, RoutedEventArgs e)
        {
            var cierre = Math.Round(General.StringToDouble(Cierre.Text), 2);
            var sistema = Math.Round(General.StringToDouble(Sistema.Text), 2);
            Diferencia.Text = String.Format("{0:0.00}", calculaDiferencia(cierre, sistema));
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            var fecha = string.Format("{0:yyyy-MM-dd}", dtpFecha.SelectedDate);
            MovimientoCaja _caja = new MovimientoCaja(General.connectionString);

            CierreCaja cierre = _caja.ResumenCierreCaja(fecha, caja);

            EposPrint cls = new EposPrint();
            cls.TipoImpresora = General.TIPO_IMPRESORAS;
            Respuesta resp = cls.setPrinter(General.PRINTER_FACTURA);
            cls.CabeceraNegrita = true;
            if (resp.resultado == false)
            {
                ObjComportamiento.showMsgError(resp.mensaje);
                return;
            }

            cls.cantidadLineas = General.CANTIDAD_LINEAS_IMPRESION;

            cls.DefineSeparador = char.Parse(General.SEPARADOR);
            cls.cantidadColumnas = General.CANTIDAD_COLUMNAS_IMPRESION;
            cls.ImprimeNombreEmpresa = General.IMPRIME_NOMBRE_EMPRESA;
            cls.ImprimeDireccionEmpresa = General.IMPRIME_DIRECCION_EMPRESA;
            cls.ImprimeRucEmpresa = General.IMPRIME_RUC_EMPRESA;
            cls.CabeceraNegrita = General.NEGRITA_EN_CABECERA;
            cls.AlineacionCabecera = General.ALINEACION_CABECERA;
            cls.ImprimeSeparadorEndCabecera = General.IMPRIME_SEPARADO_FIN_CABECERA;
            cls.ImprimeNegritaCabeceraTabla = General.IMPRIME_NEGRITA_CABECERA_CONTENIDO;
            cls.CantidadLineasSaltarEndTabla = General.CANTIDAD_LINEAS_FINAL_ITEMS;
            cls.ImprimeSeparadorTablas = General.IMPRIME_SEPARADOR_FIN_ITEMS;
            cls.CantidadDecimalesItems = General.CANTIDAD_DECIMALES_EN_ITEMS;
            cls.ImprimeFormaDePago = General.IMPRIME_FORMA_PAGO;
            cls.IncluyeDataMesero = General.IMPRIME_NOMBRE_MESERO;
            cls.IncluyeAgradecimiento = General.IMPRIME_AGRADECIMIENTO;

            cls.CierreTicket(General.EMPRESA, General.DIRECCION_LOCAL, General.RUC_EMPRESA, cierre, General.MUESTRA_ELIMINADOS_ANULADOS);
            cls.sbIpr();
            cls.Dispose();
        }

        //private void ActualizaValores()
        //{
        //    efectivo = General.StringToDouble(Efectivo.Text);
        //    tarjeta = General.StringToDouble(Tarjeta.Text);
        //    cheque = General.StringToDouble(Cheque.Text);
        //    transfer = General.StringToDouble(Transfer.Text);
        //    Cierre.Text = (efectivo + tarjeta + cheque + transfer).ToString();
        //    if (!string.IsNullOrEmpty(Cierre.Text) && !string.IsNullOrEmpty(Sistema.Text))
        //    {
        //        var cierre = General.StringToDouble(Cierre.Text);
        //        var sistema = General.StringToDouble(Sistema.Text);
        //        Diferencia.Text = String.Format("{0:0.00}", calculaDiferencia(cierre, sistema));
        //    }
        //}

        private void ActualizaValores_LostFocus(object sender, RoutedEventArgs e)
        {
            efectivo = General.StringToDouble(Efectivo.Text);
            tarjeta = General.StringToDouble(Tarjeta.Text);
            cheque = General.StringToDouble(Cheque.Text);
            transfer = General.StringToDouble(Transfer.Text);
            delivery = General.StringToDouble(Delivery.Text);
            Cierre.Text = (efectivo + tarjeta + cheque + transfer + delivery).ToString();
            if (!string.IsNullOrEmpty(Cierre.Text) && !string.IsNullOrEmpty(Sistema.Text))
            {
                var cierre = General.StringToDouble(Cierre.Text);
                var sistema = General.StringToDouble(Sistema.Text);
                Diferencia.Text = String.Format("{0:0.00}", calculaDiferencia(cierre, sistema));
            }
        }

        //private void Tarjeta_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    ActualizaValores();
        //}

        //private void Cheque_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    ActualizaValores();
        //}

        private void dtpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Busca un Inicio / Cierre de caja Previo
            var hoy = DateTime.Now;
            var fecha = string.Format("{0:yyyy-MM-dd}",dtpFecha.SelectedDate);

            if (this.tipo == "I")
                return;

            MovimientoCaja _caja = new MovimientoCaja(General.connectionString);

            CierreCaja cierre = _caja.BuscaInicioCierre(fecha, caja);

            Imprimir.Visibility = Visibility.Hidden;
            btnGrabar.IsEnabled = false;

            if (cierre != null)
            {
                this.sec = cierre.secuencial;

                Inicio.Text = string.Format("{0:0.00}", cierre.inicio);// cierre.inicio.ToString();

                if (cierre.cierre > 0)
                {
                    Efectivo.Text = string.Format("{0:0.00}", cierre.efectivo); //cierre.efectivo.ToString();
                    Tarjeta.Text = string.Format("{0:0.00}", cierre.tarjeta); //cierre.tarjeta.ToString();
                    Cheque.Text = string.Format("{0:0.00}", cierre.cheque); //cierre.cheque.ToString();
                    Delivery.Text = string.Format("{0:0.00}", cierre.delivery); //cierre.delivery.ToString();
                    Transfer.Text = string.Format("{0:0.00}", cierre.transfer); //cierre.cheque.ToString();

                    Cierre.Text = string.Format("{0:0.00}", cierre.cierre); //cierre.cierre.ToString();

                    hinicio.Text = string.Format("{0:HH:mm:ss}", cierre.horaCiere);

                    Imprimir.Visibility = Visibility.Visible;
                    
                }
                else
                {
                    Efectivo.Text = "0.00";
                    Tarjeta.Text = "0.00";
                    Transfer.Text = "0.00";
                    Cheque.Text = "0.00";
                    Delivery.Text = "0.00";

                    Cierre.Text = "0.00";

                    if (cierre.hora == null)
                    {
                        hinicio.Text = string.Format("{0:HH:mm:ss}", DateTime.Now);
                    }
                    else
                    {
                        hinicio.Text = string.Format("{0:HH:mm:ss}", cierre.hora);
                    }

                    btnGrabar.IsEnabled = true;

                    if (cierre.inicio == 0 && this.tipo == "C")
                    {
                        return;
                    }
                }
            }
            else
            {
                ObjComportamiento.showMsgInfo("No existe un inicio o cierre de caja para esta fecha");
            }

            if (DateTime.Parse(fecha) < DateTime.Parse(string.Format("{0:yyyy-MM-dd}",hoy)))
            {
                btnGrabar.IsEnabled = false;
            }
            else
            {
                btnGrabar.IsEnabled = true;
            }
        }
    }
}

using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlCreaNotaCredito.xaml
    /// </summary>
    public partial class ctlCreaNotaCredito : UserControl
    {
        public Window contenedor { get; set; }
        public ctlCarriers parent { get; set; } = null;
        public string accion { get; set; } = "N";
        public bool aprobado { get; set; } = false;
        public bool anulado { get; set; } = false;
        public string ntcrscl { get; set; }
        public string ventscl { get; set; }


        private clsTextBox clsCodVent;
        private clsTextBox clsObserv;

        public List<VentaDetalle> DetVentOrg;
        public List<vNotaCreditoDetail> ncDetVentOrg;

        private Ventas ventas = new Ventas(General.connectionString);

        public ctlCreaNotaCredito()
        {
            InitializeComponent();

            clsCodVent = new clsTextBox(txtVenta, General.USA_TECLADO_PANTALLA);
            clsObserv = new clsTextBox(txtObservacion, General.USA_TECLADO_PANTALLA);

            clsCodVent.setMaxLength(10);
            clsCodVent.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;

            clsObserv.setMaxLength(200);
            clsObserv.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS_LETRAS;
            clsObserv.permiteEspacios = true;
            clsObserv.permitePunto = true;
            clsObserv.otrosCaracteresPermitidos = "();-,#$%/[]_";

            Limpiar();

            txtVenta.Focus();
        }

        private void txtVenta_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnBuscar_Click(btnBuscar, new RoutedEventArgs());
            }
        }

        public void Mostrar()
        {
            if (accion == "M")
            {
                txtObservacion.IsEnabled = false;
                txtVenta.IsEnabled = false;
            }

            if (anulado)
            {
                btnGrabar.IsEnabled = false;
            }

            txtVenta.Text = ventscl;
            btnBuscar_Click(btnBuscar, new RoutedEventArgs());
        }

        private void Limpiar()
        {
            txtIdCliente.Text = "";
            txtCliente.Text = "";
            txtFactura.Text = "";
            txtAutorizacion.Text = "";

            txtObservacion.IsReadOnly = true;
            txtObservacion.Text = "";

            txtSubTotal.Text = string.Format("{0:0.00}", 0);
            txtIva.Text = string.Format("{0:0.00}", 0);
            txtDescuento.Text = string.Format("{0:0.00}", 0);
            txtTotal.Text = string.Format("{0:0.00}", 0);

            // Mostrando el detalle
            Detalle.ItemsSource = null;
            chkSelectAll.IsChecked = true;
        }

        public void ObtenerNotaCredito()
        {
            NotaCredito notaCredito = new NotaCredito(General.connectionString);

            vNotaCredito nt = notaCredito.getNotaCredito(ntcrscl);

            txtIdCliente.Text = nt.nidnid;
            txtCliente.Text = nt.nidnbr;
            txtFactura.Text = nt.numfact;
            txtAutorizacion.Text = nt.ventaut;

            if (!string.IsNullOrEmpty(nt.ntcraut))
            {
                btnGrabar.IsEnabled = false;
            }

            txtVenta.Text = nt.ventfac;
            txtObservacion.Text = nt.ntcrobv;
            txtSubTotal.Text = string.Format("{0:0.00}",nt.ntcrsbt);
            txtDescuento.Text = string.Format("{0:0.00}", nt.ntcrdsc);
            txtIva.Text = string.Format("{0:0.00}", nt.ntcriva);
            txtTotal.Text = string.Format("{0:0.00}", nt.ntcrtot);

            // Mostrando el detalle
            Detalle.ItemsSource = null;

            if (nt.ntcrmtv == "A")
            {
                cboMotivo.SelectedIndex = 0;
                chkSelectAll.IsChecked = true;
            }
            else
            {
                cboMotivo.SelectedIndex = 1;
                chkSelectAll.IsChecked = false;
            }

            Detalle.ItemsSource = nt.notaCreditoDetalle;

            ncDetVentOrg = new List<vNotaCreditoDetail>();

            ncDetVentOrg = (List<vNotaCreditoDetail>)nt.notaCreditoDetalle.Clone<vNotaCreditoDetail>();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";

            Limpiar();

            if (txtVenta.Text.Length == 0)
            {
                msg += "Debe ingresar el secuencial de Venta";
            }

            if (msg != "")
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            string secuencial = txtVenta.Text.PadLeft(10,'0');
            // Se verifica la existencia de la venta
            ventas.getVentaAndNotaCreditoByVentScl(secuencial);

            if (ventas.ventscl.ToString().PadLeft(10, '0') == secuencial && ventas.ntcrcrd == false && accion == "N")
            {
                if (ventas.ventStd == "P")
                {
                    // Se verifica que se haya hecho la factura electrónica y que esté autorizada
                    var factura = ventas.getFacturaElectronica(secuencial);

                    if (factura != null)
                    {
                        if (factura.ventscl == secuencial && factura.etdofac == "A")
                        {
                            txtIdCliente.Text = ventas.nidnid;
                            txtCliente.Text = ventas.nidnbr;
                            txtFactura.Text = factura.numefac;
                            txtAutorizacion.Text = factura.autosri;

                            txtObservacion.IsReadOnly = false;

                            txtSubTotal.Text = string.Format("{0:0.00}", ventas.ventsbt);
                            txtIva.Text = string.Format("{0:0.00}", ventas.ventiva);
                            txtDescuento.Text = string.Format("{0:0.00}", ventas.ventdsc);
                            txtTotal.Text = string.Format("{0:0.00}", ventas.venttot);

                            // Mostrando el detalle
                            Detalle.ItemsSource = null;
                            Detalle.ItemsSource = ventas.detVenta;

                            DetVentOrg = new List<VentaDetalle>();

                            DetVentOrg = (List<VentaDetalle>)ventas.detVenta.Clone<VentaDetalle>();

                            txtVenta.Text = secuencial;

                            cboMotivo_SelectionChanged(cboMotivo, null);

                            cboMotivo.Focus();
                        }
                        else
                        {
                            ObjComportamiento.showMsgError("La venta con secuencial " + secuencial + " no fue facturada electrónicamente, por tanto no puede generarse la nota de crédito");
                            return;
                        }
                    }
                }
                else
                {
                    errorPorEstado(ventas.ventStd, secuencial);
                    return;
                }
            }
            else if (ventas.ventscl.ToString().PadLeft(10, '0') == secuencial && ventas.ntcrcrd == true && accion == "N")
            {
                ObjComportamiento.showMsgError("Ya existe una nota de crédito para esta le venta con secuencial " + secuencial);
                return;
            }
            else if (ventas.ventscl.ToString().PadLeft(10, '0') == secuencial && ventas.ntcrcrd == true && accion != "N")
            {
                NotaCredito notaCredito = new NotaCredito(General.connectionString);


                txtIdCliente.Text = ventas.nidnid;
                txtCliente.Text = ventas.nidnbr;
                //txtFactura.Text = factura.numefac;
                //txtAutorizacion.Text = factura.autosri;

                txtObservacion.IsReadOnly = false;

                txtSubTotal.Text = string.Format("{0:0.00}", ventas.ventsbt);
                txtIva.Text = string.Format("{0:0.00}", ventas.ventiva);
                txtDescuento.Text = string.Format("{0:0.00}", ventas.ventdsc);
                txtTotal.Text = string.Format("{0:0.00}", ventas.venttot);

                // Mostrando el detalle
                Detalle.ItemsSource = null;
                Detalle.ItemsSource = ventas.detVenta;

                DetVentOrg = new List<VentaDetalle>();

                DetVentOrg = (List<VentaDetalle>)ventas.detVenta.Clone<VentaDetalle>();

                txtVenta.Text = secuencial;
            }
            else
            {
                ObjComportamiento.showMsgError("La venta con secuencial " + secuencial + " no existe");
                return;
            }
        }

        private void errorPorEstado(string estado, string secuencial)
        {
            string mensaje = "La venta con el secuencial " + secuencial + " ";

            switch (estado)
            {
                case "A":
                case "L":
                    mensaje += "fue anulada o eliminada";
                    break;
                default:
                    mensaje += "No ha sido pagada por el cliente";
                    break;
            }

            mensaje += ", por tanto no puede generarse una nota de crédito.";
            ObjComportamiento.showMsgError(mensaje);
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";

            if (txtAutorizacion.Text.Length == 0)
            {
                msg += "No se ha buscado y encontrado una venta facturada electrónicamente." + System.Environment.NewLine;
            }

            if (txtFactura.Text.Length == 0)
            {
                msg += "No existe un número de factura asociado a la venta." + System.Environment.NewLine;
            }

            if (txtObservacion.Text.Length == 0)
            {
                msg += "Debe ingresar una observación." + System.Environment.NewLine;
            }

            // Verificamos data en el Grid solo si se va a realizar una devolución
            if(cboMotivo.SelectedIndex == 1)
            {
                if (accion == "N")
                {
                    List<VentaDetalle> items = Detalle.Items
                                            .Cast<VentaDetalle>()
                                            .ToList()
                                            .Where(x => x.itemsel == true)
                                            .Select(o => new VentaDetalle
                                            {
                                                itemcnt = o.itemcnt,
                                                itemcod = o.itemcod,
                                                itemdto = o.itemdto,
                                                itemiva = o.itemiva,
                                                itemprc = o.itemprc,
                                                itempto = o.itempto,
                                                itemsel = o.itemsel,
                                                nbrprd = o.nbrprd,
                                                itemEnable = o.itemEnable,
                                                usaiva = o.usaiva
                                            })
                                            .ToList();

                    foreach (VentaDetalle detalle in items)
                    {
                        if (detalle.itemcnt == 0)
                        {
                            msg += "Existen cantidades marcadas para Devolución con valor 0." + System.Environment.NewLine;
                        }

                        int cantidadOriginal = DetVentOrg.ToList().Where(x => x.itemcod == detalle.itemcod).FirstOrDefault().itemcnt;
                        if (detalle.itemcnt > cantidadOriginal)
                        {
                            msg += $"El producto marcado para Devolución con código {detalle.itemcod} no puede tener un valor superior a {cantidadOriginal}." + System.Environment.NewLine;
                        }
                    }
                }
                else
                {
                    List<vNotaCreditoDetail> items = Detalle.Items
                                            .Cast<vNotaCreditoDetail>()
                                            .ToList()
                                            .Where(x => x.itemsel == true)
                                            .Select(o => new vNotaCreditoDetail
                                            {
                                                itemcnt = o.itemcnt,
                                                itemcod = o.itemcod,
                                                itemdto = o.itemdto,
                                                itemiva = o.itemiva,
                                                itemprc = o.itemprc,
                                                itempto = o.itempto,
                                                itemsel = o.itemsel,
                                                nbrprd = o.nbrprd,
                                                itemEnable = o.itemEnable,
                                                usaiva = o.usaiva
                                            })
                                            .ToList();

                    foreach (vNotaCreditoDetail detalle in items)
                    {
                        if (detalle.itemcnt == 0)
                        {
                            msg += "Existen cantidades marcadas para Devolución con valor 0." + System.Environment.NewLine;
                        }

                        int cantidadOriginal = ncDetVentOrg.ToList().Where(x => x.itemcod == detalle.itemcod).FirstOrDefault().itemcnt;
                        if (detalle.itemcnt > cantidadOriginal)
                        {
                            msg += $"El producto marcado para Devolución con código {detalle.itemcod} no puede tener un valor superior a {cantidadOriginal}." + System.Environment.NewLine;
                        }
                    }
                }
                
            }

            if (msg != "")
            {
                ObjComportamiento.showMsgError(msg);
                return;
            }

            if (accion == "M")
            {
                if (aprobado)
                {
                    ObjComportamiento.showMsgInfo("Esta nota de crédito no puede ser anulada, porque ya fue aprobada por el SRI");
                    return;
                }

                if (ObjComportamiento.showMsgYesNo("¿Desea continuar con la anulación de la NOTA DE CRÉDITO, una vez realizado no podrá deshacerse?") == MessageBoxResult.Yes)
                {
                    // Se procede a anular
                    NotaCredito notaCredito = new NotaCredito(General.connectionString);

                    Respuesta respuesta = notaCredito.AnularNotaCredito(ntcrscl);

                    ObjComportamiento.showMuestraMensaje(respuesta);
                }
            }
            else
            {
                // Se compara si la data a devolver es igual a la data original
                // de ser iguales se presentará el mensaje de cambio a Anulación
                string msgContinue = "";
                bool paraAnulacion = false;
                if (Detalle.Items[0].GetType() == typeof(VentaDetalle))
                {
                    var actual = Detalle.Items.Cast<VentaDetalle>().ToList<VentaDetalle>();
                    var org = DetVentOrg;

                    msgContinue = "¿Desea continuar con la creación de la NOTA DE CRÉDITO?";

                    if (comparaVentas(org, actual) && cboMotivo.SelectedIndex == 1)
                    {
                        msgContinue = "Los Item seleccionados para Devolución no han sufrido cambio, " +
                                      "por tanto será tratado como una Anulación." + Environment.NewLine + msgContinue;
                        paraAnulacion = true;
                    }
                }

                if (Detalle.Items[0].GetType() == typeof(vNotaCreditoDetail))
                {
                    var actual = Detalle.Items.Cast<vNotaCreditoDetail>().ToList<vNotaCreditoDetail>();
                    var org = ncDetVentOrg;

                    msgContinue = "¿Desea continuar con la modificación de la NOTA DE CRÉDITO?";
                }

                if (ObjComportamiento.showMsgYesNo(msgContinue) == MessageBoxResult.Yes)
                {
                    // se procede a grabar
                    NotaCredito notaCredito = new NotaCredito(General.connectionString);

                    notaCredito.ntcraut = txtAutorizacion.Text;
                    notaCredito.ventfac = txtFactura.Text;
                    notaCredito.ntcrscl = txtVenta.Text;
                    notaCredito.ntcrusu = General.USUARIO;
                    notaCredito.usrnid = General.CODUSUARIO;
                    notaCredito.cajacod = General.CAJA;
                    if (paraAnulacion)
                    {
                        notaCredito.ntcrmtv = "A";
                    }
                    else
                    {
                        notaCredito.ntcrmtv = cboMotivo.SelectedIndex == 0 ? "A" : "D";
                    }

                    notaCredito.ntcrsbt = General.StringToDouble(txtSubTotal.Text);
                    notaCredito.ntcrdsc = General.StringToDouble(txtDescuento.Text);
                    notaCredito.ntcriva = General.StringToDouble(txtIva.Text);
                    notaCredito.ntcrtot = General.StringToDouble(txtTotal.Text);
                    notaCredito.ntcrobv = txtObservacion.Text;

                    notaCredito.notaCreditoDetalle = new List<NotaCreditoDetail>();
                    foreach (var detalle in Detalle.Items)
                    {
                        if (detalle.GetType() == typeof(VentaDetalle))
                        {
                            NotaCreditoDetail ncd = new NotaCreditoDetail
                            {
                                itemcnt = ((VentaDetalle)detalle).itemcnt,
                                itemcod = ((VentaDetalle)detalle).itemcod,
                                itemiva = ((VentaDetalle)detalle).usaiva,
                                itempiv = ((VentaDetalle)detalle).itemiva,
                                itemprc = ((VentaDetalle)detalle).itemprc,
                                itempto = ((VentaDetalle)detalle).itempto,
                                ntcrscl = notaCredito.ntcrscl
                            };

                            if (((VentaDetalle)detalle).itemsel)
                            {
                                notaCredito.notaCreditoDetalle.Add(ncd);
                            }
                        }

                        if (detalle.GetType() == typeof(vNotaCreditoDetail))
                        {
                            NotaCreditoDetail ncd = new NotaCreditoDetail
                            {
                                itemcnt = ((vNotaCreditoDetail)detalle).itemcnt,
                                itemcod = ((vNotaCreditoDetail)detalle).itemcod,
                                itemiva = ((vNotaCreditoDetail)detalle).usaiva,
                                itempiv = ((vNotaCreditoDetail)detalle).itemiva,
                                itemprc = ((vNotaCreditoDetail)detalle).itemprc,
                                itempto = ((vNotaCreditoDetail)detalle).itempto,
                                ntcrscl = notaCredito.ntcrscl
                            };

                            if (((vNotaCreditoDetail)detalle).itemsel)
                            {
                                notaCredito.notaCreditoDetalle.Add(ncd);
                            }
                        }
                    }
                    
                    Respuesta respuesta = notaCredito.grabar(accion);

                    ObjComportamiento.showMuestraMensaje(respuesta);

                    if (respuesta.resultado)
                    {
                        // Se debe enviar a procesar al SRI
                        if (General.ENVIO_AUTO_AL_SRI)
                        {
                            EjecutarEnvioSRI(respuesta, notaCredito);
                        }

                        Limpiar();
                        txtVenta.Text = "";
                    }
                }
            }
        }

        private void EjecutarEnvioSRI(dynamic respuesta, NotaCredito notaCredito)
        {
            string mensajeOriginal = respuesta.mensaje;
            string mensajeLimpio = mensajeOriginal;

            if (!string.IsNullOrEmpty(mensajeOriginal) && mensajeOriginal.EndsWith("."))
            {
                mensajeLimpio = mensajeOriginal[..^1];
            }

            notaCredito.ntcrscl = mensajeLimpio.Length >= 10 ? mensajeLimpio[^10..] : mensajeLimpio;

            string codigoNotaCredito = notaCredito.ntcrscl.PadLeft(10, '0');
            mFacturasSRI data = notaCredito.getNotaCreditoToSRIByNtcrScl(codigoNotaCredito);
            data.docutip = "C";

            winGeneral win = new winGeneral
            {
                Width = 500,
                Height = 300,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            ctlProgressFact prgFact = new ctlProgressFact();
            win.Contenido.Children.Add(prgFact);

            RoutedEventHandler loadedHandler = null!;
            loadedHandler = (sender, e) =>
            {
                win.Loaded -= loadedHandler;
                prgFact.Iniciar(data);
            };
            win.Loaded += loadedHandler;

            win.ShowDialog();
        }

        private bool comparaVentas(List<VentaDetalle> original, List<VentaDetalle> nueva)
        {
            bool iguales = true;

            for(int i = 0; i< original.Count; i++)
            {
                if (original[i].itemcnt != nueva[i].itemcnt)
                {
                    iguales = false;
                }
            }

            return iguales;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void cboMotivo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Detalle != null && cboMotivo != null)
            {
                if (cboMotivo.SelectedIndex == 0)
                {
                    Detalle.Columns[2].IsReadOnly = true;
                    chkSelectAll.IsEnabled = false;
                    ActivaCheckBox(false);
                }
                else
                {
                    Detalle.Columns[2].IsReadOnly = false;
                    chkSelectAll.IsEnabled = true;
                    ActivaCheckBox(true);
                }
            }
            
        }

        private void ActivaCheckBox(bool value)
        {
            foreach (var fact in Detalle.Items)
            {
                if (fact.GetType() == typeof(VentaDetalle))
                {
                    ((VentaDetalle)fact).itemEnable = value;
                }

                if (fact.GetType() == typeof(vNotaCreditoDetail))
                {
                    ((vNotaCreditoDetail)fact).itemEnable = value;
                }
            }
            Detalle.Items.Refresh();
        }

        private void SeleccionaCheckBox(bool value)
        {
            if (Detalle.Items != null && Detalle.Items.Count > 0)
            {
                if (Detalle.Items[0].GetType() == typeof(VentaDetalle))
                {
                    foreach (VentaDetalle fact in Detalle.Items)
                    {
                        fact.itemsel = value;
                    }
                }

                if (Detalle.Items[0].GetType() == typeof(vNotaCreditoDetail))
                {
                    foreach (vNotaCreditoDetail fact in Detalle.Items)
                    {
                        fact.itemsel = value;
                    }
                }
                
                Detalle.Items.Refresh();
            }
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            SeleccionaCheckBox(true);
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            SeleccionaCheckBox(false);
        }

        private void selItemVenta(object sender, bool valor)
        {
            if (sender != null)
            {
                var cod = ((CheckBox)sender).Tag;

                try
                {
                    var objVenta = Detalle.Items
                                    .Cast<VentaDetalle>()
                                    .ToList()
                                    .FirstOrDefault(x => x.itemcod == cod);

                    objVenta.itemsel = valor;
                }
                catch (Exception e)
                {
                    var objVenta = Detalle.Items
                                    .Cast<vNotaCreditoDetail>()
                                    .ToList()
                                    .FirstOrDefault(x => x.itemcod == cod);

                    objVenta.itemsel = valor;
                }
                
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            selItemVenta(sender, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            selItemVenta(sender, false);
        }

        private void Detalle_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement.GetType() == typeof(TextBox))
            {
                string ValorNuevo = ((TextBox)e.EditingElement).Text;
                
                _ = int.TryParse(ValorNuevo, out int valor);

                var item = Detalle.CurrentItem;

                if (item.GetType() == typeof(VentaDetalle))
                {
                    VentaDetalle vta = (VentaDetalle)item;
                    if (vta != null)
                    {
                        vta.itempto = valor * vta.itemprc + valor * vta.itemiva;
                    }
                }

                if (item.GetType() == typeof(vNotaCreditoDetail))
                {
                    vNotaCreditoDetail vta = (vNotaCreditoDetail)item;
                    if (vta != null)
                    {
                        vta.itempto = valor * vta.itemprc + valor * vta.itemiva;
                    }
                }
            }
        }

        private void Detalle_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Detalle.Items.Refresh();

                foreach(VentaDetalle vta in Detalle.Items.Cast<VentaDetalle>().ToList())
                {
                    int valor = vta.itemcnt;
                    vta.itempto = valor * vta.itemprc + valor * vta.itemiva;
                }

                Detalle.Items.Refresh();

                // Actualizar subtotales, iva y totales
                var items = (List<VentaDetalle>)Detalle.Items.Cast<VentaDetalle>().ToList<VentaDetalle>();

                var subtotal = items.Where(x => x.itemsel == true).Sum(x => x.itemcnt * x.itemprc);
                var iva = items.Where(x => x.itemsel == true).Sum(x => x.itemcnt * x.itemiva);
                var descuento = items.Where(x => x.itemsel == true).Sum(x => x.itemcnt * x.itemdto);

                // Hay que incluir el descuento en el cálculo del total
                var total = subtotal + iva;

                txtSubTotal.Text = string.Format("{0:0.00}", subtotal);
                txtIva.Text = string.Format("{0:0.00}", iva);
                txtDescuento.Text = string.Format("{0:0.00}", descuento);
                txtTotal.Text = string.Format("{0:0.00}", total);
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}

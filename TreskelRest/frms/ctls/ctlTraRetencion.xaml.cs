using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlTraRetencion.xaml
    /// </summary>
    public partial class ctlTraRetencion : UserControl, IDisposable
    {
        private clsTextBox clsEje;
        private clsTextBox clsBase;
        private clsTextBox clsNumComp;

        private NumKeyboard nKeyboard = new NumKeyboard();
        private bool disposedValue;

        private Retencion clsRetencion = new Retencion(General.connectionString);

        private mCompra sCompra = new mCompra();
        private mVenta sVenta = new mVenta();

        public ctlTraRetencion()
        {
            InitializeComponent();

            clsEje = new clsTextBox(txtEjerFisc, General.USA_TECLADO_PANTALLA)
            {
                caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS,
                otrosCaracteresPermitidos = "/"
            };
            clsEje.setMaxLength(7);

            clsBase = new clsTextBox(txtBaseImpo, General.USA_TECLADO_PANTALLA)
            {
                caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.DECIMALES
            };
            clsBase.setMaxLength(10);

            clsNumComp = new clsTextBox(txtNumComp, General.USA_TECLADO_PANTALLA)
            {
                caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS,
                otrosCaracteresPermitidos = "-"
            };
            clsNumComp.setMaxLength(20);

            llenaComboPorcentajes();
            RetencionContainer.IsEnabled = false;
            Limpiar();

            clsRetencion.reteCaj = General.CAJA;

            nKeyboard.retornaTexto += TextoNumerico_Retorna;

            FechaEmision.SelectedDate = DateTime.Today;
        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                ObjComportamiento.showMsgError("Debe ingresar una clave para poder realizar el pedido");
                btnCajero.IsChecked = false;
                return;
            }

            using (Usuarios usuario = new Usuarios(General.connectionString))
            {
                string resultadoConsulta = usuario.getCodMesero(texto, General.USA_CLAVE_NUMERICA);

                if (string.IsNullOrEmpty(resultadoConsulta))
                {
                    ObjComportamiento.showMsgError("Clave ingresada no válida");
                    return;
                }

                string[] rcodigo = resultadoConsulta.Split(';');
                if (rcodigo.Length < 3)
                {
                    ObjComportamiento.showMsgError("Formato de datos de usuario incorrecto.");
                    return;
                }

                string codigo = rcodigo[0];
                string cellphone = rcodigo[1];
                string nombre = rcodigo[2];

                if (string.IsNullOrEmpty(codigo))
                {
                    ObjComportamiento.showMsgError("Clave ingresada incorrecta");
                    btnCajero.IsChecked = false;
                    return;
                }

                List<mPermiso> permisos = usuario.getPermisoUsuario(codigo);

                if (permisos.Count == 0)
                {
                    ObjComportamiento.showMsgError("Clave ingresada no válida");
                    return;
                }

                UsrEmpleado usr = usuario.getEmpleadoMesero(codigo);

                Mesero.Content = usr?.usrusr?.ToLower() ?? string.Empty;
                clsRetencion.reteUsr = codigo;

                ActivaElementosCajero(true);
            }
        }

        public void ActivaElementosCajero(Boolean valor)
        {
            RetencionContainer.IsEnabled = valor;
            btnGrabar.IsEnabled = true;
        }

        private void Limpiar()
        {
            var fecha = DateTime.Now;
            var anio = fecha.Year;
            var mes = fecha.Month;

            txtEjerFisc.Text = mes.ToString().PadLeft(2,'0') + "/" + anio.ToString();

            txtTotal.Text = string.Format("{0:0.00}", 0);
            txtValor.Text = string.Format("{0:0.00}", 0);
            txtNumComp.Text = "";
            txtCedula.Text = "";
            txtCliente.Text = "";
            btnGrabar.IsEnabled = false;

            dgvRetc.Items.Clear();
            LimpiarDetalle();
        }

        private void LimpiarDetalle()
        {
            txtBaseImpo.Text = "0.00";
            txtValor.Text = "0.00";
            cboTipoRet.SelectedIndex = 0;
            llenaComboPorcentajes();
            cboPctRet.SelectedIndex = 0;
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            txtCedula.Text = "";
            txtCliente.Text = "";

            clsRetencion.reteCli = "";
            clsRetencion.reteFch = DateTime.Now;
            clsRetencion.reteCTip = "C";
            clsRetencion.reteTip = "F";
            clsRetencion.reteScl = "";
            clsRetencion.docuSri = "";
            clsRetencion.docuNAu = "";
            clsRetencion.reteNum = "";
            clsRetencion.reteDet.Clear();
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            if (((ComboBoxItem)cboConcepto.SelectedItem).Tag.ToString() == "C")
            {
                // Retenciones en compras
                Proveedor proveedor = new Proveedor(General.connectionString);
                proveedor.provNid = txtCedula.Text.ToString();

                if (proveedor.getById())
                {
                    txtCliente.Text = proveedor.provNbr;
                }
                else
                {
                    ObjComportamiento.showMsgError("Proveedor no existe");
                }
            }
            else
            {
                // Retenciones en ventas
                Clientes cliente = new Clientes(General.connectionString);

                cliente.nidNid = txtCedula.Text.ToString();
                if (cliente.getById())
                {
                    txtCliente.Text = cliente.nidNbr;

                    clsRetencion.reteCli = txtCedula.Text;
                    clsRetencion.reteCTip = cliente.nidTip;
                }
                else
                {
                    ObjComportamiento.showMsgError("Cliente no existe, verifique la información o vaya a la opción clientes y créelo.");
                }
            }
        }

        private void btnCajero_Checked(object sender, RoutedEventArgs e)
        {
            nKeyboard.MaxLength = General.CANTIDAD_CARACTERES_PASSWORD;

            nKeyboard.setTextBox = new PasswordBox();

            nKeyboard.ShowDialog();
        }

        private void btnCajero_Unchecked(object sender, RoutedEventArgs e)
        {
            nKeyboard.Hide();
            ActivaElementosCajero(false);
            Mesero.Content = "";
        }

        private void btnDelFact_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAnlFact_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(txtCedula.Text, "[0-9]{13}"))
            {
                ObjComportamiento.showMsgError("- El Ruc debe tener 13 caracteres.");
                return;
            }

            if (!Regex.IsMatch(txtEjerFisc.Text, "[0-1][0-9][/][2-9][0-9][0-9][0-9]"))
            {
                ObjComportamiento.showMsgError("El periodo fiscal no cumple con el siguiente patrón:" + System.Environment.NewLine +
                    "- Los 2 primeros números representan un mes, deben ir desde 01 al 12" + System.Environment.NewLine +
                    "- Los 4 últimos números representan un año, el Sistema acepta valores superiores al 2000");
                return;
            }

            if (!Regex.IsMatch(txtNumComp.Text, "[0-9]{3}[-][0-9]{3}[-][0-9]{9}"))
            {
                ObjComportamiento.showMsgError("- El número de comprobante debe tener el siguiente formato ###-###-#########");
                return;
            }

            var emision = FechaEmision.SelectedDate ?? DateTime.Today;
            string[] fiscal = txtEjerFisc.Text.Split('/');

            int anioFiscal = int.Parse(fiscal[0]);
            int mesFiscal = int.Parse(fiscal[1]);

            if (anioFiscal > emision.Year || (anioFiscal == emision.Year && mesFiscal > emision.Month))
            {
                ObjComportamiento.showMsgError("El período fiscal no puede ser mayor al de la fecha de emisión.");
                return;
            }

            string msgError = "";

            if (txtNumComp.Text.Split('-').Length != 3)
            {
                msgError += $"El número de comprobante no tiene el formato adecuado, revise.{Environment.NewLine}";
            }

            if (dgvRetc.Items.Count == 0)
            {
                msgError += $"- No existen Retenciones a procesar.{Environment.NewLine}";
            }

            if (!string.IsNullOrEmpty(msgError))
            {
                ObjComportamiento.showMsgError(msgError);
                return;
            }

            string concepto = (cboConcepto.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;
            string? ventSclId = null;

            if (concepto == "C")
            {
                using (Compra compra = new Compra(General.connectionString))
                {
                    mCompra mcompra = compra.getCompra(txtCedula.Text, txtNumComp.Text);

                    if (General.VALIDA_COMPRA_RETENCION)
                    {
                        if (mcompra == null || mcompra.compscl == null)
                        {
                            ObjComportamiento.showMsgError($"La compra con número de factura {txtNumComp.Text} para el Proveedor con Ruc {txtCedula.Text}{Environment.NewLine}no ha sido registrada, regístrela antes de realizar la Retención");
                            return;
                        }
                    }
                    ventSclId = mcompra?.compscl;
                }
            }
            else
            {
                using (Ventas venta = new Ventas(General.connectionString))
                {
                    mVenta mventa = venta.getVenta(txtCedula.Text, txtNumComp.Text);

                    if (mventa == null)
                    {
                        ObjComportamiento.showMsgError($"La venta no ha sido registrada mediante el proceso de facturación electrónica{Environment.NewLine}");
                        return;
                    }
                    ventSclId = mventa.ventscl;
                }
            }

            if (ObjComportamiento.showMsgYesNo("¿Desea grabar la Retención?") != MessageBoxResult.Yes) return;

            clsRetencion.ventScl = ventSclId;
            clsRetencion.reteTip = concepto;
            clsRetencion.reteDTp = (cboTipoComp.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;
            clsRetencion.docuSri = txtNumComp.Text;
            clsRetencion.docuNAu = string.Empty;
            clsRetencion.reteFch = emision;
            clsRetencion.reteTot = General.StringToDouble(txtTotal.Text);
            clsRetencion.reteCli = txtCedula.Text;
            clsRetencion.reteEtd = "A";

            clsRetencion.reteDet.Clear();
            clsRetencion.reteDet.AddRange(dgvRetc.Items.Cast<RetencionDetail>());


            var anioEmision =emision.Year;
            var mesEmision = emision.Month;

            var respuesta = clsRetencion.grabar("N");
            ObjComportamiento.showMuestraMensaje(respuesta);

            if (respuesta?.resultado == true)
            {
                if (concepto == "C" && General.ENVIO_AUTO_AL_SRI)
                {
                    EjecutarEnvioSRI(respuesta);
                }

                Limpiar();
            }
        }

        private void EjecutarEnvioSRI(dynamic respuesta)
        {
            string mensajeOriginal = respuesta.mensaje;
            string mensajeLimpio = mensajeOriginal;

            if (!string.IsNullOrEmpty(mensajeOriginal) && mensajeOriginal.EndsWith("."))
            {
                mensajeLimpio = mensajeOriginal[..^1];
            }
            
            clsRetencion.reteScl = mensajeLimpio.Length >= 10 ? mensajeLimpio[^10..] : mensajeLimpio;
            
            string codigoRetencion = clsRetencion.reteScl.PadLeft(10, '0');
            mFacturasSRI data = clsRetencion.getRetencionesToSRIByReteScl(codigoRetencion);
            data.docutip = "R";

            winGeneral win = new winGeneral
            {
                Width = 500,
                Height = 300,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            ctlProgressFact prgFact = new ctlProgressFact { 
                contenedor = win
            };

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

        /*
        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string msgError = "";

            if (!Regex.IsMatch(txtCedula.Text, "[0-9]{13}"))
            {
                ObjComportamiento.showMsgError("- El Ruc debe tener 13 caracteres.");
                return;
            }

            if (!Regex.IsMatch(txtEjerFisc.Text, "[0-1][0-9][/][2-9][0-9][0-9][0-9]"))
            {
                ObjComportamiento.showMsgError("El periodo fiscal no cumple con el siguiente patrón:" + System.Environment.NewLine +
                    "- Los 2 primeros números representan un mes, deben ir desde 01 al 12" + System.Environment.NewLine +
                    "- Los 4 últimos números representan un año, el Sistema acepta valores superiores al 2000");
                return;
            }

            if (!Regex.IsMatch(txtNumComp.Text, "[0-9]{3}[-][0-9]{3}[-][0-9]{9}"))
            {
                ObjComportamiento.showMsgError("- El número de comprobante debe tener el siguiente formato ###-###-#########");
                return;
            }

            var emision = ((DateTime)FechaEmision.SelectedDate);

            var anioEmision =emision.Year;
            var mesEmision = emision.Month;

            var fiscal = txtEjerFisc.Text.Split('/');
            var anioFiscal = int.Parse(fiscal[0]);
            var mesFiscal = int.Parse(fiscal[1]);

            if (anioFiscal > anioEmision)
            {
                ObjComportamiento.showMsgError("El año fiscal no puede ser mayor al de la fecha de emisión");
            }

            if (anioFiscal == anioEmision)
            {
                if (mesFiscal > mesEmision)
                {
                    ObjComportamiento.showMsgError("El año fiscal no puede ser mayor al de la fecha de emisión (Mes Fiscal mayor al de Emisión");
                }
            }

            var numFactura = txtNumComp.Text.Split('-');

            if (numFactura.Length != 3)
            {
                msgError += "El número de comprobante no tiene el formato adecuado, revise.";
            }

            if (dgvRetc.Items.Count == 0)
            {
                msgError += "- No existen Retenciones a procesar." + System.Environment.NewLine;
            }

            if (msgError != "")
            {
                ObjComportamiento.showMsgError(msgError);
                return;
            }

            // Buscar el secuencial de compra/venta de acuerdo con el tipo y el proveedor/cliente
            if (((ComboBoxItem)cboConcepto.SelectedItem).Tag.ToString() == "C")
            {
                Compra compra = new Compra(General.connectionString);
                mCompra mcompra = null;

                if (General.VALIDA_COMPRA_RETENCION)
                {
                    mcompra = compra.getCompra(txtCedula.Text, txtNumComp.Text);

                    if (mcompra.compscl == null)
                    {
                        msgError += "La compra con número de factura " + txtNumComp.Text + " para el Proveedor con Ruc " + txtCedula.Text + System.Environment.NewLine;
                        msgError += "no ha sido registrada, regístrela antes de realizar la Reteción";
                    }

                    if (msgError != "")
                    {
                        ObjComportamiento.showMsgError(msgError);
                        return;
                    }
                }

                // Proceso de grabado de la retención
                if (ObjComportamiento.showMsgYesNo("¿Desea grabar la Retención en compra?") == MessageBoxResult.Yes)
                {
                    // Realiza el proceso de grabado
                    clsRetencion.ventScl = mcompra != null ? mcompra.compscl : null;
                    clsRetencion.reteTip = ((ComboBoxItem)cboConcepto.SelectedItem).Tag.ToString();
                    clsRetencion.reteDTp = ((ComboBoxItem)cboTipoComp.SelectedItem).Tag.ToString();
                    clsRetencion.docuSri = txtNumComp.Text;
                    clsRetencion.docuNAu = "";//txtNumAut.Text;
                    clsRetencion.reteFch = (DateTime)FechaEmision.SelectedDate;
                    clsRetencion.reteTot = General.StringToDouble(txtTotal.Text);
                    clsRetencion.reteCli = txtCedula.Text;
                    clsRetencion.reteEtd = "A";

                    foreach (var detalle in dgvRetc.Items)
                    {
                        clsRetencion.reteDet.Add((RetencionDetail)detalle);
                    }

                    var respuesta = clsRetencion.grabar("N");

                    ObjComportamiento.showMuestraMensaje(respuesta);

                    if (respuesta.resultado)
                    {
                        if (General.ENVIO_AUTO_AL_SRI)
                        {
                            respuesta.mensaje = string.IsNullOrEmpty(respuesta.mensaje) ? respuesta.mensaje : respuesta.mensaje[..^1];
                            string textoBase = respuesta.mensaje ?? string.Empty;
                            clsRetencion.ventScl = textoBase.Length >= 10 ? textoBase[^10..] : textoBase;

                            string codigoRetencion = clsRetencion.ventScl.PadLeft(10, '0');
                            mFacturasSRI data = clsRetencion.getRetencionesToSRIByReteScl(codigoRetencion);

                            respuesta.mensaje = respuesta.mensaje[..^1];
                            clsRetencion.ventScl = respuesta.mensaje.Length >= 10 ? respuesta.mensaje[^10..] : respuesta.mensaje;

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

                            RoutedEventHandler loadedHandler = null;
                            loadedHandler = (sender, e) =>
                            {
                                win.Loaded -= loadedHandler;
                                prgFact.Iniciar(data);
                            };
                            win.Loaded += loadedHandler;

                            win.ShowDialog();
                        }

                        Limpiar();
                    }
                }
            }
            else
            {
                // En caso de ser una retención por venta
                Ventas venta = new Ventas(General.connectionString);
                mVenta mventa = venta.getVenta(txtCedula.Text, txtNumComp.Text);

                if (mventa != null)
                {
                    if (ObjComportamiento.showMsgYesNo("¿Desea grabar la Retención en compra?") == MessageBoxResult.Yes)
                    {
                        clsRetencion.ventScl = mventa.ventscl;
                        clsRetencion.reteTip = ((ComboBoxItem)cboConcepto.SelectedItem).Tag.ToString();
                        clsRetencion.reteDTp = ((ComboBoxItem)cboTipoComp.SelectedItem).Tag.ToString();
                        clsRetencion.docuSri = txtNumComp.Text;
                        clsRetencion.docuNAu = ""; 
                        clsRetencion.reteFch = (DateTime)FechaEmision.SelectedDate;
                        clsRetencion.reteTot = General.StringToDouble(txtTotal.Text);
                        clsRetencion.reteCli = txtCedula.Text;
                        clsRetencion.reteEtd = "A";

                        foreach (var detalle in dgvRetc.Items)
                        {
                            clsRetencion.reteDet.Add((RetencionDetail)detalle);
                        }

                        var respuesta = clsRetencion.grabar("N");

                        ObjComportamiento.showMuestraMensaje(respuesta);

                        if (respuesta.resultado)
                        {
                            Limpiar();
                        }
                    }
                }
                else
                {
                    msgError += "La venta no ha sido registrada mediante el proceso de facturación electrónica" + System.Environment.NewLine;
                }
            }

            if (msgError != "")
            {
                ObjComportamiento.showMsgError(msgError);
                return;
            }
        }
         */

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(!Regex.IsMatch(txtEjerFisc.Text, "[0-1][0-9][/][2-9][0-9][0-9][0-9]"))
            {
                ObjComportamiento.showMsgError("El periodo fiscal no cumple con el siguiente patrón:" + System.Environment.NewLine +
                    "- Los 2 primeros números representan un mes, deben ir desde 01 al 12" + System.Environment.NewLine +
                    "- Los 4 últimos números representan un año, el Sistema acepta valores superiores al 2000");
                return;
            }

            if (!Regex.IsMatch(txtNumComp.Text, "[0-9]{3}[-][0-9]{3}[-][0-9]{9}"))
            {
                ObjComportamiento.showMsgError("El número de comprobante debe tener el siguiente formato ###-###-#########");
                return;
            }

            if (txtBaseImpo.Text != "")
            {
                if (decimal.Parse(txtBaseImpo.Text) == 0)
                {
                    ObjComportamiento.showMsgError("La base imponible no puede ser cero");
                    return;
                }
            }

            var porcentaje = (mRetencion)((ComboBoxItem)(cboPctRet.SelectedItem)).Tag;

            RetencionDetail detalle = new RetencionDetail();

            string tipoImpo = ((ComboBoxItem)cboTipoRet.SelectedItem).Tag.ToString();
            detalle.aniofisc = txtEjerFisc.Text;
            detalle.baseimpo = Math.Round(General.StringToDouble(txtBaseImpo.Text), 2);
            detalle.tipoimpo = tipoImpo == "I" ? "IVA" : "Renta";
            detalle.pctrete = porcentaje.valor;
            detalle.valorrete = Math.Round(General.StringToDouble(txtValor.Text), 2);
            detalle.codsri = porcentaje.codsri;
            detalle.codigo = porcentaje.codigo;

            dgvRetc.Items.Add(detalle);

            CalcularTotal();
            LimpiarDetalle();
        }

        private void CalcularTotal()
        {
            double total = dgvRetc.Items
                .OfType<RetencionDetail>()
                .Sum(x => x.valorrete);

            txtTotal.Text = $"{total:0.00}";
        }

        private void btnDelItem_Click(object sender, RoutedEventArgs e)
        {
            RetencionDetail row = (RetencionDetail)((FrameworkElement)sender).DataContext;

            dgvRetc.Items.Remove(row);

            CalcularTotal();
        }

        private void cboTipoRet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            llenaComboPorcentajes();
        }

        private void llenaComboPorcentajes()
        {
            if (cboPctRet == null || cboTipoRet == null) return;

            string tipo = (cboTipoRet.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(tipo)) return;

            var porcentajes = loadPorcentajes(tipo);

            cboPctRet.Items.Clear();

            for (int i = 0; i < porcentajes.Count; i++)
            {
                var porcentaje = porcentajes[i];

                ComboBoxItem cboi = new ComboBoxItem
                {
                    Tag = porcentaje,
                    Content = porcentaje.descripcion,
                    Height = 40,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    IsSelected = (i == 0)
                };

                cboPctRet.Items.Add(cboi);
            }
        }

        private List<mRetencion> loadPorcentajes(string tipo)
        {
            PctRetencion retencion = new PctRetencion(General.connectionString);

            return retencion.GetByType(tipo);
        }

        private void CalculaValor()
        {
            try
            {
                if ((ComboBoxItem)cboPctRet.SelectedItem != null)
                {
                    var porcentaje = (mRetencion)((ComboBoxItem)cboPctRet.SelectedItem).Tag;
                    txtValor.Text = string.Format("{0:0.00}", General.StringToDouble(txtBaseImpo.Text) * porcentaje.valor / 100);
                }
            }
            catch (Exception)
            {

            }
        }

        private void cboPctRet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculaValor();    
        }

        private void txtEjerFisc_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void txtBaseImpo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEjerFisc.Text != "")
            {
                CalculaValor();
            }
        }

        private void btnBscFac_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumComp.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                ObjComportamiento.showMsgError("Debe ingresar el número de comprobante y el identificador del proveedor/cliente.");
                return;
            }

            string concepto = (cboConcepto.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;

            if (concepto == "C")
            {
                using (Compra compra = new Compra(General.connectionString))
                {
                    compra.compscl = txtNumComp.Text;
                    mCompra mcompra = compra.getCompra(txtCedula.Text, txtNumComp.Text);

                    if (mcompra?.compscl != null)
                    {
                        sCompra = mcompra;
                        ObjComportamiento.showMsgInfo("La compra fue encontrada, puede ingresar los valores respectivos.");
                        return;
                    }
                }

                ObjComportamiento.showMsgError("La compra no fue encontrada.");
            }
            else
            {
                using (Ventas venta = new Ventas(General.connectionString))
                {
                    mVenta mventa = venta.getVenta(txtCedula.Text, txtNumComp.Text);

                    if (mventa != null)
                    {
                        sVenta = mventa;
                        ObjComportamiento.showMsgInfo("La venta fue encontrada, puede ingresar los valores respectivos.");
                        return;
                    }
                }

                ObjComportamiento.showMsgError("La venta no fue encontrada.");
            }
        }

        private void btnBscFacCod_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSetValores_Click(object sender, RoutedEventArgs e)
        {
            string tipoRetencion = (cboTipoRet.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;
            string concepto = (cboConcepto.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? string.Empty;

            double baseIva = (concepto == "C") ? (sCompra?.iva ?? 0) : (sVenta?.ventiva ?? 0);
            double baseRenta = (concepto == "C") ? (sCompra?.subtot ?? 0) : (sVenta?.ventsbt ?? 0);

            double valorBase = tipoRetencion switch
            {
                "I" => baseIva,
                "R" => baseRenta,
                _ => 0
            };

            txtBaseImpo.Text = $"{valorBase:0.00}";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~ctlTraRetencion()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

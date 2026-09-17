using Entidades;
using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;
using LibPrint.modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LibPrint
{
    public class EposPrint : IDisposable
    {
        private Printer printer = null;
        public Boolean printCajaIzq { get; set; } = false;
        public int cantidadLineas = 3;
        public string tipo = "";
        public bool impresionOriginal = true;
        private bool IsOffLine = false;

        private string ESC = Convert.ToString((char)27);
        private string LF = Convert.ToString((char)10);
        private string FONT = Convert.ToString((char)77);
        private string B11 = Convert.ToString(1);


        /*
         Parámetros para configuración de la impresión
         */
        public char DefineSeparador { get; set; } = '-';
        public int cantidadColumnas { get; set; } = 48;
        public bool ImprimeNombreEmpresa { get; set; } = true;
        public bool ImprimeDireccionEmpresa { get; set; } = true;
        public bool ImprimeRucEmpresa { get; set; } = true;
        public bool CabeceraNegrita { get; set; } = false;
        public string AlineacionCabecera { get; set; } = "C";
        public string TipoImpresora { get; set; } = "I";
        public bool ImprimeSeparadorEndCabecera { get; set; } = true;

        public bool ImprimeNegritaCabeceraTabla { get; set; } = false;
        public int CantidadLineasSaltarEndTabla { get; set; }
        public bool ImprimeSeparadorTablas { get; set; } = true;
        public int CantidadDecimalesItems { get; set; } = 4;
        public bool ImprimeFormaDePago { get; set; } = true;
        public bool IncluyeDataMesero { get; set; } = true;
        public bool IncluyeAgradecimiento { get; set; } = true;
        public bool ImprimeCabeceraComandas { get; set; } = true;
        public bool ImprimeClienteComanda { get; set; } = true;
        public bool SaltoLineaCabeceraComandas { get; set; } = false;
        public string UrlDescarga { get; set; } = "";
        public string imprimeOpcionDescagar { get; set; } = "";
        public bool usaEspaciosInterlineado { get; set; } = false;
        public bool usaCabeceraExpandida { get; set; } = false;
        public bool imprimeSecTemporal { get; set; } = false;
        public bool imprimeClaveDownload { get; set; } = false;

        public EposPrint()
        {
            
        }

        public void TestImpresion(string texto)
        {
            printer.Append("MODO DE 48 COLUMNAS");
            printer.Append("1...5...10...15...20...25...30...35...40...45.48");
            printer.Separator();
            printer.Append(texto);
            printer.BoldMode(texto);
            printer.UnderlineMode(texto);
            printer.Separator();
            printer.ExpandedMode(PrinterModeState.On);
            printer.Append("MODO EXP DE 23 COLUMNAS");
            printer.Append("1...5...10...15...20..23");
            printer.ExpandedMode(PrinterModeState.Off);
            printer.Separator();
            printer.CondensedMode(PrinterModeState.On);
            printer.Append("MODO COND DE 64 COLUMNAS");
            printer.Append("1...5...10...15...20...25...30...35...40...45...50...55...60..64");
            printer.CondensedMode(PrinterModeState.Off);
            printer.Separator();
            printer.DoubleWidth2();
            printer.Append("Font con ancho 2");
            printer.DoubleWidth3();
            printer.Append("Font con ancho 3");
            printer.NormalWidth();
            printer.Append("Font con ancho normal");
            printer.Separator();
            printer.AlignLeft();
            printer.Append(texto);
            printer.AlignRight();
            printer.Append(texto);
            printer.AlignCenter();
            printer.Append(texto);
            printer.Separator();
            printer.Font(texto, Fonts.FontA);
            printer.Font(texto, Fonts.FontB);
            printer.Font(texto, Fonts.FontC);
            printer.Font(texto, Fonts.FontD);
            printer.Font(texto, Fonts.FontE);
            printer.Font(texto, Fonts.SpecialFontA);
            printer.Font(texto, Fonts.SpecialFontB);
            printer.Separator();
            printer.Append("INICIANDO SALTO DE 1 LINEAS");
            printer.NewLine();
            printer.Separator();
            printer.Append("INICIANDO SALTO DE 3 LINEAS");
            printer.NewLines(3);
            printer.Separator();
            printer.Append("DEFINICION DE SEPARADORES");
            printer.Separator('.');
            printer.Separator('-');
            printer.Separator('_');
            printer.Separator('=');
            printer.Separator('+');
            printer.Separator(':');
            printer.Separator('#');
            printer.Separator();
            printer.InitializePrint();
            printer.SetLineHeight(24);
            printer.Append("This is first line with line height of 30 dots");
            printer.SetLineHeight(40);
            printer.Append("This is second line with line height of 24 dots");
            printer.Append("This is third line with line height of 40 dots");
            printer.Separator();
            printer.Append("FIN DEL TEST");
            printer.FullPaperCut();

            IniciaImpresion();
        }

        public Respuesta setPrinter(string printerName)
        {
            Respuesta resp = new Respuesta();

            try
            {
                printer = new Printer(printerName);
                
                if (TipoImpresora == "I")
                    printer.Append(ESC + FONT + B11 + "");

                printer.SetLineHeight(24);
                
                resp.resultado = true;
            }
            catch (Exception e)
            {
                resp.resultado = false;
                resp.mensaje = "No se ha definido impresora de destino, o existe algún error con la impresora";
            }

            return resp;
        }

        private String fnRellenaString(int iCnt, String sTexto, String sPos, String sChrRllno)
        {
            if (sTexto == null) sTexto = string.Empty;

            char relleno = string.IsNullOrEmpty(sChrRllno) ? ' ' : sChrRllno[0];

            if (sTexto.Length >= iCnt)
            {
                return sTexto[..iCnt];
            }

            return sPos == "I"
                ? sTexto.PadLeft(iCnt, relleno)
                : sTexto.PadRight(iCnt, relleno);
        }

        private void Alimentacion()
        {
            printer.NewLines(cantidadLineas);
        }

        private void Alimentacion(int cantidad)
        {
            printer.NewLines(cantidad);
        }

        private void setPedido(String sCodPedd)
        {
            printer.Append(this.fnRellenaString(40, "PEDIDO No: " + sCodPedd + "  Fecha: " + String.Format("{0:d}", DateTime.Today), "F", " "));
        }

        private void SetBeep()
        {
            printer.Append(new byte[] { 27, 66, 4, 4 });
        }

        private void Separador(bool fromComanda = false)
        {
            if (TipoImpresora == "I")
            {
                printer.Append(DefineSeparador.ToString().PadRight(cantidadColumnas, '-'));
            }
            else
            {
                if (fromComanda == true)
                {
                    printer.Append(DefineSeparador.ToString().PadRight(23, '-'));
                }
                else
                {
                    printer.Append(DefineSeparador.ToString().PadRight(cantidadColumnas, '-'));
                }
            }
        }

        private string quitaCaracteresEspeciales(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;

            return string.Create(texto.Length, texto, (buffer, str) =>
            {
                for (int i = 0; i < str.Length; i++)
                {
                    buffer[i] = char.ToUpperInvariant(str[i]) switch
                    {
                        'Á' => 'A',
                        'É' => 'E',
                        'Í' => 'I',
                        'Ó' => 'O',
                        'Ú' => 'U',
                        'Ñ' => 'N',
                        var c => c
                    };
                }
            });
        }

        public void ImprimirTest(string texto)
        {
            Separador();
            printer.Append("PRODUCTO              | CANT. |   P.TOT.");
            Separador();
            printer.Append("POLLO CURRY           |     2 |   12    ");
            printer.Append("POLLO CURRY           |     2 |   12    ");
            printer.Append("POLLO CURRY           |     2 |   12    ");
            printer.Append("POLLO CURRY           |     2 |   12    ");
            Separador();
            printer.Append(texto);
            printer.NewLines(3);
            Separador();
            printer.PartialPaperCut();
            printer.PrintDocument();
            
        }

        #region Cocina
        private List<string> separaObservación(String observacion)
        {
            observacion = quitaCaracteresEspeciales(observacion);
            List<string> msg = new List<string>();

            observacion = observacion.Replace('\r', ' ').Replace('\n', ',').Trim();
            msg = observacion.Split(',').ToList();

            return msg;
        }

        public void Comanda(string target,List<ProdPedd> tbPeddCocna, string sMesa, string sFecha, string sTipo = "",
                            string sScl = "", bool bImpTime = false, string sMsro = "", string sCarrier = "",
                            string sCarrierCod = "", string sCliente = "", string sRetira = "", 
                            string sSclTmp = "")
        {
            printer.NewLine();

            if (TipoImpresora == "T")
            {
                SetBeep();
            }
            
            int cntCols = 23;

            if (cantidadColumnas == 40 && TipoImpresora == "I")
            {
                cntCols = 40;
            }

            if (ImprimeCabeceraComandas)
            {
                printer.ExpandedMode(PrinterModeState.On);
                printer.AlignCenter();
                if (cantidadColumnas > 40)
                    printer.DoubleWidth3();
                printer.BoldMode(target);
                printer.ExpandedMode(PrinterModeState.Off);

                printer.AlignLeft();
                printer.NewLines(2);

                Separador(true);
            }
            
            // Datos del pedido
            sMesa = sMesa.PadRight(8, ' '); 
            sFecha = DateTime.Now.ToString("yyyy-MM-dd hh:mm");

            printer.DoubleWidth2();

            if (sTipo.Equals("") || sTipo.Equals("LOCAL"))
            {
                printer.BoldMode(PrinterModeState.On);
                printer.Append(("MESA : " + sMesa).PadRight(cntCols, ' '));
                printer.BoldMode(PrinterModeState.Off);

                if (SaltoLineaCabeceraComandas)
                {
                    printer.NewLine();
                }

                if (bImpTime) 
                {
                    printer.Append(("FECHA: " + sFecha).PadRight(cntCols, ' '));
                    if (SaltoLineaCabeceraComandas)
                    {
                        printer.NewLine();
                    }
                }
            }
            else if (sTipo.Equals("DOMICILIO") || sTipo.Equals("LLEVAR") || sTipo.Equals("RETIRAR"))
            {
                if (sCliente != "" && sTipo.Equals("LLEVAR") || sTipo.Equals("RETIRAR"))
                {
                    printer.Append(("PARA  : " + sCliente).PadRight(cntCols, ' '));
                    if (SaltoLineaCabeceraComandas)
                    {
                        printer.NewLine();
                    }
                }

                if (sRetira != "" || sTipo.Equals("RETIRAR"))
                {
                    printer.Append(("RETIRA  : " + sRetira).PadRight(cntCols, ' '));
                    if (SaltoLineaCabeceraComandas)
                    {
                        printer.NewLine();
                    }
                }
                if (bImpTime) 
                {
                    printer.Append(("FECHA: " + sFecha).PadRight(cntCols, ' '));
                    if (SaltoLineaCabeceraComandas) printer.NewLine();
                }
            }
            else if (sTipo.Equals("CARRIER"))
            {
                printer.Append(("POR  : " + sCarrier).PadRight(cntCols, ' '));
                if (SaltoLineaCabeceraComandas) printer.NewLine();
                if (bImpTime) 
                { 
                    printer.Append(("FECHA: " + sFecha).PadRight(cntCols, ' '));
                    if (SaltoLineaCabeceraComandas) printer.NewLine();
                }
                printer.Append(("COD  : " + sCarrierCod).PadRight(cntCols, ' '));
                if (SaltoLineaCabeceraComandas) printer.NewLine();
            }
            else
            {
                printer.Append(("MESA : " + sMesa).PadRight(cntCols, ' '));
                if (SaltoLineaCabeceraComandas) printer.NewLine();
                if (bImpTime)
                {
                    printer.Append(("FECHA: " + sFecha).PadRight(cntCols, ' '));
                    if (SaltoLineaCabeceraComandas) printer.NewLine();
                }
            }

            if (ImprimeClienteComanda)
            {
                if (!string.IsNullOrEmpty(sCliente) && sCliente != "CLIENTE FINAL")
                {
                    var cliente = "CLIENTE : " + sCliente;

                    if (cliente.Length < cntCols)
                    {
                        printer.Append(cliente.PadRight(cntCols, ' '));
                    }
                    else
                    {
                        printer.Append(cliente.Substring(0, cntCols).PadRight(cntCols, ' '));
                    }
                }
            }

            if ((cantidadColumnas == 40 || cantidadColumnas == 48 || cantidadColumnas == 68) && TipoImpresora == "I")
            {
                printer.Append(("PEDIDO: " + sScl + "  TIPO : " + sTipo).PadRight(cntCols, ' '));
                
                if (imprimeSecTemporal)
                {
                    if (!string.IsNullOrEmpty(sSclTmp))
                        printer.Append(("     #: " + sSclTmp).PadRight(cntCols, ' '));
                }
                
                if (SaltoLineaCabeceraComandas) printer.NewLine();
            }
            else
            {
                printer.Append(("PEDIDO: " + sScl).PadRight(cntCols, ' '));
                if (imprimeSecTemporal)
                {
                    if (!string.IsNullOrEmpty(sSclTmp))
                        printer.Append(("     #: " + sSclTmp).PadRight(cntCols, ' '));
                }
                if (SaltoLineaCabeceraComandas) printer.NewLine();
                printer.Append(("TIPO: " + sTipo).PadRight(cntCols, ' '));
                if (SaltoLineaCabeceraComandas) printer.NewLine();
            }

            if (("MESERO: " + sMsro).Length >= cntCols)
            {
                printer.Append(("MESERO: " + sMsro).Substring(0, cntCols - 1).PadRight(cntCols, ' '));
            }
            else
            {
                printer.Append(("MESERO: " + sMsro).PadRight(cntCols, ' '));
            }
            if (SaltoLineaCabeceraComandas) printer.NewLine();

            Separador(true);
            if (cantidadColumnas == 40 && TipoImpresora == "I")
            {
                printer.NewLine();
            }
            else
            {
                printer.NewLines(3);
            }
                
            printer.DoubleWidth2();
            if (this.printCajaIzq)
            {
                printer.Append("CANT " + " PRODUCTO".PadRight(cntCols - 5, ' '));
            }
            else
            {
                printer.Append("PRODUCTO".PadRight(cntCols - 5, ' ') + " CANT");
            }
            
            Separador(true);
            printer.DoubleWidth2();

            for (int i = 0; i < tbPeddCocna.Count; i++)
            {
                var cantidad = tbPeddCocna[i].cantidad.ToString().PadRight(4,' ');
                var producto = quitaCaracteresEspeciales(tbPeddCocna[i].producto);
                var producto1 = "";

                if (producto.Length > cntCols - 4)
                {
                    producto1 = producto.Substring(cntCols - 4).Trim();
                    producto = producto.Substring(0, cntCols - 4);
                }

                producto = producto.PadRight(cntCols - 4, ' ');

                if (this.printCajaIzq)
                {
                    printer.Append(cantidad + producto);
                    if (producto1 != "")
                        printer.Append(("    " + producto1).PadRight(cntCols, ' '));
                }
                else
                {
                    printer.Append(producto + cantidad);
                    if (producto1 != "")
                        printer.Append(producto1.PadRight(cntCols, ' '));
                }

                // Agregar datos cuando se tiene ingredientes a quitar
                if (tbPeddCocna[i].noIngredientes.Count > 0)
                {
                    for (int j = 0; j < tbPeddCocna[i].noIngredientes.Count; j++)
                    {
                        string ingdte = tbPeddCocna[i].noIngredientes[j].nombre;

                        if (ingdte.Length > 13) ingdte = ingdte.Substring(0, 12);

                        printer.Append(("    - Sin " + ingdte).PadRight(cntCols, ' '));
                    }
                }

                // Agregar datos provenientes de observacion
                if (tbPeddCocna[i].observacion != "")
                {
                    List<string> sObserv = this.separaObservación(tbPeddCocna[i].observacion);

                    for (int j = 0; j < sObserv.Count; j++)
                    {
                        var comentario = sObserv[j];
                        
                        if (comentario.Length > cntCols - 6)
                        {
                            string[] comentarios = comentario.Split(cntCols - 6).ToArray();

                            for (int m = 0; m < comentarios.Length; m++)
                            {
                                if (m == 0)
                                {
                                    printer.Append(("    - " + comentarios[m].Trim()).PadRight(cntCols, ' '));
                                }
                                else
                                {
                                    printer.Append(("      " + comentarios[m].Trim()).PadRight(cntCols, ' '));
                                }
                            }

                        }
                        else
                        {
                            printer.Append(("    - " + comentario).PadRight(cntCols, ' '));
                        }
                    }
                }
                if (SaltoLineaCabeceraComandas) printer.NewLine();
            }

            Separador(true);

            printer.ExpandedMode(PrinterModeState.Off);
            SaltoYFinal();
        }

        public void fnCocinaPrnt(List<ProdPedd> tbPeddCocna, String sMesa, String sFecha, String sTipo = "", String sScl = "", Boolean bImpTime = false, String sMsro = "", String sCarrier = "", string sCarrierCod = "")
        {
            sMesa = this.fnRellenaString(6, sMesa, "I", " ");
            sFecha = this.fnRellenaString(10, sFecha, "I", " ");

            if (bImpTime) sFecha = sFecha + " " + DateTime.Now.ToString("hh:mm");

            tipo = "C";

            if (sTipo.Equals("") || sTipo.Equals("LOCAL"))
            {
                printer.Append(this.fnRellenaString(40, "MESA: " + sMesa + "  FECHA: " + sFecha, "F", " "));
            }
            else if (sTipo.Equals("DOMICILIO") || sTipo.Equals("LLEVAR"))
            {
                printer.Append(this.fnRellenaString(40, "FECHA: " + sFecha, "F", " "));
            }
            else if (sTipo.Equals("CARRIER"))
            {
                printer.Append(this.fnRellenaString(40, "POR: " + sCarrier + "  FECHA: " + sFecha, "F", " "));
                printer.Append(this.fnRellenaString(40, "COD: " + sCarrierCod, "F", " "));
            }
            else
            {
                printer.Append(this.fnRellenaString(40, "PARA: " + sMesa + "  FECHA: " + sFecha, "F", " "));
            }

            printer.Append(this.fnRellenaString(40, "MESERO: " + sMsro, "F", " "));
            printer.Append(this.fnRellenaString(40, "PEDIDO: " + sScl + "    TIPO: " + sTipo, "F", " "));

            Separador();
            if (this.printCajaIzq)
            {
                printer.Append("CANT.  | PRODUCTO                      .");
            }
            else
            {
                printer.Append("PRODUCTO                        | CANT. ");
            }

            Separador();

            for (int i = 0; i < tbPeddCocna.Count; i++)
            {
                string str4 = "";
                string str5 = "";
                if (this.printCajaIzq)
                {
                    str4 = this.fnRellenaString(6, " " + tbPeddCocna[i].cantidad.ToString(), "F", " ");
                    str5 = this.fnRellenaString(32, tbPeddCocna[i].producto, "F", " ");
                    printer.Append(this.fnRellenaString(40, str4 + " " + str5, "F", " "));
                }
                else
                {
                    str4 = this.fnRellenaString(32, " " + tbPeddCocna[i].producto, "F", " ");
                    str5 = this.fnRellenaString(6, tbPeddCocna[i].cantidad.ToString(), "I", " ");
                    printer.Append(this.fnRellenaString(40, str4 + "   " + str5, "F", " "));
                }

                // Agregar datos cuando se tiene ingredientes a quitar
                if (tbPeddCocna[i].noIngredientes.Count > 0)
                {
                    for (int j = 0; j < tbPeddCocna[i].noIngredientes.Count; j++)
                    {
                        string str6 = this.fnRellenaString(40, "    - Sin " + tbPeddCocna[i].noIngredientes[j].nombre, "F", " ") + " ";
                        printer.Append(str6);
                    }
                }

                if (tbPeddCocna[i].observacion != "")
                {
                    List<string> sObserv = this.separaObservación(tbPeddCocna[i].observacion);

                    for (int j = 0; j < sObserv.Count; j++)
                    {
                        string str7 = this.fnRellenaString(40, "      " + sObserv[j], "F", " ") + " ";
                        printer.Append(str7);
                    }
                }
            }

            Separador();
            Alimentacion();
            printer.PartialPaperCut();
        }

        #endregion

        public void fnDataPeddDmclo(String sCodPedd, String sCliente, String sTlf, String sDir, String sRef)
        {
            sRef = quitaCaracteresEspeciales(sRef);
            sCliente = quitaCaracteresEspeciales(sCliente);
            sDir = quitaCaracteresEspeciales(sDir);
            sTlf = quitaCaracteresEspeciales(sTlf);

            printer.AlignCenter();
            printer.BoldMode("PEDIDO A DOMICILIO ");
            Separador();
            printer.AlignLeft();
            printer.Append(("PEDIDO No : " + sCodPedd).PadRight(cantidadColumnas, ' '));
            Separador();
            printer.Append(("CLIENTE   : " + sCliente).PadRight(cantidadColumnas, ' '));
            printer.Append(("TELEFONO  : " + sTlf).PadRight(cantidadColumnas, ' '));
            printer.Append(("DIRECCION : " + sDir).PadRight(cantidadColumnas, ' '));
            printer.Append(("REFERENCIA : " + sRef).PadRight(cantidadColumnas, ' '));
            Separador();

            Alimentacion();
            printer.PartialPaperCut();
        }

        #region Factura
        private void setCabeceraFactura(String sMesa, String sCodPedd, String sCliCod,
            String sCliNbr, String sCliDir, String sCliTlf)
        {
            sCliNbr = quitaCaracteresEspeciales(sCliNbr);
            sCliDir = quitaCaracteresEspeciales(sCliDir);

            String sHora = DateTime.Now.TimeOfDay.ToString().Substring(0, 5);
            
            printer.Append(this.fnRellenaString(40, "MESA : " + sMesa + "   HORA: " + sHora, "F", " ") + " ");
            setPedido(sCodPedd);
            printer.Append("RUC/CED  : " + this.fnRellenaString(30, sCliCod, "F", " "));
            printer.Append("NOMBRE   : " + sCliNbr);
            printer.Append("DIRECCION: " + this.fnRellenaString(30, sCliDir, "F", " "));
            printer.Append("TELEFONO : " + this.fnRellenaString(30, sCliTlf, "F", " "));

            Separador();
            printer.Append("PRODUCTO              | CANT. |   P.TOT.");
            Separador();
        }

        private string getTipoPedido(string tipo)
        {
            string sTipo = "";
            switch (tipo)
            {
                case "L":
                    sTipo = "LOCAL";
                    break;
                case "D":
                    sTipo = "DOMICILIO";
                    break;
                case "R":
                    sTipo = "RETIRAR";
                    break;
                case "C":
                    sTipo = "LLEVAR";
                    break;
                case "E":
                    sTipo = "DELIVERY";
                    break;
            }

            return sTipo;
        }

        private string getProductoFormat(string producto, int LProducto)
        {
            int cnt = cantidadColumnas - 2;
            if (producto.Length > cantidadColumnas - LProducto - 2)
                producto = producto.Substring(0, cantidadColumnas - LProducto - 2);
            producto = producto.PadRight(cnt - LProducto, ' ');

            return producto;
        }

        private void GeneraCabecera(string NombreEmpresa, string DireccionEmpresa, string Ruc)
        {
            if (AlineacionCabecera == "I") printer.AlignLeft();
            if (AlineacionCabecera == "D") printer.AlignRight();
            if (AlineacionCabecera == "C") printer.AlignCenter();

            switch (cantidadColumnas)
            {
                case 23:
                    printer.ExpandedMode(PrinterModeState.On);
                    break;
                case 64:
                    printer.CondensedMode(PrinterModeState.On);
                    break;
                case 48:
                    printer.NormalWidth();
                    break;
                default:
                    break;
            }

            if (DireccionEmpresa.Length > cantidadColumnas)
                DireccionEmpresa = DireccionEmpresa.Substring(0, cantidadColumnas - 1);

            if (usaCabeceraExpandida) printer.ExpandedMode(PrinterModeState.On);

            if (CabeceraNegrita)
            {
                if (ImprimeNombreEmpresa) printer.BoldMode(NombreEmpresa);
                if (ImprimeRucEmpresa) printer.BoldMode("RUC : " + Ruc);
                if (usaCabeceraExpandida) printer.ExpandedMode(PrinterModeState.Off);
                if (ImprimeDireccionEmpresa) printer.BoldMode(DireccionEmpresa);
            }
            else
            {
                if (ImprimeNombreEmpresa) printer.Append(NombreEmpresa.ToUpper());
                if (ImprimeRucEmpresa) printer.Append(Ruc);
                if (usaCabeceraExpandida) printer.ExpandedMode(PrinterModeState.Off);
                if (ImprimeDireccionEmpresa) printer.Append(DireccionEmpresa);
            }

            if (ImprimeSeparadorEndCabecera) Separador();

            setSaltoLinea(3);
        }

        private void GeneraDatosPedido(string sMesa, string tipoPedd, string mesero, string pedido = "", string sSclTmp = "")
        {
            DateTime fecha = DateTime.Now;
            string sFecha = string.Format("{0:dd-MM-yyyy}",fecha.Date);
            string sHora = fecha.TimeOfDay.ToString().Substring(0, 5);
            /*
             Impresión de los datos del pedido
             */
            printer.AlignLeft();
            printer.Append(("FECHA : " + sFecha + "   HORA : " + sHora).PadRight(cantidadColumnas, ' '));
            if (tipoPedd == "L")
            {
                printer.Append(("MESA  : " + sMesa+ "   TIPO : " + getTipoPedido(tipoPedd)).PadRight(cantidadColumnas, ' '));
            }
            else
            {
                printer.Append(("TIPO : " + getTipoPedido(tipoPedd)).PadRight(cantidadColumnas, ' '));
            }

            if (IncluyeDataMesero)
            {
                printer.Append(("MESERO : " + mesero).PadRight(cantidadColumnas, ' '));
            }

            printer.Append(("PEDIDO : " + pedido).PadRight(cantidadColumnas, ' '));

            if (imprimeSecTemporal)
            {
                if (!string.IsNullOrEmpty(sSclTmp))
                    printer.Append(("     #: " + sSclTmp).PadRight(cantidadColumnas, ' '));
            }

            Separador();
            setSaltoLinea(3);
        }

        private void GeneraAgradecimiento(string NombreEmpresa)
        {
            if (IncluyeAgradecimiento)
            {
                string adicional = "Esperamos contar nuevamente con su asistencia";
                printer.AlignCenter();
                printer.Append(NombreEmpresa + " le agradecce su compra");
                if (adicional.Length > cantidadColumnas)
                    adicional = "¡Vuelva pronto!";
                printer.Append(adicional);
                printer.AlignLeft();
            }
        }

        private void SaltoYFinal()
        {
            if (TipoImpresora == "T")
                printer.NewLines(3);
            else
                printer.NewLines(2);
            printer.PartialPaperCut();
        }

        private void GeneraDetalle(DataTable tbPeddVta, string sSubTotal, 
                                   string sSubTNoIva, string sDctoPct, string sDescuento, 
                                   string sIva, string sTotal)
        {
            /*
             Impresión del detalle de la factura
             */
            string HProducto = "PRODUCTO";
            string HCantidad = " CANT. ";
            string HTotal = " P. UNIT ";
            int LProducto = HCantidad.Length + HTotal.Length;

            HProducto = getProductoFormat(HProducto, LProducto);

            if (ImprimeNegritaCabeceraTabla)
            {
                printer.AlignCenter();
                printer.BoldMode("DETALLE DE VALORES A FACTURAR");
                printer.AlignLeft();
                setSaltoLinea();
                printer.BoldMode(HProducto + " " + HCantidad + " " + HTotal);
            }
            else
            {
                printer.AlignCenter();
                printer.Append("DETALLE DE VALORES A FACTURAR");
                printer.AlignLeft();
                setSaltoLinea();

                printer.Append(HProducto + " " + HCantidad + " " + HTotal);
            }

            foreach (DataRow data in tbPeddVta.Rows)
            {
                string producto = data["itemNbr"].ToString();
                string cantidad = data["itemCnt"].ToString();
                string formato;

                if (CantidadDecimalesItems == 4) formato = "{0:0.0000}";
                else if (CantidadDecimalesItems == 3) formato = "{0:0.000}";
                else formato = "{0:0.00}";

                string precio = string.Format(formato, Math.Round(Utils.StringToDouble(data["itemPrc"].ToString()), CantidadDecimalesItems));

                producto = getProductoFormat(producto, LProducto);

                printer.Append(producto + " " + cantidad.PadLeft(HCantidad.Length, ' ') + " " + precio.PadLeft(HTotal.Length, ' '));
            }

            if (ImprimeSeparadorTablas) Separador();

            setSaltoLinea(CantidadLineasSaltarEndTabla, true);
            
            printer.Append(("SUB-TOTAL SIN IVA :" + sSubTNoIva.PadLeft(8, ' ')).PadRight(cantidadColumnas, ' '));
            printer.Append(("SUB-TOTAL CON IVA :" + sSubTotal.PadLeft(8, ' ')).PadRight(cantidadColumnas, ' '));
            printer.Append(("DESCUENTO (" + sDctoPct.PadLeft(5, ' ') + "%):" + String.Format("{0:0.00}", sDescuento).PadLeft(8, ' ')).PadRight(cantidadColumnas, ' '));
            printer.Append(("I.V.A.            :" + sIva.PadLeft(8, ' ')).PadRight(cantidadColumnas, ' '));
            printer.Append(("TOTAL GENERAL     :" + sTotal.PadLeft(8, ' ')).PadRight(cantidadColumnas, ' '));
            setSaltoLinea();

            if (ImprimeSeparadorTablas) Separador();
            setSaltoLinea(2, true);

        }

        public void CierreTicket(string NombreEmpresa, string DireccionEmpresa, string Ruc, CierreCaja cierreCaja, bool MuestraEliminados = false)
        {
            if (TipoImpresora == "T") printer.NewLines(3);
            GeneraCabecera(NombreEmpresa, DireccionEmpresa, Ruc);

            printer.AlignCenter();
            printer.ExpandedMode(PrinterModeState.On);
            printer.BoldMode("CIERRE DE CAJA");
            printer.ExpandedMode(PrinterModeState.Off);

            if (TipoImpresora == "T") printer.NewLines(3);

            printer.InitializePrint();
            printer.AlignLeft();

            printer.Append("SECUENCIAL : " + cierreCaja.secuencial);
            printer.Append("CAJA       : " + cierreCaja.caja);
            printer.Append("FECHA      : " + cierreCaja.fecha + " " + cierreCaja.hora);
            Separador();

            setSaltoLinea(3);

            printer.BoldMode("VENTAS");
            Separador();
            printer.Append("LOCAL      : " + string.Format("{0:0.00}", cierreCaja.ventas.locales).PadLeft(8, ' '));
            printer.Append("DOMICILIO  : " + string.Format("{0:0.00}", cierreCaja.ventas.domicilio).PadLeft(8, ' '));
            printer.Append("LLEVAR     : " + string.Format("{0:0.00}", cierreCaja.ventas.llevar).PadLeft(8, ' '));
            printer.Append("RETIRAR    : " + string.Format("{0:0.00}", cierreCaja.ventas.retirar).PadLeft(8, ' '));

            setSaltoLinea(3);

            printer.BoldMode("  FORMA DE PAGO");
            printer.NewLine();

            foreach (var pago in cierreCaja.ventas.pago)
            {
                string spago = pago.tipo.ToString().PadRight(16, ' ');

                printer.Append($"  {spago}: " + string.Format("{0:0.00}", pago.valor).PadLeft(8, ' '));
                setSaltoLinea();
            }

            printer.Append("  TOTAL         ".PadRight(18, ' ') + ": " + string.Format("{0:0.00}", cierreCaja.ventas.total).PadLeft(8, ' '));
            setSaltoLinea(3, true);

            printer.BoldMode("VALES DE INGRESO/EGRESO");
            Separador();

            printer.Append("  INGRESO : " + string.Format("{0:0.00}", cierreCaja.vales.valorIngreso).PadLeft(8, ' '));
            printer.Append("  EGRESO  : " + string.Format("{0:0.00}", cierreCaja.vales.valorEgreso).PadLeft(8, ' '));

            if (cierreCaja.salarios > 0)
            {
                printer.NewLine();
                printer.BoldMode("SALARIOS PAGADOS CON CAJA CHICA");
                Separador();
                printer.Append("  SALARIOS : " + string.Format("{0:0.00}", cierreCaja.salarios).PadLeft(8, ' '));
            }

            setSaltoLinea(3, true);

            double totalCompras = 0;
            if (cierreCaja.compras != null)
            {
                if (cierreCaja.compras.valor > 0)
                {
                    printer.BoldMode("COMPRAS");
                    Separador();

                    printer.BoldMode("    FORMA DE PAGO");
                    totalCompras = cierreCaja.compras.pago.Select(o => o.valor).Sum();

                    foreach (var pago in cierreCaja.compras.pago)
                    {
                        var spago = pago.tipo.ToString().PadRight(19, ' ');

                        printer.Append($"  {spago}: " + string.Format("{0:0.00}", pago.valor).PadLeft(8, ' '));
                    }
                }
            }

            Separador();
            var totalSistema = cierreCaja.inicio + cierreCaja.ventas.total - totalCompras +
                cierreCaja.vales.valorIngreso - cierreCaja.vales.valorEgreso + cierreCaja.delivery;

            printer.Append("INICIO DE CAJA: (+) " + string.Format("{0:0.00}", cierreCaja.inicio).PadLeft(8, ' '));

            Separador();
            if (TipoImpresora == "T") printer.NewLines(2);
            printer.Append("TOTAL INGRESO :     " + string.Format("{0:0.00}", cierreCaja.inicio + cierreCaja.ventas.total + cierreCaja.vales.valorIngreso).PadLeft(8, ' '));
            printer.Append("TOTAL EGRESO  : (-) " + string.Format("{0:0.00}", cierreCaja.vales.valorEgreso + totalCompras + cierreCaja.salarios).PadLeft(8, ' '));
            printer.Append("VALOR SISTEMA :     " + string.Format("{0:0.00}", totalSistema).PadLeft(8, ' '));
            printer.Append("VALOR EFECTIVO:     " + string.Format("{0:0.00}", cierreCaja.efectivo).PadLeft(8, ' '));
            printer.Append("VALOR TARJETA :     " + string.Format("{0:0.00}", cierreCaja.tarjeta).PadLeft(8, ' '));
            printer.Append("VALOR CHEQUE  :     " + string.Format("{0:0.00}", cierreCaja.cheque).PadLeft(8, ' '));
            printer.Append("VALOR TRANSFER:     " + string.Format("{0:0.00}", cierreCaja.transfer).PadLeft(8, ' '));
            printer.Append("VALOR CIERRE  :     " + string.Format("{0:0.00}", cierreCaja.cierre).PadLeft(8, ' '));
            printer.Append("DIFERENCIA    :     " + string.Format("{0:0.00}", cierreCaja.cierre - totalSistema).PadLeft(8, ' '));

            if (MuestraEliminados)
            {
                setSaltoLinea(3, true);

                printer.BoldMode("VALORES DE FACTURAS ELIMINADAS/ANULADAS");
                Separador();

                printer.Append("  ELIMINADAS: " + string.Format("{0:0.00}", cierreCaja.eliminado).PadLeft(8, ' '));
                printer.Append("  ANULADAS  : " + string.Format("{0:0.00}", cierreCaja.anulado).PadLeft(8, ' '));

                setSaltoLinea(3);
            }

            Separador();
            printer.Append("VALOR DELIVERY:     " + string.Format("{0:0.00}", cierreCaja.delivery).PadLeft(8, ' '));

            setSaltoLinea(3);

            Separador();
            setSaltoLinea(10, true, 2);

            printer.Append("Firma: ".PadRight(cantidadColumnas, '_'));
            SaltoYFinal();
        }

        public void PreFactura(DataTable tbPeddVta, string NombreEmpresa, string DireccionEmpresa,
                                string Ruc, string tipoPedd, string sCodPedd, string mesero,
                                string sMesa, string sSubTotal, string sSubTNoIva, string sDescuento,
                                string sDctoPct, string sIva, string sTotal)
        {
            if (TipoImpresora == "T")
                printer.NewLines(3);
            // Genera cabecera
            GeneraCabecera(NombreEmpresa, DireccionEmpresa, Ruc);

            // Genera datos del Pedido
            GeneraDatosPedido(sMesa, tipoPedd, mesero);

            // Genera detalle de Factura
            GeneraDetalle(tbPeddVta, sSubTotal, sSubTNoIva, sDctoPct, sDescuento, sIva, sTotal);

            /*
             Imprime Opciones para Llenado de datos del cliente:
             */
            if (ImprimeNegritaCabeceraTabla)
                printer.BoldMode("DATOS DEL CLIENTE PARA FACTURACION");
            else
                printer.Append("DATOS DEL CLIENTE PARA FACTURACION");

            string linea = "_".PadRight(cantidadColumnas - 11, '_');
            string lineafull = "_".PadRight(cantidadColumnas, '_');

            if(TipoImpresora == "T")
                printer.NewLine();

            printer.Append("RUC/CI   : " + linea);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append("NOMBRE   : " + linea);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append("APELLIDO : " + linea);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append("DIRECCION: " + linea);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append(lineafull);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append(lineafull);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append("TELEFONO : " + linea);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append("EMAIL    : " + linea);
            if (TipoImpresora == "T") printer.NewLine();
            printer.Append(lineafull);

            setSaltoLinea(3, true);

            GeneraAgradecimiento(NombreEmpresa);

            SaltoYFinal();
        }

        private void setSaltoLinea(int cantidadLineas = 0, bool saltoParaImpacto = false, int cantidadImpacto = 0)
        {
            if (TipoImpresora == "T" && usaEspaciosInterlineado == true)
            {
                if(cantidadLineas > 0)
                    printer.NewLines(cantidadLineas);
                else
                    printer.NewLine();
            }
            else
            {
                if (saltoParaImpacto)
                {
                    if (cantidadImpacto > 0)
                    {
                        printer.NewLines(cantidadImpacto);
                    }
                    else
                    {
                        printer.NewLine();
                    }
                }
            }
        }

        public void coordenadas(string empleado, trjCoordRow coord, string NombreEmpresa, string DireccionEmpresa, string Ruc)
        {
            if (TipoImpresora == "T")
                printer.NewLines(3);

            AlineacionCabecera = "C";
            usaCabeceraExpandida = true;
            printer.NormalWidth();
            Separador();
            ImprimeNombreEmpresa = true;
            ImprimeRucEmpresa = true;
            // Genera cabecera
            GeneraCabecera(NombreEmpresa, DireccionEmpresa, Ruc);
            printer.Append(("Empleado: " + empleado).PadRight(cantidadColumnas, ' '));
            Separador();

            if (ImprimeNegritaCabeceraTabla)
                printer.BoldMode("  | A | B | C | D | E | F | G | H | I | J ");
            else
                printer.Append("  | A | B | C | D | E | F | G | H | I | J ");
            Separador();

            printer.Append("1 |" + coord.f1.c01 + "|" + coord.f1.c02 + "|" + coord.f1.c03 + "|" + coord.f1.c04 + "|" + coord.f1.c05 + "|"
                                + coord.f1.c06 + "|" + coord.f1.c07 + "|" + coord.f1.c08 + "|" + coord.f1.c09 + "|" + coord.f1.c10);

            printer.Append("2 |" + coord.f2.c01 + "|" + coord.f2.c02 + "|" + coord.f2.c03 + "|" + coord.f2.c04 + "|" + coord.f2.c05 + "|"
                                + coord.f2.c06 + "|" + coord.f2.c07 + "|" + coord.f2.c08 + "|" + coord.f2.c09 + "|" + coord.f2.c10);

            printer.Append("3 |" + coord.f3.c01 + "|" + coord.f3.c02 + "|" + coord.f3.c03 + "|" + coord.f3.c04 + "|" + coord.f3.c05 + "|"
                                + coord.f3.c06 + "|" + coord.f3.c07 + "|" + coord.f3.c08 + "|" + coord.f3.c09 + "|" + coord.f3.c10);

            printer.Append("4 |" + coord.f4.c01 + "|" + coord.f4.c02 + "|" + coord.f4.c03 + "|" + coord.f4.c04 + "|" + coord.f4.c05 + "|"
                                + coord.f4.c06 + "|" + coord.f4.c07 + "|" + coord.f4.c08 + "|" + coord.f4.c09 + "|" + coord.f4.c10);

            printer.Append("5 |" + coord.f5.c01 + "|" + coord.f5.c02 + "|" + coord.f5.c03 + "|" + coord.f5.c04 + "|" + coord.f5.c05 + "|"
                                + coord.f5.c06 + "|" + coord.f5.c07 + "|" + coord.f5.c08 + "|" + coord.f5.c09 + "|" + coord.f5.c10);

            Separador();
            SaltoYFinal();
        }

        public void Factura(DataTable tbPeddVta, DataTable tbPagoVta, string NombreEmpresa, string DireccionEmpresa,
                                string Ruc, string sCliCod, string sCliNbr, string sCliDir, string sCliTlf, string sCliEmail,
                                string tipoPedd, string sCodPedd, string mesero,
                                string sMesa, string sSubTotal, string sSubTNoIva, string sDescuento,
                                string sDctoPct, string sIva, string sTotal, string sRecibo, string sCambio,
                                string clave, string ventscl, bool esCredito = false, string emprNbr = "", string rucEmpr = "",
                                bool emprPaga = false, string sclTemp = "")
        {
            if (TipoImpresora == "T")
                printer.NewLines(3);
            // Genera cabecera
            GeneraCabecera(NombreEmpresa, DireccionEmpresa, Ruc);

            // Genera datos del Pedido
            GeneraDatosPedido(sMesa, tipoPedd, mesero, sCodPedd, sclTemp);

            /*
             Imprime Opciones para Llenado de datos del cliente:
             */
            printer.AlignCenter();
            if (ImprimeNegritaCabeceraTabla)
                printer.BoldMode("DATOS DEL CLIENTE PARA FACTURACION");
            else
                printer.Append("DATOS DEL CLIENTE PARA FACTURACION");

            printer.AlignLeft();

            setSaltoLinea();

            Separador();

            printer.Append(("RUC/CI   : " + sCliCod).PadRight(cantidadColumnas, ' '));
            setSaltoLinea();
            printer.Append(("CLIENTE  : " + sCliNbr).PadRight(cantidadColumnas, ' '));
            setSaltoLinea();
            printer.Append(("DIRECCION: " + sCliDir).PadRight(cantidadColumnas, ' '));
            setSaltoLinea();
            printer.Append(("TELEFONO : " + sCliTlf).PadRight(cantidadColumnas, ' '));
            setSaltoLinea();
            printer.Append(("EMAIL    : " + sCliEmail).PadRight(cantidadColumnas, ' '));
            setSaltoLinea();
            Separador();
            setSaltoLinea();

            // Genera detalle de Factura
            GeneraDetalle(tbPeddVta, sSubTotal, sSubTNoIva, sDctoPct, sDescuento, sIva, sTotal);

            // Genera detalle del Pago
            if (ImprimeFormaDePago)
            {
                if (!esCredito || tbPagoVta.Rows.Count > 0)
                {
                    printer.AlignCenter();
                    printer.Append("DETALLE/RESUMEN DE LA FORMA DE PAGO");
                    printer.AlignLeft();

                    foreach (DataRow pago in tbPagoVta.Rows)
                    {
                        var tipo = pago["pgoForm"];
                        var valor = pago["pgoValor"];

                        switch (tipo)
                        {
                            case "E":
                                tipo = "EFECTIVO";
                                break;
                            case "T":
                                tipo = "TARJETA DE CREDITO";
                                break;
                            case "C":
                                tipo = "CHEQUE";
                                break;
                            case "F":
                                tipo = "TRANSFERENCIA";
                                break;
                        }

                        printer.Append(("TIPO : " + tipo.ToString().PadRight(18, ' ') +
                                        " VALOR : " + string.Format("{0:0.00}",
                                                       Utils.StringToDouble(valor.ToString()))
                                        .PadRight(8, ' ')).PadRight(cantidadColumnas, ' '));
                    }
                }
                else
                {
                    if (TipoImpresora == "T")
                        printer.ExpandedMode(PrinterModeState.On);
                    printer.AlignCenter();
                    printer.Append("DETALLE DEL CRÉDITO");
                    printer.AlignLeft();
                    if (TipoImpresora == "T")
                        printer.ExpandedMode(PrinterModeState.Off);

                    if (string.IsNullOrEmpty(emprNbr) || !emprPaga)
                    {
                        printer.Append(("DEUDOR : " + sCliNbr).PadRight(cantidadColumnas, ' '));
                    }
                    else
                    {
                        printer.Append(("DEUDA TRANSFERIDA A : " + emprNbr).PadRight(cantidadColumnas, ' '));
                    }

                    printer.Append("VALOR : " + string.Format("{0:0.00}",
                                                       Utils.StringToDouble(sTotal.ToString()))
                                        .PadRight(8, ' ').PadRight(cantidadColumnas, ' '));

                }

            }

            setSaltoLinea(3);

            printer.Append("RECIBE : " + sRecibo);
            setSaltoLinea();
            printer.Append("CAMBIO : " + sCambio);
            Separador();

            // Imprime clave para descarga
            ImprimeClaveDownload(clave);

            if (TipoImpresora == "T") printer.NewLines(5);

            // Genera agradecimiento
            GeneraAgradecimiento(NombreEmpresa);
            SaltoYFinal();
        }

        private void ImprimeClaveDownload(string clave)
        {
            if (!string.IsNullOrEmpty(clave))
            {
                printer.Append("Para descargar su factura acceda al sitio web ");
                printer.Append(UrlDescarga + " /#/facturas/descarga");
                printer.Append("Usuario = CEDULA / RUC");
                printer.Append("CLAVE = " + clave);
                Separador();
            }
        }

        public void ComprobantePago(string NombreEmpresa, string DireccionEmpresa, string Ruc, 
                                     string nid, string empleado, string cantidad, string fecha, string hora,
                                     string motivo, string tipo)
        {
            if (TipoImpresora == "T")
                printer.NewLines(3);
            // Genera cabecera
            GeneraCabecera(NombreEmpresa, DireccionEmpresa, Ruc);

            /*
             Imprime Datos del Empleado:
             */
            printer.AlignCenter();
            if (ImprimeNegritaCabeceraTabla)
                printer.BoldMode("DATOS DEL EMPLEADO");
            else
                printer.Append("DATOS DEL EMPLEADO");

            printer.AlignLeft();

            setSaltoLinea();

            Separador();

            empleado = quitaCaracteresEspeciales(empleado);
            printer.Append(("RUC/CI   : " + nid).PadRight(cantidadColumnas, ' '));
            printer.Append(("Empleado : " + empleado).PadRight(cantidadColumnas, ' '));
            printer.Append(("Motivo   : " + motivo.ToUpper()).PadRight(cantidadColumnas, ' '));
            printer.Append(("Fecha    : " + fecha).PadRight(cantidadColumnas, ' '));
            printer.Append(("Tipo     : " + tipo).PadRight(cantidadColumnas, ' '));
            printer.Append(("Hora     : " + hora).PadRight(cantidadColumnas, ' '));

            setSaltoLinea();
            printer.Append("Ha recivido la cantidad de: $ " + cantidad +" DOLARES.".PadRight(cantidadColumnas, ' '));
            printer.AlignCenter();
            printer.Append("RECIBIDO");
            setSaltoLinea(3);
            printer.Append("_____________________________");
            if (ImprimeNegritaCabeceraTabla)
            {
                printer.BoldMode(empleado);
                printer.BoldMode("CI: " + nid);
            }
            else
            {
                printer.Append(empleado);
                printer.Append("CI: " + nid);
            }
            printer.AlignLeft();
            SaltoYFinal();
        }


        public void fnPrefactura(DataTable tbPeddVta, DataTable tbPagoVta, string NombreEmpresa, string DireccionEmpresa, 
                                string tipoPedd, String sCodPedd,
                                String sMesa, String sSubTotal, string sSubTNoIva, String sDescuento, 
                                String sDctoPct, String sIva, String sTotal)
        {
            printer.Append(this.fnRellenaString(41, NombreEmpresa, "F", " "));
            printer.Append(this.fnRellenaString(41, DireccionEmpresa, "F", " "));

            Separador();

            String sHora = DateTime.Now.TimeOfDay.ToString().Substring(0, 5);

            string sTipo = getTipoPedido(tipoPedd);
            
            printer.Append(this.fnRellenaString(40, "MESA : " + sMesa + "   HORA: " + sHora, "F", " ") + " ");
            printer.Append(this.fnRellenaString(40, "TIPO : " + sTipo, "F", " "));

            setPedido(sCodPedd);
            
            Separador();
            printer.Append("PRODUCTO              | CANT. |   P.TOT.");
            Separador();

            int num2 = tbPeddVta.Rows.Count;
            int index1 = 7;

            for (int i = 0; i < num2; i++)
            {
                try
                {
                    String str4 = this.fnRellenaString(25, tbPeddVta.Rows[i]["itemNbr"].ToString(), "F", " ");
                    String str5 = this.fnRellenaString(6, tbPeddVta.Rows[i]["itemCnt"].ToString(), "I", " ");
                    string str6 = "";

                    str6 = this.fnRellenaString(9, Math.Round(
                           Utils.StringToDouble(tbPeddVta.Rows[i]["itemCnt"].ToString()) *
                           Utils.StringToDouble(tbPeddVta.Rows[i]["itemPrc"].ToString()),
                           4).ToString(), "I", " ");

                    printer.Append(str4 + " " + str5 + "  " + str6);
                }
                catch (Exception)
                {
                    printer.Append(this.fnRellenaString(41, "", "F", " "));
                }
                
            }

            for (int i = 0; i < 28 - index1; i++)
            {
                printer.Append(" ");
                index1 += 1;
            }

            Separador();

            printer.Append("SUB-TOTAL SIN IVA:" + this.fnRellenaString(23, sSubTNoIva, "I", " "));
            printer.Append("SUB-TOTAL CON IVA:" + this.fnRellenaString(23, sSubTotal, "I", " "));
            printer.Append("DESCUENTO (" + this.fnRellenaString(4, sDctoPct, "I", " ") + "%):" + this.fnRellenaString(23, String.Format("{0:0.00}", sDescuento), "I", " "));
            printer.Append("I.V.A.           :" + this.fnRellenaString(23, sIva, "I", " "));
            printer.Append("TOTAL GENERAL    :" + this.fnRellenaString(23, sTotal, "I", " "));

            Separador();
            
            printer.Append(this.fnRellenaString(41, "DATOS DEL CLIENTE PARA FACTURACION", "F", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("RUC/CI: " + this.fnRellenaString(33, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("NOMBRE: " + this.fnRellenaString(33, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("APELLIDO: " + this.fnRellenaString(31, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("DIRECCION: " + this.fnRellenaString(30, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("TELEFONO: " + this.fnRellenaString(31, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("EMAIL: " + this.fnRellenaString(33, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, "_", "I", "_"));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, "Agradecemos su compra", "I", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));

            Alimentacion();
            printer.PartialPaperCut();
        }

        public void fnFacturaPrnt(DataTable tbPeddVta, DataTable tbPagoVta, String sCodPedd,
                                String sMesa, String sSubTotal, string sSubTNoIva, String sDescuento, String sDctoPct, String sIva, String sTotal,
                               String sCliCod, String sCliNbr, String sCliDir, String sCliTlf, String sRecibo,
                               String sCambio, String sTipo)
        {
            tipo = sTipo == "F" ? "F" : "P";
            setCabeceraFactura(sMesa, sCodPedd, sCliCod, sCliNbr, sCliDir, sCliTlf);

            int num2 = tbPeddVta.Rows.Count;
            int index1 = 7;

            if (sTipo.Equals("F") && num2 > 20)
            {
                num2 = 20;
            }
            
            for (int i = 0; i < num2; i++)
            {
                String str4 = this.fnRellenaString(25, tbPeddVta.Rows[i]["itemNbr"].ToString(), "F", " ");
                String str5 = this.fnRellenaString(6, tbPeddVta.Rows[i]["itemCnt"].ToString(), "I", " ");
                string str6 = "";

                if (sTipo.Equals("F"))
                {
                    str6 = this.fnRellenaString(9, Math.Round(
                        Utils.StringToDouble(tbPeddVta.Rows[i]["itemCnt"].ToString()) *
                        Utils.StringToDouble(tbPeddVta.Rows[i]["itemPrc"].ToString()),
                        4).ToString(), "I", " ");
                }
                else
                {
                    str6 = this.fnRellenaString(9, Math.Round(
                        Utils.StringToDouble(tbPeddVta.Rows[i]["itemCnt"].ToString())*
                        Utils.StringToDouble(tbPeddVta.Rows[i]["itemPrc"].ToString()) +
                        Utils.StringToDouble(tbPeddVta.Rows[i]["itemIva"].ToString()), 
                        6).ToString(), "I", " ");
                }

                printer.Append(str4 + " " + str5 + "  " + str6);
            }

            if (sTipo.Equals("F"))
            {
                for (int i = 0; i < 28 - index1; i++)
                {
                    printer.Append(" ");
                    index1 += 1;
                }
            }

            Separador();

            if (sTipo.Equals("F"))
            {
                printer.Append("SUB-TOTAL SIN IVA:" + this.fnRellenaString(23, sSubTNoIva, "I", " "));
                printer.Append("SUB-TOTAL CON IVA:" + this.fnRellenaString(23, sSubTotal, "I", " "));
                printer.Append("DESCUENTO (" + this.fnRellenaString(4, sDctoPct, "I", " ") + "%):" + this.fnRellenaString(23, String.Format("{0:0.00}", sDescuento), "I", " "));
                printer.Append("I.V.A.           :" + this.fnRellenaString(23, sIva, "I", " "));
                printer.Append("TOTAL GENERAL    :" + this.fnRellenaString(23, sTotal, "I", " "));
            }
            else
            {
                printer.Append("TOTAL GENERAL   :" + this.fnRellenaString(24, sTotal, "I", " "));
            }

            printer.Append("============= FORMA DE PAGO ============= ");
            foreach(DataRow pago in tbPagoVta.Rows)
            {
                var tipo = pago["pgoForm"];
                var valor = pago["pgoValor"];
                var cambio = pago["pgoCbio"];

                printer.Append(this.fnRellenaString(41, 
                    "TIPO : " + (tipo.Equals("E") ? "EFECTIVO " : 
                    (tipo.ToString().Equals("T") ? "TARJETA  " : 
                    "CHEQUE   ")) + " VALOR : " + valor.ToString(), "F", " "));
            }
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("RECIBE : " + this.fnRellenaString(31, sRecibo, "I", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append("CAMBIO : " + this.fnRellenaString(31, sCambio, "I", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));
            printer.Append(this.fnRellenaString(41, " ", "I", " "));

            Separador();

            Alimentacion();
            printer.PartialPaperCut();

            if(sTipo == "F")
            {
                printer.OpenDrawer();
            }
        }

        #endregion

        public void IniciaImpresion()
        {
            printer.PrintDocument();
        }

        public void sbIpr()
        {
            try
            {
                printer.PrintDocument();
                DoEvents();
                if (tipo == "F" && impresionOriginal)
                {
                    printer.OpenDrawer();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: elimine el estado administrado (objetos administrados).
                }

                // TODO: libere los recursos no administrados (objetos no administrados) y reemplace el siguiente finalizador.
                // TODO: configure los campos grandes en nulos.

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~EposPrint() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

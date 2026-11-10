using Entidades;
using LibConfig.cls;
using LibEncrypt;
using LibHttpClient;
using LibHttpClient.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using TreskelRest.frms;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace TreskelRest.cls
{
    public enum TamanioBotonesMenu
    {
        Normal = 1,
        Small = 2
    }

    public enum usaMailTipo
    {
        Pedido = 1,
        Factura = 2
    }

    public enum usaTipoAccion
    {
        Anulacion = 1,
        Eliminacion = 2
    }

    public class General
    {
        public static Principal principal;

        public static TamanioBotonesMenu TAMANIO_BOTONES_PEDD = TamanioBotonesMenu.Normal;
        public static TamanioBotonesMenu TAMANIO_BOTONES_FACT = TamanioBotonesMenu.Normal;

        public static int CANTIDAD_CARACTERES_PASSWORD = 20;
        public static int CAMBIO_RAPIDO = 0;
        public static int CANTIDAD_MESAS = 5;
        public static int CANTIDAD_MESAS_ALTERNA = 0;
        public static int CANTIDAD_MESAS_EXTERIOR = 0;
        public static int CANTIDAD_MESAS_TERRAZA = 0;
        public static int CANTIDAD_MESAS_2DO_PISO = 0;
        public static int IMPRIME_HORA_COCINA = 0;
        public static int CANTIDAD_LINEAS_IMPRESION = 5;
        public static int CANTIDAD_COLUMNAS_IMPRESION = 48;
        public static int CANTIDAD_LINEAS_FINAL_ITEMS = 5;
        public static int CANTIDAD_DECIMALES_EN_ITEMS = 4;
        public static int CANTIDAD_COLUMNAS_GRUPOS = 1;
        public static int CANTIDAD_COPIAS_FACTURA = 1;

        public static bool EJECUTA_GOTFOCUS = false;
        public static bool USA_CLAVE_NUMERICA = false;
        public static bool PUEDE_ELIMINAR_PEDIDO = false;
        public static bool PUEDE_DAR_REGALO = false;
        public static bool PUEDE_DAR_DESCUENTO = false;
        public static bool PUEDE_ANULAR_FACTURA = false;
        public static bool SISTEMA_VALIDADO = false;
        public static bool SISTEMA_REGISTRADO = false;
        public static bool COPIAR_CORREOS = false;
        public static bool TICKET_COCINA_CANTIDAD_IZQ = false;
        public static bool IMPRIME_TICKET_CARRIER_AUT = true;
        public static bool ENVIA_CORREO_POR_ELIMINACION = false;
        public static bool PROCESANDO = false;
        public static bool USA_IMPRESORA_PROFORMA = false;
        public static bool ENVIA_IMPRESORA_PARRILLA = false;
        public static bool ENVIA_IMPRESORA_BAR = false;
        public static bool USA_TECLADO_PANTALLA = false;
        public static bool USA_DESCUENTOS_PERSONALIZADOS = false;
        public static bool LLEVA_CONTABILIDAD = false;
        public static bool USA_RETENCIONES = false;
        public static bool USA_FACTURACION_ELECTRONICA = false;
        public static bool ENVIO_AUTO_AL_SRI = false;
        public static bool ENVIO_ONLINE = false;
        public static bool ES_TIPO_RIMPE = false;
        public static bool AGREGA_SERVICIO = false;
        public static bool IMPRIME_NOMBRE_EMPRESA = true;
        public static bool IMPRIME_DIRECCION_EMPRESA = true;
        public static bool IMPRIME_RUC_EMPRESA = true;
        public static bool NEGRITA_EN_CABECERA = true;
        public static bool IMPRIME_SEPARADO_FIN_CABECERA = true;
        public static bool IMPRIME_NEGRITA_CABECERA_CONTENIDO = true;
        public static bool IMPRIME_SEPARADOR_FIN_ITEMS = true;
        public static bool IMPRIME_FORMA_PAGO = true;
        public static bool IMPRIME_NOMBRE_MESERO = true;
        public static bool IMPRIME_AGRADECIMIENTO = true;
        public static bool IMPRIME_TICKET_CIERRE_CAJA = true;
        public static bool VALIDA_COMPRA_RETENCION = true;
        public static bool IMPRIME_CABECERA_COMANDAS = true;
        public static bool SALTOLINEA_CABECERA_COMANDAS = false;
        public static bool MUESTRA_ELIMINADOS_ANULADOS = false;
        public static bool USA_ESPACIOS_INTERLINEADO = false;
        public static bool MODO_EXPANDIDO_CABECERA_FACT = false;
        public static bool IMPRIME_COMANDAS_MARKET = false;
        public static bool MUESTRA_MESAS_MARKET = false;
        public static bool PERMITE_DAR_CREDITO = false;
        public static bool IMPRIME_RETENCION = false;
        public static bool USA_API_MAIL = false;
        public static bool USA_MAIL_RELAY = false;
        public static bool IMPRIME_CLAVE_DOWNLOAD = false;
        public static bool USA_ESPACIO_INTERLINEADO_FACT = false;
        public static bool USA_ESPACIO_INTERLINEADO_COCINA = false;
        public static bool USA_ESPACIO_INTERLINEADO_PARRILLA = false;
        public static bool USA_ESPACIO_INTERLINEADO_BAR = false;
        public static bool MUESTRA_CLIENTE_LLEVAR = false;
        public static bool IMPRIME_CLIENTE_COMANDA = false;
        public static bool PERMITE_USO_BARCODE = false;
        public static bool PERMITE_BUSCAR_ITEMS = false;
        public static bool CLICK_GRUPO_BUSCAR_ITEMS = false;
        public static bool USA_WHATSAPP = false;
        public static bool USA_SMS = false;
        public static bool USA_COORD_ALTERNO = false;
        public static bool IMPRIME_SECUENCIAL_TEMPORAL = false;
        public static bool USA_FONDO_RANDOM = false;
        public static bool SOLO_VENTAS_LOCALES = false;
        public static bool CONFIRMA_RECEPTOR = false;
        public static bool INCLUYE_PRECIOS_COMPRAS = true;

        public static string connectionString = "";
        public static string SEPARADOR = "-";
        public static string ALINEACION_CABECERA = "C";
        public static string CAJA = "0001";
        public static string CODUSUARIO = "";
        public static string USUARIO = "";
        public static string TipoVentana = "V";
        public static string NOMBRE_USUARIO = "";
        public static string MODO_PEDIDO_FACTURACION = "";
        public static string ORGANIZA_ITEMS_PEDIDO = "H";
        public static string TIPO_BOTONES = "ST";
        public static string CORREO_HOST = "";
        public static string CORREO_PORT = "";
        public static string CORREO_USER = "";
        public static string CORREO1 = "";
        public static string CORREO2 = "";
        public static string CORREO3 = "";
        public static string CORREO4 = "";        
        public static string CORREO_PASSWORD = "";
        public static string CORREO_AUTH = "";
        public static string CORREO_UTLS = "";
        public static string SUCURSAL = "";
        public static string PRINTER_COCINA = "";
        public static string PRINTER_FACTURA = "";
        public static string PRINTER_PROFORMA = "";
        public static string PRINTER_PARRILLA = "";
        public static string PRINTER_BAR = "";
        public static string RUC_EMPRESA = "";
        public static string EMPRESA = "";
        public static string LOGO_EMPRESA = "";
        public static string RAZON_SOCIAL = "";
        public static string DIRECCION_MATRIZ = "";
        public static string DIRECCION_LOCAL = "";
        public static string CODIGO_EMPRESA = "";
        public static string MAINBOARD_SERIAL = "";
        public static string FORMATO_NUMERICO_REPORTE = "###.##0,00";
        public static string RUTA_FIRMA_ELECTRONICA = "";
        public static string RUTA_XML_GENERADOS = "";
        public static string EMPRESA_GUID = "";
        public static string URL_WS = "";
        public static string USUARIO_WS = "";
        public static string CLAVE_WS = "";
        public static string CLAVE_FIRMA_ELECTRONICA = "";
        public static string NUMERO_FACTURERO = "001";
        public static string FORMA_PAGO_SERVICIO = "P"; // P = PORCENTAJE, V = VALOR 
        public static string TIPO_IMPRESORAS = "I";
        public static string DOCUMENTO_CREAR_FACTURAR = "F";
        public static string PC_NAME = "NO DEFINIDA";
        public static string IP_ADDRESS = "127.0.0.1";
        public static string URL_SITIO_WEB = "";
        public static string TOKEN_MAIL = "";
        public static string MODO_VALIDACION_ANULA_ELIMINA = "N";
        public static string TIPO_IMPRESORA_FACT = "";
        public static string TIPO_IMPRESORA_COCINA = "";
        public static string TIPO_IMPRESORA_PARRILLA = "";
        public static string TIPO_IMPRESORA_BAR = "";
        public static string MARCA_IMPRESORA_FACT = "";
        public static string MARCA_IMPRESORA_COCINA = "";
        public static string MARCA_IMPRESORA_PARRILLA = "";
        public static string MARCA_IMPRESORA_BAR = "";
        public static string FORMA_ORDEN_ITEMS_GRPS = "C";
        public static string URL_MAIL_RELAY = "";
        public static string TOKEN_WHATSAPP = "";
        public static string URL_WHATSAPP = "";
        public static string TOKEN_SMS = "";
        public static string URL_SMS = "";
        public static string IMAGEN_FONDO_SELECC = "1";
        public static string FONDO_TEMA_LOGIN = "0";
        public static string PRODUCT_CODE = "";
        public static string RUC_DEVELOP_EMPRESA = "";

        public static int TEMA_REPORTE = -1;
        public static int CANTIDAD_MAXIMA_ARTICULOS_FACTURAR = 20;
        public static int CANTIDAD_PROVINCIAS = 24;
        public static int SEGUNDOS_ESPERA = 0;
        public static int CANTIDAD_MINIMA_STOCK = 0;

        public static double VALOR_PAGO_SERVICIO = 0;
        
        public static double IVA = 12;
        public static double IVA_FERIADO = 12;
        public static double IVA_COMPRA = 12;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static string  getCurrentAppPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void generaConnectionString(Config config)
        {
            connectionString += "server=" + config.database.Server + ";";
            connectionString += "port=" + config.database.Puerto + ";";
            connectionString += "database=" + config.database.DbName + ";";
            connectionString += "user id=" + config.database.Usuario + ";";
            connectionString += "password=" + config.database.Clave + ";";
        }

        public static void generaPrintersConfig(Config config)
        {
            General.USA_TECLADO_PANTALLA = config.system.UsaTouch;
            General.PRINTER_COCINA=config.system.PrinterCocina;
            General.PRINTER_FACTURA = config.system.PrinterFactura;
            General.PRINTER_PROFORMA = config.system.PrinterProforma;
            General.PRINTER_PARRILLA = config.system.PrinterParrilla;
            General.PRINTER_BAR = config.system.PrinterBar;
            General.CAJA = config.system.Caja;
        }

        public static void loadWindow(UserControl objeto, Grid contenido)
        {
            var ctrl = objeto;
            contenido.Children.Clear();
            contenido.Children.Add(ctrl);
        }

        public static Boolean parseInt(String valor)
        {
            try
            {
                int value = int.Parse(valor);
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public static Boolean validaEmail(string valor)
        {
            String expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if( Regex.IsMatch(valor, expresion))
            {
                if(Regex.Replace(valor, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void selectedByTagCombo(ComboBox cbo, string tag)
        {
            foreach (ComboBoxItem itm in cbo.Items)
            {
                if(itm != null)
                {
                    if (itm.Tag.Equals(tag))
                    {
                        cbo.SelectedValue = itm;
                    }
                }
            }

        }

        public static void selectedByContentCombo(ComboBox cbo, string content)
        {
            foreach (object itm in cbo.Items)
            {
                if (itm != null && itm.GetType() == typeof(ComboBoxItem))
                {
                    if (((ComboBoxItem)itm).Content.ToString() == content)
                    {
                        cbo.SelectedValue = itm;
                    }
                }

                if (itm != null && itm.GetType() == typeof(string))
                {
                    if (itm.ToString() == content)
                    {
                        cbo.SelectedValue = itm;
                    }
                }
            }
        }

        public static String getImageFromFile(System.Windows.Controls.Image img)
        {
            String _img = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selección de Imágenes";
            openFileDialog.Filter = "Imagenes (*.png;*.jpeg)|*.png;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                _img = openFileDialog.FileName;
                img.Source = new BitmapImage(new Uri(_img));
            }

            return _img;
        }

        public static byte[] bitmapImageToBytes(String archivo)
        {
            if (!string.IsNullOrEmpty(archivo))
            {
                byte[] _img = null;

                FileStream img = new FileStream(archivo, FileMode.Open, FileAccess.Read);

                using (BinaryReader br = new BinaryReader(new BufferedStream(img)))
                {
                    _img = br.ReadBytes(Convert.ToInt32(img.Length));
                }

                img.Close();

                return _img;
            }
            return null;
        }

        public static BitmapImage BtyeArrayToBitmapImage(byte[] _img)
        {
            using (MemoryStream ms = new MemoryStream(_img))
            {
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = null;
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();

                return bi;
            }
        }

        public static string ImageToBase64(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            else
            {
                return Convert.ToBase64String(data);
            }
        }

        public static byte[] base64ToByteArray(string data)
        {
            return Convert.FromBase64String(data);
        }

        public static DateTime GetPrimeraDiaFecha()
        {
            DateTime actual = DateTime.Now;

            int mes = actual.Month;
            int anio = actual.Year;

            return new DateTime(anio, mes, 1);
        }

        public static DateTime GetUltimoDiaFecha()
        {
            DateTime primero = GetPrimeraDiaFecha();

            return primero.AddMonths(1).AddDays(-1);
        }

        public static DataTable reOrdena(DataTable dt, string columna, string direccion)
        {
            DataTable dtOut = null;
            dt.DefaultView.Sort = columna + " " + direccion;
            dtOut = dt.DefaultView.ToTable();

            return dtOut;
        }

        public static void fillComboBoxFromDataTable(ComboBox cbo, DataTable dt, String incluir, bool combinaCodDsc = false)
        {
            if (cbo == null) return;
            cbo.Items.Clear();

            cbo.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

            string textoDefault = incluir switch
            {
                "T" => "-- Todos --",
                "S" => "-- Seleccione --",
                "N" => "-- Ninguno --",
                _ => null
            };

            if (textoDefault != null)
            {
                cbo.Items.Add(new ComboBoxItem
                {
                    Tag = "",
                    Content = textoDefault,
                    IsSelected = true,
                    Height = 40
                });
            }

            if (dt == null || dt.Rows.Count == 0) return;

            foreach (DataRow dr in dt.Rows)
            {
                string codigo = dr["codigo"]?.ToString() ?? string.Empty;
                string descripcion = dr["descripcion"]?.ToString() ?? string.Empty;
                string contenido = combinaCodDsc ? $"({codigo}) {descripcion}" : descripcion;

                cbo.Items.Add(new ComboBoxItem
                {
                    Tag = codigo,
                    Content = contenido,
                    Height = 40
                });
            }
        }

        public static string getTagFromComboBox(ComboBox cbo)
        {
            string valor = "";
            if (cbo.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                valor = itemSeleccionado.Tag?.ToString();
            }
            
            return valor;
        }

        public static String sbDefineImagen(string valor)
        {
            return valor switch
            {
                "L" => "pack://application:,,,/images/toserv.png",
                "D" => "pack://application:,,,/images/delivery.png",
                "C" => "pack://application:,,,/images/tocarry.png",
                "E" => "pack://application:,,,/images/uber.png",
                "R" => "pack://application:,,,/images/clock-color.png",
                _ => string.Empty // Valor por defecto si no coincide ninguno
            };
        }

        public static String StringFormatMoney(double valor)
        {
            valor = Math.Round(valor, 6, MidpointRounding.ToEven);
            valor = Math.Round(valor, 5, MidpointRounding.ToEven);
            valor = Math.Round(valor, 4, MidpointRounding.ToEven);
            valor = Math.Round(valor, 3, MidpointRounding.ToEven);
            valor = Math.Round(valor, 2, MidpointRounding.ToEven);

            return String.Format("{0:0.00}", valor);
        }

        public static string contraseniaDownload(int longitudContrasenia)
        {
            if (longitudContrasenia <= 0) return string.Empty;

            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.+-!:;%$#@";

            return string.Create(longitudContrasenia, caracteres, (span, alphabet) =>
            {
                RandomNumberGenerator.GetItems(alphabet, span);
            });
        }

        public static void loadParametrosBase()
        {
            Parametros parametros = new Parametros(General.connectionString);
            DataTable dt = parametros.listar();

            foreach (DataRow dr in dt.Rows)
            {
                string valor = dr["valor"].ToString();

                switch ((int)dr["codigo"])
                {
                    case 2:
                        // Definición del valor del iva
                        General.IVA = General.StringToDouble(valor);
                        break;
                    case 25:
                        General.IMPRIME_HORA_COCINA = int.Parse(valor);
                        break;
                    case 27:
                        General.TICKET_COCINA_CANTIDAD_IZQ = valor.Equals("1");
                        break;
                    case 47:
                        General.ENVIA_IMPRESORA_PARRILLA = valor.Equals("1");
                        break;
                    case 49:
                        General.ENVIA_IMPRESORA_BAR = valor.Equals("1");
                        break;
                    case 95:
                        General.TIPO_IMPRESORAS = valor;
                        break;
                    case 96:
                        General.IMPRIME_CABECERA_COMANDAS = valor.Equals("1");
                        break;
                    case 164:
                        General.SOLO_VENTAS_LOCALES = valor.Equals("1");
                        break;
                    case 165:
                        General.IVA_FERIADO = General.StringToDouble(valor);
                        break;
                    case 166:
                        General.CONFIRMA_RECEPTOR = valor.Equals("1");
                        break;
                    case 167:
                        General.CANTIDAD_MINIMA_STOCK = int.Parse(valor);
                        break;
                    case 168:
                        General.INCLUYE_PRECIOS_COMPRAS = valor.Equals("1");
                        break;
                    case 169:
                        General.RUC_DEVELOP_EMPRESA = valor;
                        break;
                }
            }
        }

        public static void loadParametros()
        {
            PcNetworkInfo pcInfo = new PcNetworkInfo();
            General.PC_NAME = pcInfo.PcName;
            General.IP_ADDRESS = pcInfo.IpAddress;
            string numeroCrd = "";
            string validadorCrd = "";

            Parametros parametros = new Parametros(General.connectionString);
            DataTable dt = parametros.listar();

            // 1. Consulta WMI optimizada con LINQ de alto rendimiento (Elimina el bucle foreach manual)
            using (var searcher = new ManagementObjectSearcher("select SerialNumber from Win32_BaseBoard"))
            {
                var board = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                General.MAINBOARD_SERIAL = board?.GetPropertyValue("SerialNumber")?.ToString() ?? "";
            }

            // 2. Diccionario de Mapeo de Acciones (Asociación directa Código -> Variable Global)
            // Cada acción se ejecuta de manera segura protegiendo conversiones numéricas y booleanas
            var mapaParametros = new Dictionary<int, Action<string>>
            {
                { 2,  val => General.IVA = General.StringToDouble(val) },
                { 4,  val => General.CANTIDAD_MESAS = ParseInt(val) },
                { 5,  val => General.ENVIA_CORREO_POR_ELIMINACION = val == "1" }, // Simplificación directa a Booleano
                { 6,  val => General.CORREO_HOST = val },
                { 7,  val => General.CORREO_PORT = val },
                { 8,  val => General.CORREO_USER = val },
                { 9,  val => General.CORREO_PASSWORD = val },
                { 10, val => General.CORREO_AUTH = val },
                { 11, val => General.CORREO_UTLS = val },
                { 14, val => General.CORREO1 = val },
                { 15, val => General.COPIAR_CORREOS = val == "1" },
                { 16, val => General.CORREO2 = val },
                { 17, val => General.CORREO3 = val },
                { 18, val => General.CORREO4 = val },
                { 20, val => General.USA_CLAVE_NUMERICA = val == "1" },
                { 22, val => General.TEMA_REPORTE = ParseInt(val) },
                { 23, val => General.CAMBIO_RAPIDO = ParseInt(val) },
                { 25, val => General.IMPRIME_HORA_COCINA = ParseInt(val) },
                { 26, val => General.CANTIDAD_PROVINCIAS = ParseInt(val) },
                { 27, val => General.TICKET_COCINA_CANTIDAD_IZQ = val == "1" },
                { 28, val => General.IMPRIME_TICKET_CARRIER_AUT = val == "1" },
                { 32, val => General.CANTIDAD_LINEAS_IMPRESION = ParseInt(val) },
                { 39, val => General.MODO_PEDIDO_FACTURACION = val },
                { 40, val => General.TIPO_BOTONES = val },
                { 45, val => General.ORGANIZA_ITEMS_PEDIDO = val },
                { 46, val => General.USA_IMPRESORA_PROFORMA = val == "1" },
                { 47, val => General.ENVIA_IMPRESORA_PARRILLA = val == "1" },
                { 48, val => General.FORMATO_NUMERICO_REPORTE = val },
                { 49, val => General.ENVIA_IMPRESORA_BAR = val == "1" },
                { 50, val => General.CANTIDAD_MESAS_ALTERNA = ParseInt(val) },
                { 51, val => General.CANTIDAD_MESAS_EXTERIOR = ParseInt(val) },
                { 52, val => General.CANTIDAD_MESAS_TERRAZA = ParseInt(val) },
                { 53, val => General.CANTIDAD_MESAS_2DO_PISO = ParseInt(val) },
                { 54, val => General.USA_FACTURACION_ELECTRONICA = val == "1" },
                { 55, val => General.ENVIO_AUTO_AL_SRI = val == "1" },
                { 56, val => General.ENVIO_ONLINE = val == "1" },
                { 57, val => General.RUTA_FIRMA_ELECTRONICA = val },
                { 60, val => General.EMPRESA_GUID = val },
                { 61, val => General.URL_WS = val },
                { 62, val => General.USUARIO_WS = val },
                { 63, val => General.CLAVE_WS = val },
                { 64, val => General.CLAVE_FIRMA_ELECTRONICA = val },
                { 65, val => General.RUTA_XML_GENERADOS = val },
                { 67, val => General.NUMERO_FACTURERO = !string.IsNullOrEmpty(val) ? val : "001" }, // Reemplazo de ??
                { 68, val => General.ES_TIPO_RIMPE = val == "1" },
                { 69, val => General.TAMANIO_BOTONES_PEDD = val == "1" ? TamanioBotonesMenu.Normal : TamanioBotonesMenu.Small },
                { 70, val => General.TAMANIO_BOTONES_FACT = val == "1" ? TamanioBotonesMenu.Normal : TamanioBotonesMenu.Small },
                { 72, val =>
                    {
                        General.VALOR_PAGO_SERVICIO = General.StringToDouble(val);
                        General.AGREGA_SERVICIO = General.VALOR_PAGO_SERVICIO > 0;
                    }
                },
                { 73,  val => General.FORMA_PAGO_SERVICIO = val },
                // Nota: Códigos 74 al 77 eliminados por estar comentados/vacíos en el switch original
                { 78,  val => General.SEPARADOR = val },
                { 79,  val => General.CANTIDAD_COLUMNAS_IMPRESION = ParseInt(val) },
                { 80,  val => General.IMPRIME_NOMBRE_EMPRESA = val == "1" },
                { 81,  val => General.IMPRIME_DIRECCION_EMPRESA = val == "1" },
                { 82,  val => General.IMPRIME_RUC_EMPRESA = val == "1" },
                { 83,  val => General.NEGRITA_EN_CABECERA = val == "1" },
                { 84,  val => General.ALINEACION_CABECERA = val },
                { 85,  val => General.IMPRIME_SEPARADO_FIN_CABECERA = val == "1" },
                { 86,  val => General.IMPRIME_NEGRITA_CABECERA_CONTENIDO = val == "1" },
                { 87,  val => General.CANTIDAD_LINEAS_FINAL_ITEMS = ParseInt(val) },
                { 88,  val => General.IMPRIME_SEPARADOR_FIN_ITEMS = val == "1" },
                { 89,  val => General.CANTIDAD_DECIMALES_EN_ITEMS = ParseInt(val) },
                { 90,  val => General.IMPRIME_FORMA_PAGO = val == "1" },
                { 91,  val => General.IMPRIME_NOMBRE_MESERO = val == "1" },
                { 92,  val => General.IMPRIME_AGRADECIMIENTO = val == "1" },
                { 93,  val => General.IMPRIME_TICKET_CIERRE_CAJA = val == "1" },
                { 94,  val => General.VALIDA_COMPRA_RETENCION = val == "1" },
                { 95,  val => General.TIPO_IMPRESORAS = val },
                { 96,  val => General.IMPRIME_CABECERA_COMANDAS = val == "1" },
                { 97,  val => General.CANTIDAD_COLUMNAS_GRUPOS = ParseInt(val) },
                { 98,  val => General.DOCUMENTO_CREAR_FACTURAR = val },
                { 103, val => General.SALTOLINEA_CABECERA_COMANDAS = val == "1" },
                { 108, val => General.IMPRIME_CLAVE_DOWNLOAD = val == "1" },
                { 109, val => General.URL_SITIO_WEB = val },
                { 110, val => General.IMPRIME_RETENCION = val == "1" },
                { 111, val => General.USA_API_MAIL = val == "1" },
                { 112, val => General.TOKEN_MAIL = val },
                { 119, val => General.MUESTRA_ELIMINADOS_ANULADOS = val == "1" },
                { 120, val => General.USA_ESPACIOS_INTERLINEADO = val == "1" },
                { 121, val => General.TIPO_IMPRESORA_FACT = val },
                { 122, val => General.TIPO_IMPRESORA_COCINA = val },
                { 123, val => General.TIPO_IMPRESORA_PARRILLA = val },
                { 124, val => General.TIPO_IMPRESORA_BAR = val },
                { 125, val => General.MARCA_IMPRESORA_FACT = val },
                { 126, val => General.MARCA_IMPRESORA_COCINA = val },
                { 127, val => General.MARCA_IMPRESORA_PARRILLA = val },
                { 128, val => General.MARCA_IMPRESORA_BAR = val },
                { 129, val => General.USA_ESPACIO_INTERLINEADO_FACT = val == "1" },
                { 130, val => General.USA_ESPACIO_INTERLINEADO_COCINA = val == "1" },
                { 131, val => General.USA_ESPACIO_INTERLINEADO_PARRILLA = val == "1" },
                { 132, val => General.USA_ESPACIO_INTERLINEADO_BAR = val == "1" },
                { 133, val => General.CANTIDAD_COPIAS_FACTURA = ParseInt(val) },
                { 134, val => General.MODO_EXPANDIDO_CABECERA_FACT = val == "1" },
                { 138, val => General.IMPRIME_COMANDAS_MARKET = val == "1" },
                { 139, val => General.MUESTRA_MESAS_MARKET = val == "1" },
                { 140, val => General.PERMITE_DAR_CREDITO = val == "1" },
                { 141, val => General.FORMA_ORDEN_ITEMS_GRPS = val },
                { 143, val => General.MODO_VALIDACION_ANULA_ELIMINA = string.IsNullOrEmpty(val) ? "N" : val },
                { 144, val => General.MUESTRA_CLIENTE_LLEVAR = val == "1" },
                { 145, val => General.IMPRIME_CLIENTE_COMANDA = val == "1" },
                { 146, val => General.PERMITE_USO_BARCODE = val == "1" },
                { 147, val => General.PERMITE_BUSCAR_ITEMS = val == "1" },
                { 148, val => General.CLICK_GRUPO_BUSCAR_ITEMS = val == "1" },
                { 149, val => General.URL_MAIL_RELAY = val },
                { 150, val => General.USA_SMS = val == "1" },
                { 151, val => General.TOKEN_WHATSAPP = val },
                { 152, val => General.URL_WHATSAPP = val },
                { 153, val => General.TOKEN_SMS = val },
                { 154, val => General.URL_SMS = val },
                { 155, val => General.USA_WHATSAPP = val == "1" },
                { 156, val => General.USA_MAIL_RELAY = val == "1" },
                { 157, val => General.USA_COORD_ALTERNO = val == "1" },
                { 158, val => General.SEGUNDOS_ESPERA = ParseInt(val) },
                // El caso 159 se omite al estar vacío en tu switch original
                { 160, val => General.IMPRIME_SECUENCIAL_TEMPORAL = val == "1" },
                // Los casos 161 y 162 se omiten al estar vacíos en tu switch original
                { 163, val => General.FONDO_TEMA_LOGIN = val },
                { 164, val => General.SOLO_VENTAS_LOCALES = val == "1" },
                { 165, val => General.IVA_FERIADO = double.TryParse(val, out double iva) ? iva : 0.0 }, // Parseo decimal protegido
                { 166, val => General.CONFIRMA_RECEPTOR = val == "1" },
                { 167, val => General.CANTIDAD_MINIMA_STOCK = int.TryParse(val, out int stockmin) ? stockmin: 0 },
                { 168, val => General.INCLUYE_PRECIOS_COMPRAS = val == "1" },
                { 169, val => General.RUC_DEVELOP_EMPRESA = val },

                // === SERIE DE CONFIGURACIÓN DE EMPRESA (9900) ===
                { 9989, val => General.USA_RETENCIONES = val == "1" },
                { 9990, val => General.LLEVA_CONTABILIDAD = val == "1" },
                { 9991, val => General.CODIGO_EMPRESA = val },
                { 9992, val => General.DIRECCION_LOCAL = val },
                { 9993, val => General.DIRECCION_MATRIZ = val },
                { 9994, val => General.RAZON_SOCIAL = val },
                { 9995, val => General.LOGO_EMPRESA = val },
                { 9996, val => numeroCrd = val },       // Asignación directa a variable local del método
                { 9997, val => validadorCrd = val },    // Asignación directa a variable local del método
                { 9998, val => General.RUC_EMPRESA = val },
                { 9999, val => General.EMPRESA = val }
            };

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["codigo"] != DBNull.Value && mapaParametros.TryGetValue((int)dr["codigo"], out var asignarParametro))
                {
                    // Pasamos el string limpio evitando excepciones por nulos
                    string valorCelda = dr["valor"]?.ToString() ?? "";
                    asignarParametro(valorCelda);
                }
            }


            AlgoritmosValidacion validacion = new AlgoritmosValidacion();
            General.SISTEMA_VALIDADO = validacion.verificaValidadorCard(numeroCrd, General.RUC_EMPRESA, validadorCrd);

            // 2. Inicialización y refresco del hardware
            var PcID = new Hardware.Info.HardwareInfo();
            PcID.RefreshCPUList();
            PcID.RefreshMotherboardList();

            // 3. Extracción protegida contra nulos (Evita cierres inesperados si las listas vienen vacías)
            var cpu = PcID.CpuList?.FirstOrDefault();
            var motherboard = PcID.MotherboardList?.FirstOrDefault();

            string cpuId = cpu?.ProcessorId ?? "FALLBACK_CPU";
            string boardSerial = motherboard?.SerialNumber ?? "FALLBACK_BOARD";

            // 4. Generación y formateo del código de producto
            PRODUCT_CODE = FormatInGroups(cpuId + boardSerial, 4);

            // 5. Validación de la licencia mediante criptografía
            CryptLicense cript = new CryptLicense();
            LicenseInfo lic = cript.validar(PRODUCT_CODE);

            // 6. Asignación del estado de registro usando lógica condicional inversa y limpia
            if (lic != null && !lic.error)
            {
                General.SISTEMA_REGISTRADO = true;
            }
            else
            {
                General.SISTEMA_REGISTRADO = false; // Asegura limpiar el estado si la licencia expiró o falló
                string mensajeError = lic?.errorMessage ?? "Error desconocido al validar la licencia del sistema.";
                ObjComportamiento.showMsgError(mensajeError);
            }

            int ParseInt(string input)
            {
                return int.TryParse(input, out int resultado) ? resultado : 0;
            }
        }
     
        public static System.Windows.Forms.Screen getCurrentScreen(System.Windows.Window window = null)
        {
            System.Windows.Interop.WindowInteropHelper interopHelper = null;
            if (window != null)
            {
                interopHelper = new System.Windows.Interop.WindowInteropHelper(window);
            }
            else {
                interopHelper = new System.Windows.Interop.WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            }
            
            var activeScreen = System.Windows.Forms.Screen.FromHandle(interopHelper.Handle);

            return activeScreen;
        }   

        public static double getCurrentScreenWidth(System.Windows.Window window = null)
        {
            System.Windows.Forms.Screen screen = General.getCurrentScreen(window);
            return screen.WorkingArea.Width;
        }

        public static double getCurrentScreeResolution()
        {
            return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        }

        public static double StringToDouble(string value)
        {
            value = value ?? "0";
            value = value.Replace(",", ".").Trim();
            value = value == "" ? "0" : value;
            var culture = new System.Globalization.CultureInfo("es-EC");
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            double number = Convert.ToSingle(value, culture);
            return number;
        }

        public static (string Data, string RawTrimmed) GetXmlParaEnvio(string strFile)
        {
            string rawTrimmed = System.IO.File.ReadAllText(strFile).Trim();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawTrimmed);

            string data;
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { OmitXmlDeclaration = false }))
                {
                    xmlDoc.WriteTo(xw);
                }
                data = sw.ToString();
            }

            return (data, rawTrimmed);
        }

        public static int generaNumeroAleatorio()
        {
            var guid = Guid.NewGuid();
            var justNumbers = new String(guid.ToString().Where(Char.IsDigit).ToArray());
            var seed = int.Parse(justNumbers.Substring(0, 4));

            var numero = new Random(seed).Next(0, 10);

            return numero;
        }

        public static string generaPin()
        {
            string resultado = "";
            for(int i=0; i<6; i++)
            {
                resultado += General.generaNumeroAleatorio().ToString();
            }

            return resultado;
        }


        public static void enviarWhatsapp(string mensaje, string destino)
        {
            HttpClienteAPI client = new HttpClienteAPI();

            client.TrsklUrl = General.URL_WHATSAPP;

            WhatsappModel wa = new WhatsappModel
            {
                ruc = General.RUC_EMPRESA,
                empresaId = General.EMPRESA_GUID,
                token = General.TOKEN_WHATSAPP,
                mensaje = mensaje,
                destino = destino
            };

            CryptUtil crypUtil = new CryptUtil();
            string msgAuth = JsonConvert.SerializeObject(wa);
            var valor = crypUtil.Encriptar(msgAuth);

            client.sendWhatsapp(valor).Wait();
        }

        public static void enviarCorreo(string usuario, string titulo, string mensaje, usaMailTipo tipo, usaTipoAccion accion, string secuencial, string msgauth, string responsable = "")
        {
            if (General.ENVIA_CORREO_POR_ELIMINACION)
            {
                string msg = "";

                string tipoProceso = "";

                if (tipo == usaMailTipo.Factura)
                {
                    tipoProceso = "VENTA";
                }
                else
                {
                    tipoProceso = "PEDIDO";
                }


                if (accion == usaTipoAccion.Anulacion)
                {
                    msg = "En " + General.EMPRESA + " el usuario " + usuario + (responsable != "" ? "(" +responsable + ") " : "") +
                          " ha realizado una anulación de un(a) " + tipoProceso + " con secuencial " + secuencial +
                          " por motivo : " + ((char)13) + mensaje + ((char)13) + ((char)13) +
                          "Mensaje generado por el sistema TreskelRest.";
                }
                else
                {
                    msg = "En " + General.EMPRESA + " el usuario " + usuario + (responsable != "" ? "(" + responsable + ") " : "") +
                          " ha realizado una eliminación de un(a) " + tipoProceso + " con secuencial " + secuencial +
                          " por motivo : " + ((char)13) + mensaje + ((char)13) + ((char)13) +
                          "Mensaje generado por el sistema TreskelRest.";
                }

                if (General.USA_MAIL_RELAY)
                {
                    CryptUtil crypUtil = new CryptUtil();
                    var valor = crypUtil.Encriptar(msgauth);

                    HttpClienteAPI api = new HttpClienteAPI();
                    api.TrsklUrl = General.URL_MAIL_RELAY;

                    List<string> destinatarios = new List<string>();

                    if (!string.IsNullOrEmpty(General.CORREO1)) destinatarios.Add(General.CORREO1);
                    if (General.COPIAR_CORREOS)
                    {
                        if (!string.IsNullOrEmpty(General.CORREO2)) destinatarios.Add(General.CORREO2);
                        if (!string.IsNullOrEmpty(General.CORREO3)) destinatarios.Add(General.CORREO3);
                        if (!string.IsNullOrEmpty(General.CORREO4)) destinatarios.Add(General.CORREO4);
                    }

                    if (destinatarios.Count > 0)
                    {
                        api.sendMail(valor,
                                 destinatarios,
                                 titulo,
                                 msg).GetAwaiter();
                    }
                }
                else
                {
                    Email correo = new Email(General.CORREO_HOST, General.CORREO_PORT, General.CORREO_USER,
                                             General.CORREO_PASSWORD, General.CORREO_AUTH, General.CORREO_UTLS,
                                             General.SUCURSAL);
                    correo.subject = titulo;
                    correo.body = mensaje;

                    Thread hiloCorreo = new Thread(new ThreadStart(correo.enviar));
                    hiloCorreo.Start();
                }
            }
        }

        public static void retornaLimites(int porcentaje, out int limite)
        {
            decimal lim1 = (General.SEGUNDOS_ESPERA * porcentaje) / 100;
            
            decimal numero = lim1 / 60;
            var entero = decimal.Truncate(numero);
            var fraccion = numero - entero;

            limite = ((int)entero) * 100 + (int)(fraccion * 60);
        }

        public static int generaNumeroAleatorio(int inicio, int fin)
        {
            var guid = Guid.NewGuid();
            var justNumbers = new String(guid.ToString().Where(Char.IsDigit).ToArray());
            var seed = int.Parse(justNumbers.Substring(0, 4));

            var random = new Random(seed);
            return random.Next(inicio, fin);
        }

        public class MesDescripcion
        {
            public string descripcion;
            public string acronimo;

            public MesDescripcion(string descripcion, string acronimo)
            {
                this.descripcion = descripcion;
                this.acronimo = acronimo;
            }
        }

        public static string obtnieneMesPorNumero(int valor, bool acronimo = false)
        {
            var mes = new Dictionary<string, MesDescripcion>
            {
                { "1",  new MesDescripcion("Enero", "E") },
                { "2",  new MesDescripcion("Febrero", "F") },
                { "3",  new MesDescripcion("Marzo", "M") },
                { "4",  new MesDescripcion("Abril", "A") },
                { "5",  new MesDescripcion("Mayo", "Y") },
                { "6",  new MesDescripcion("Junio", "J") },
                { "7",  new MesDescripcion("Julio", "L") },
                { "8",  new MesDescripcion("Agosto", "T") },
                { "9",  new MesDescripcion("Septiembre", "S") },
                { "10", new MesDescripcion("Octubre", "O") },
                { "11", new MesDescripcion("Noviembre", "N") },
                { "12", new MesDescripcion("Diciembre", "D") }
            };

            if (mes.TryGetValue(valor.ToString(), out MesDescripcion seleccion))
            {
                return acronimo ? seleccion.acronimo : seleccion.descripcion;
            }

            return null;
        }

        public static Image<Rgba32> ConvertBitmapImageToImageSharp(string imageName)
        {
            var uri = new Uri($"pack://application:,,,/images/{imageName}");
            var resourceStreamInfo = System.Windows.Application.GetResourceStream(uri);

            if (resourceStreamInfo != null)
            {
                using (Stream stream = resourceStreamInfo.Stream)
                using (var image = ImageSharpImage.Load(stream))
                {
                    return image.CloneAs<Rgba32>();
                }
            }

            return null;

        }

        public static string FormatInGroups(string inputText, int cantChars = 4)
        {
            if (string.IsNullOrEmpty(inputText) || cantChars <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            int length = inputText.Length;

            for (int i = 0; i < length; i += cantChars)
            {
                int caracteresRestantes = Math.Min(cantChars, length - i);
                sb.Append(inputText.Substring(i, caracteresRestantes));
                if (i + cantChars < length)
                {
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }
    }
}

using ClosedXML.Excel;
using Entidades;
using LibReports.modelo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace LibReports
{
    public class ExcelBase
    {        
        public static Dictionary<string, string> x = new Dictionary<string, string>()
        {
            {"color1","#F1DCDB"},
            {"color2","#CB630D"},
            {"color3","#ABCB0D"},
            {"color4","#63CB0D"},
            {"color5","#459007"},
            {"color6","#079071"},
            {"color7","#077B90"},
            {"color8","#083887"},
            {"color9","#606E87"},
            {"color10","#706087"},
            {"color11","#BF49BF"},
            {"color12","#BF4972"}
        };

        public List<themeExcel> tema = new List<themeExcel>();


        private int temaSeleccionado = 0;

        protected Empresa emp;
        public string NombreEmpresa;
        public string FormatoNumerico = "###.##0,00";

        protected String connectionString = "";
        protected XLWorkbook wb = new XLWorkbook();
        protected IXLWorksheet ws;
        protected IXLWorksheet wsp;

        public Boolean Instalado = false;

        public string generarFileName(string prefijo)
        {
            string archivo = "";

            DateTime fecha = DateTime.Now;
            var anio = fecha.Year;
            var mes = fecha.Month.ToString().PadLeft(2, '0');
            var dia = fecha.Day.ToString().PadLeft(2, '0');
            var hora = fecha.Hour.ToString().PadLeft(2, '0');
            var minuto = fecha.Minute.ToString().PadLeft(2, '0');

            archivo = prefijo + "_" + anio + mes + dia + "_" + hora + minuto;

            return archivo + ".xlsx";
        }

        private void cargaTemas()
        {
            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#FFFFFF",
                colorLetrasCabecera = "#000000",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#FFFFFF",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#C71B17",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2= "#FDE9D9",
                colorLetrasContenido= "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#E26B0A",
                colorLetrasCabecera = "#FFFFFF",                
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#FCD9BC",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#244062",
                colorLetrasCabecera = "#FFFFFF",                
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#DCE6F1",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#4F6228",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#D8E4BC",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#403151",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#E4DFEC",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#00B050",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#C0FD77",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#00B0F0",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#E1FEAC",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#C03078",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#F7EF89",
                colorLetrasContenido = "#000000"
            });

            tema.Add(new themeExcel
            {
                colorFondoCabecera = "#0D0D0D",
                colorLetrasCabecera = "#FFFFFF",
                colorFondoContenido1 = "#FFFFFF",
                colorFondoContenido2 = "#D9D9D9",
                colorLetrasContenido = "#000000"
            });            
        }

        public themeExcel getTema()
        {
            return this.tema[this.temaSeleccionado];
        }

        public void setEmpresaTitulo(String nameSheet, String titulo, int colFinal)
        {
            ws.Name = nameSheet;
            ws.Cell(1, 1).Value = this.NombreEmpresa;//empresa;
            ws.Cell(1, 1).Style.Font.FontSize = 16;

            ws.Range(1, 1, 1, colFinal).Merge();
            ws.Range(1, 1, 1, colFinal).Style.Font.Bold = true;
            ws.Range(1, 1, 1, colFinal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(3, 1).Value = titulo;
            ws.Cell(3, 1).Style.Font.FontSize = 16;
            ws.Range(3, 1, 3, colFinal).Merge();
            ws.Range(3, 1, 3, colFinal).Style.Font.Bold = true;
            ws.Range(3, 1, 3, colFinal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        public Color setColorFromHtml(string color)
        {
            ColorConverter cc = new ColorConverter();

            return (Color)cc.ConvertFromString(color); 
        }

        protected double CentimetersToPoints(double centimetro)
        {
            return centimetro * 28.34;
        }

        protected double CentimetersToTwips(double centimeto)
        {
            return centimeto * 566.93;
        }

        public ExcelBase(String connectionString, int tema)
        {
            // Se definen los colores del tema seleccionado.
            this.connectionString = connectionString;
            this.emp= new Empresa(this.connectionString);
            this.cargaTemas();

            this.connectionString = connectionString;
            //excel = new Microsoft.Office.Interop.Excel.Application();

            this.temaSeleccionado = tema + 1;

            if (IsExcelInstalled())
            {
                this.Instalado = true;

                //this.wb = this.excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                this.wb = new XLWorkbook();
                this.ws = this.wb.Worksheets.Add();

                //this.ws.PageSetup.PaperSize = XlPaperSize.xlPaperA4;
                defineMargenes();
                
                return;
            }                        
        }    

        public void defineMargenes()
        {
            this.ws.PageSetup.Margins.SetLeft(CentimetersToPoints(2));
            this.ws.PageSetup.Margins.SetRight(CentimetersToPoints(1));
            this.ws.PageSetup.Margins.SetTop(CentimetersToPoints(2));
            this.ws.PageSetup.Margins.SetBottom(CentimetersToPoints(2));
        }

        public void creaNuevoSheet(string nameSheet, string titulo, int colFinal)
        {
            wsp = this.wb.Worksheets.Add();
            defineMargenes();

            wsp.Name = nameSheet;
            wsp.Cell(1, 1).Value = this.NombreEmpresa;//empresa;
            wsp.Row(1).Style.Font.FontSize = 16;

            wsp.Range(1, 1, 1, colFinal).Merge();
            wsp.Range(1, 1, 1, colFinal).Style.Font.Bold = true;
            wsp.Range(1, 1, 1, colFinal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wsp.Cell(3, 1).Value = titulo;
            wsp.Row(3).Style.Font.FontSize = 16;
            wsp.Range(3, 1, 3, colFinal).Merge();
            wsp.Range(3, 1, 3, colFinal).Style.Font.Bold = true;
            wsp.Range(3, 1, 3, colFinal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
        
        public void setBorders(string range)
        {
            this.ws.Range(range).Style.Border.SetLeftBorder(XLBorderStyleValues.Medium);
            this.ws.Range(range).Style.Border.SetRightBorder(XLBorderStyleValues.Medium);
            this.ws.Range(range).Style.Border.SetTopBorder(XLBorderStyleValues.Medium);
            this.ws.Range(range).Style.Border.SetBottomBorder(XLBorderStyleValues.Medium);

            this.ws.Range(range).Style.Border.SetLeftBorderColor(XLColor.Black);
            this.ws.Range(range).Style.Border.SetRightBorderColor(XLColor.Black);
            this.ws.Range(range).Style.Border.SetTopBorderColor(XLColor.Black);
            this.ws.Range(range).Style.Border.SetBottomBorderColor(XLColor.Black);

        }

        public void setColoresContenido(int i, int colFinal, themeExcel thema)
        {
            if(i % 2 == 1)
            {
                ws.Range(i, 1, i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoContenido1);
                ws.Range(i, 1, i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasContenido);
            }
            else
            {
                ws.Range(i, 1, i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoContenido2);
                ws.Range(i, 1, i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasContenido);
            }
        }

        public void setColoresContenido(int i, int rowfinal, int colFinal, themeExcel thema)
        {
            int cantidad = rowfinal - i;

            for (var v = i + 1; v <= rowfinal; v += 2)
            {
                ws.Range(v, 1, v, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoContenido2);
                ws.Range(v, 1, v, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasContenido);
            }

        }

        public void setFormatCabecera(int i, themeExcel thema, int colFinal)
        {
            ws.Range(i, 1, i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
            ws.Range(i, 1, i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);
            ws.Range(i, 1, i, colFinal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        public void setFormatCabecera(int i, themeExcel thema, int colInicio, int colFinal)
        {
            ws.Range(i, colInicio, i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
            ws.Range(i, colInicio, i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);
            ws.Range(i, colInicio, i, colFinal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        public void setPrintTitle(string range)
        {
            //this.ws.PageSetup.PrintTitleRows = range;
            ws.PageSetup.PrintAreas.Add(range);
        }

        public void abrirReporte(string archivo)
        {
            wb.SaveAs(General.reporte + archivo);
            
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(General.reporte + archivo)
            {
                UseShellExecute = true
            };

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
            }
        }

        public string getPivotTableStyle()
        {
            switch (this.temaSeleccionado)
            {
                case 0:
                    return "PivotStyleLight22";
                case 1:
                    return "PivotStyleLight18";
                case 2:
                    return "PivotStyleLight17";
                case 3:
                    return "PivotStyleLight20";
                case 4:
                    return "PivotStyleLight21";
                case 5:
                    return "PivotStyleLight15";
                case 6:
                    return "PivotStyleMedium25";
                case 7:
                    return "PivotStyleLight16";
                case 8:
                    return "PivotStyleMedium26";
                case 9:
                    return "PivotStyleMedium15";
                default:
                    return "PivotStyleLight22";
            }
        }

        public bool IsExcelInstalled()
        {
            // Rutas comunes del registro para diferentes versiones de Excel (32 y 64 bits)
            string[] excelRegistryKeys = new string[]
            {
            // Office 2016/2019/365
            @"SOFTWARE\Microsoft\Office\16.0\Excel\InstallRoot",
            @"SOFTWARE\WOW6432Node\Microsoft\Office\16.0\Excel\InstallRoot",
            // Office 2013
            @"SOFTWARE\Microsoft\Office\15.0\Excel\InstallRoot",
            @"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Excel\InstallRoot",
            // Office 2010
            @"SOFTWARE\Microsoft\Office\14.0\Excel\InstallRoot",
            @"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Excel\InstallRoot"
                // Se pueden añadir versiones anteriores (12.0 para 2007, 11.0 para 2003)
            };

            foreach (var keyPath in excelRegistryKeys)
            {
                // Abrir la clave del registro en modo de solo lectura
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        // Si la clave existe, intenta leer un valor para confirmar la instalación.
                        // "Path" o "ExcelBinPath" suelen ser valores que existen si está instalado.
                        object pathValue = key.GetValue("Path");
                        if (pathValue != null && !string.IsNullOrEmpty(pathValue.ToString()))
                        {
                            Console.WriteLine($"Excel encontrado en: {keyPath}");
                            return true; // Excel está instalado
                        }
                    }
                }
            }

            Console.WriteLine("Excel no se encontró en las rutas de registro comunes.");
            return false; // No se encontró ninguna clave válida para Excel
        }
    }
}

using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;

namespace LibReports
{
    public class RptRegalos : ExcelBase
    {
        private string archivo = "";

        public RptRegalos(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptRegalos");
        }

        public Boolean crear(string desde, string hasta, Boolean detalle)
        {
            Ventas venta = new Ventas(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = venta.reporteRegalos(desde, hasta, detalle);
            String empresa = emp.getNombre();

            try
            {
                int colFinal = 5;

                int i = 5;
                ws.Cell(i, 1).Value = "DESDE : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = desde;
                ws.Cell(i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "HASTA : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = hasta;
                ws.Cell(i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                i = 8;
                int inicio = i;

                ws.Cell(i, 1).Value = "FECHA";
                ws.Cell(i, 2).Value = "TIPO";
                ws.Cell(i, 3).Value = "CAJA";
                ws.Cell(i, 4).Value = "EMPLEADO";

                if (detalle)
                {
                    ws.Cell(i, 5).Value = "SECUENCIAL";
                    ws.Cell(i, 6).Value = "MOTIVO";

                    colFinal = 6;
                }
                else
                {
                    ws.Cell(i, 5).Value = "CANTIDAD";

                    colFinal = 5;
                }

                this.setEmpresaTitulo("Reporte de Regalos", "REPORTE DE REGALOS", colFinal);

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 12;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 40;

                ws.Row(i).Style.Font.Bold = true;
                this.setFormatCabecera(i, thema, colFinal);

                i++;
                double total = 0;

                if (data != null)
                {
                    foreach (mRegalos regalo in data)
                    {
                        ws.Cell(i, 1).Value = regalo.fecha;
                        ws.Cell(i, 2).Value = regalo.tipo;
                        ws.Cell(i, 3).Value = regalo.caja;
                        ws.Cell(i, 4).Value = regalo.empleado;

                        if (detalle)
                        {
                            ws.Cell(i, 5).Value = regalo.secuencial;
                            ws.Cell(i, 6).Value = regalo.motivo;
                        }
                        else
                        {
                            ws.Cell(i, 5).Value = regalo.cantidad;
                            total += regalo.cantidad;
                        }

                        this.setColoresContenido(i, colFinal, thema);
                        i++;
                    }
                }

                if (!detalle)
                {
                    ws.Cell(i, 1).Value = "TOTALES";
                    ws.Cell(i, 5).Value = total;

                    ws.Range(i, 1,i, 5).Style.Font.Bold = true;
                    ws.Range(i, 1,i, 4).Merge();

                    ws.Range(i, 1,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                    ws.Range(i, 1,i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                    ws.PageSetup.PrintAreas.Add("A1: E" + i);
                    this.setBorders("A" + inicio + ":E" + i);
                    this.setPrintTitle("$A1:$E8");
                }
                else
                {
                    ws.Range(i, 1,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                    ws.Range(i, 1,i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                    ws.Range(inicio + 1, 5,i, colFinal).Style.NumberFormat.Format = FormatoNumerico;

                    ws.PageSetup.PrintAreas.Add("A1:F" + i);
                    this.setBorders("A" + inicio + ":F" + i);
                    this.setPrintTitle("$A1:$F8");
                }

                ws.Range(inicio + 1, 1,i, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Range(inicio + 1, 2,i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                this.abrirReporte(this.archivo);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}

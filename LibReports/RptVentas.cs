using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;

namespace LibReports
{
    public class RptVentas : ExcelBase
    {
        private string archivo = "";

        public RptVentas(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptVentas");
        }

        public Boolean crear(int anio, int mes, Boolean detalle)
        {
            Ventas ventas = new Ventas(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = ventas.reporteVentas(anio, mes, detalle);

            try
            {
                int colFinal = 6;
                this.setEmpresaTitulo("Ventas Mensual", "REPORTE DE VENTAS MENSUAL", colFinal);

                int i = 5;
                ws.Cell(i, 1).Value = "AÑO : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = anio.ToString();
                ws.Cell(i, 2).Style.Alignment.Horizontal= XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "MES : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = General.getMesName(mes);
                ws.Cell(i, 4).Style.Alignment.Horizontal= XLAlignmentHorizontalValues.Left;

                i = 8;
                int inicio = i;

                ws.Cell(i, 1).Value = "FECHA";
                ws.Cell(i, 2).Value = "TIPO";
                ws.Cell(i, 3).Value = "SUBTOTAL";
                ws.Cell(i, 4).Value = "DESCUENTO";
                ws.Cell(i, 5).Value = "IVA";
                ws.Cell(i, 6).Value = "TOTAL";

                if (detalle)
                {
                    ws.Cell(i, 7).Value = "EFECTIVO";
                    ws.Cell(i, 8).Value = "TARJETA";
                    ws.Cell(i, 9).Value = "CHEQUE";
                    ws.Cell(i, 10).Value = "TRANSFERENCIA";
                    ws.Cell(i, 11).Value = "VUELTO";

                    colFinal = 11;
                }
                else
                {
                    ws.Cell(i, 7).Value = "VUELTO";

                    colFinal = 7;
                }

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 12;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 12;
                ws.Column(7).Width = 12;
                ws.Column(8).Width = 12;
                ws.Column(9).Width = 12;
                ws.Column(10).Width = 15;
                ws.Column(11).Width = 12;

                ws.Row(i).Style.Font.Bold = true;

                this.setFormatCabecera(i, thema, colFinal);

                i++;
                double[] totales;
                if (detalle)
                {
                    totales = new double[9];
                }
                else
                {
                    totales = new double[5];
                }

                if (data != null)
                {
                    foreach (mVentas venta in data)
                    {
                        ws.Cell(i, 1).Value = venta.fecha;
                        ws.Cell(i, 2).Value = venta.tipo;
                        ws.Cell(i, 3).Value = venta.subtotal;
                        ws.Cell(i, 4).Value = venta.descuento;
                        ws.Cell(i, 5).Value = venta.iva;
                        ws.Cell(i, 6).Value = venta.total;

                        totales[0] += venta.subtotal;
                        totales[1] += venta.descuento;
                        totales[2] += venta.iva;
                        totales[3] += venta.total;

                        if (detalle)
                        {
                            ws.Cell(i, 7).Value = venta.efectivo;
                            ws.Cell(i, 8).Value = venta.tarjeta;
                            ws.Cell(i, 9).Value = venta.cheque;
                            ws.Cell(i, 10).Value = venta.transferencia;
                            ws.Cell(i, 11).Value = venta.vuelto;

                            totales[4] += venta.efectivo;
                            totales[5] += venta.tarjeta;
                            totales[6] += venta.cheque;
                            totales[7] += venta.transferencia;
                            totales[8] += venta.vuelto;
                        }
                        else
                        {
                            ws.Cell(i, 7).Value = venta.vuelto;

                            totales[4] += venta.vuelto;
                        }

                        i++;
                    }
                }

                ws.Cell(i, 1).Value = "TOTALES";
                ws.Cell(i, 3).Value = totales[0];
                ws.Cell(i, 4).Value = totales[1];
                ws.Cell(i, 5).Value = totales[2];
                ws.Cell(i, 6).Value = totales[3];

                if (detalle)
                {
                    ws.Cell(i, 7).Value = totales[4];
                    ws.Cell(i, 8).Value = totales[5];
                    ws.Cell(i, 9).Value = totales[6];
                    ws.Cell(i, 10).Value = totales[7];
                    ws.Cell(i, 11).Value = totales[8];
                }
                else
                {
                    ws.Cell(i, 7).Value = totales[4];
                }

                ws.Range(inicio + 1, 1,i - 1, 1).Style.Alignment.Horizontal= XLAlignmentHorizontalValues.Left;
                ws.Range(inicio + 1, 2,i - 1, 2).Style.Alignment.Horizontal= XLAlignmentHorizontalValues.Left;
                ws.Range(inicio + 1, 3,i, colFinal).Style.NumberFormat.Format = "###,##0.00";

                ws.Range(i, 1,i, 2).Style.Font.Bold = true;
                ws.Range(i, 1,i, 2).Style.Alignment.Horizontal= XLAlignmentHorizontalValues.Right;
                ws.Range(i, 1,i, 2).Merge();

                this.setColoresContenido(inicio + 1, i - 1, colFinal, thema);

                if (detalle)
                {
                    ws.Range(i, 1,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                    ws.Range(i, 1,i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                    ws.PageSetup.PrintAreas.Add("A1:K" + i);
                    this.setBorders("A" + inicio + ":K" + i);
                    this.setPrintTitle("$A1:$K8");
                }
                else
                {
                    ws.Range(i, 1,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                    ws.Range(i, 1,i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                    ws.PageSetup.PrintAreas.Add("A1:G" + i);
                    this.setBorders("A" + inicio + ":G" + i);
                    this.setPrintTitle("$A1:$G8");
                }


                this.abrirReporte(this.archivo);

                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
    }
}

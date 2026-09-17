using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;
using System.Drawing;

namespace LibReports
{

    public class RptPedidos : ExcelBase
    {
        private string archivo = "";

        public RptPedidos(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptPedidos");
        }

        public Boolean crear(string desde, string hasta)
        {
            Pedido pedido = new Pedido(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = pedido.reportePedidos(desde, hasta);
            String empresa = emp.getNombre();

            try
            {
                int colFinal = 6;
                this.setEmpresaTitulo("Pedidos", "REPORTE DE PEDIDOS", colFinal);

                int i = 5;
                ws.Cell(i, 1).Value = "DESDE : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = desde;
                ws.Cell(i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "HASTA : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = hasta;
                ws.Cell(i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                i = 7;

                ws.Cell(i, 3).Value = "LOCAL";
                ws.Cell(i, 5).Value = "DOMICILIO";
                ws.Cell(i, 7).Value = "LLEVAR";
                ws.Cell(i, 9).Value = "DELIVER";
                ws.Cell(i, 11).Value = "ENTREGA";
                ws.Cell(i, 13).Value = "CANTIDAD TOTAL";
                ws.Cell(i, 14).Value = "VALOR TOTAL";

                colFinal = 14;

                ws.Row(i).Style.Font.Bold = true;

                ws.Range("C7:D7").Merge();
                ws.Range("E7:F7").Merge();
                ws.Range("G7:H7").Merge();
                ws.Range("I7:J7").Merge();
                ws.Range("K7:L7").Merge();

                ws.Range(i, 13,i + 1, 13).Merge();
                ws.Range(i, 14,i + 1, 14).Merge();

                ws.Range(i, 13,i + 1, 13).Style.Alignment.WrapText = true;
                ws.Range(i, 14,i + 1, 14).Style.Alignment.WrapText = true;

                ws.Range(i, 3,i, 4).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;
                ws.Range(i, 5,i, 6).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;
                ws.Range(i, 7,i, 8).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;
                ws.Range(i, 9,i, 10).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;
                ws.Range(i, 11,i, 12).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;

                ws.Range(i, 13,i + 1, 13).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;
                ws.Range(i, 14,i + 1, 14).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center;

                ws.Range(i, 3,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                ws.Range(i, 3,i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                i = 8;
                int inicio = i;

                ws.Cell(i, 1).Value = "FECHA";
                ws.Cell(i, 2).Value = "TIPO";
                ws.Cell(i, 3).Value = "CANTIDAD";
                ws.Cell(i, 4).Value = "VALOR";
                ws.Cell(i, 5).Value = "CANTIDAD";
                ws.Cell(i, 6).Value = "VALOR";
                ws.Cell(i, 7).Value = "CANTIDAD";
                ws.Cell(i, 8).Value = "VALOR";
                ws.Cell(i, 9).Value = "CANTIDAD";
                ws.Cell(i, 10).Value = "VALOR";
                ws.Cell(i, 11).Value = "CANTIDAD";
                ws.Cell(i, 12).Value = "VALOR";

                colFinal = 14;

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 12;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 12;
                ws.Column(7).Width = 12;
                ws.Column(8).Width = 12;
                ws.Column(9).Width = 12;
                ws.Column(10).Width = 12;
                ws.Column(11).Width = 12;
                ws.Column(12).Width = 12;
                ws.Column(13).Width = 12;
                ws.Column(14).Width = 12;

                ws.Row(i).Style.Font.Bold = true;

                this.setFormatCabecera(i, thema, colFinal);

                i++;
                int[] cantidades = new int[6];
                double[] totales = new double[6];

                if (data != null)
                {
                    foreach (rPedido rpedido in data)
                    {
                        ws.Cell(i, 1).Value = rpedido.fecha;
                        ws.Cell(i, 2).Value = rpedido.tipo;
                        ws.Cell(i, 3).Value = rpedido.lcnt;
                        ws.Cell(i, 4).Value = rpedido.lpago;
                        ws.Cell(i, 5).Value = rpedido.dcnt;
                        ws.Cell(i, 6).Value = rpedido.dpago;
                        ws.Cell(i, 7).Value = rpedido.ccnt;
                        ws.Cell(i, 8).Value = rpedido.cpago;

                        ws.Cell(i, 9).Value = rpedido.vcnt;
                        ws.Cell(i, 10).Value = rpedido.vpago;
                        ws.Cell(i, 11).Value = rpedido.rcnt;
                        ws.Cell(i, 12).Value = rpedido.rpago;


                        ws.Cell(i, 13).Value = rpedido.cantidad;
                        ws.Cell(i, 14).Value = rpedido.total;

                        cantidades[0] += rpedido.lcnt;
                        cantidades[1] += rpedido.dcnt;
                        cantidades[2] += rpedido.ccnt;
                        cantidades[3] += rpedido.vcnt;
                        cantidades[4] += rpedido.rcnt;
                        cantidades[5] += rpedido.cantidad;

                        totales[0] += rpedido.lpago;
                        totales[1] += rpedido.dpago;
                        totales[2] += rpedido.cpago;
                        totales[3] += rpedido.vpago;
                        totales[4] += rpedido.rpago;
                        totales[5] += rpedido.total;

                        i++;
                    }
                }

                ws.Cell(i, 1).Value = "TOTALES";
                ws.Cell(i, 3).Value = cantidades[0];
                ws.Cell(i, 5).Value = cantidades[1];
                ws.Cell(i, 7).Value = cantidades[2];
                ws.Cell(i, 9).Value = cantidades[3];
                ws.Cell(i, 11).Value = cantidades[4];
                ws.Cell(i, 13).Value = cantidades[5];
                ws.Cell(i, 4).Value = totales[0];
                ws.Cell(i, 6).Value = totales[1];
                ws.Cell(i, 8).Value = totales[2];
                ws.Cell(i, 10).Value = totales[3];
                ws.Cell(i, 12).Value = totales[4];
                ws.Cell(i, 14).Value = totales[5];

                ws.Range(inicio + 1, 1,i - 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Range(inicio + 1, 2,i - 1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Range(inicio + 1, 4,i, 4).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 6,i, 6).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 8,i, 8).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 10,i, 10).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 12,i, 12).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 14,i, 14).Style.NumberFormat.Format = FormatoNumerico;

                this.setColoresContenido(inicio + 1, i - 1, colFinal, thema);

                ws.Range(i, 1,i, 2).Style.Font.Bold = true;
                ws.Range(i, 1,i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Range(i, 1,i, 2).Merge();

                ws.Range(i, 1,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                ws.Range(i, 1,i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                ws.PageSetup.PrintAreas.Add("A1:N" + i);
                this.setBorders("C" + (inicio - 1) + ":N" + i);
                this.setBorders("A" + inicio + ":N" + i);
                this.setPrintTitle("$A1:$N8");

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

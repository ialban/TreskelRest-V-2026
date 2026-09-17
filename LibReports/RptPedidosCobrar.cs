using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;
using System.Drawing;

namespace LibReports
{
    public class RptPedidosCobrar : ExcelBase
    {
        private string archivo = "";

        public RptPedidosCobrar(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptPedidosCobrar");
        }

        public Boolean crear(string desde, string hasta, string id, string cliente, int tipo)
        {
            Ventas ventas = new Ventas(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = ventas.reporteVentasCobrar(desde, hasta, id, tipo);
            string empresa = emp.getNombre();

            try
            {
                int colFinal = 6;
                this.setEmpresaTitulo("Pedidos", "REPORTE DE PEDIDOS POR COBRAR", colFinal);

                int i = 5;
                ws.Cell(i, 1).Value = "DESDE : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = desde;
                ws.Cell(i, 2).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "HASTA : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = hasta;
                ws.Cell(i, 4).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Left;
                i = 6;
                ws.Cell(i, 1).Value = "CLIENTE : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = cliente;

                i = 8;

                ws.Cell(i, 1).Value = "No.";
                ws.Cell(i, 2).Value = "SECUENCIAL";
                ws.Cell(i, 3).Value = "FECHA";
                ws.Cell(i, 4).Value = "CANTIDAD";
                ws.Cell(i, 5).Value = "TOTAL";
                ws.Cell(i, 6).Value = "DESCRIPCIÓN";
                ws.Cell(i, 7).Value = "PERSONA";

                colFinal = 7;

                ws.Row(i).Style.Font.Bold = true;
                ws.Range(i, 1,i, colFinal).Style.Alignment.WrapText = true;
                ws.Range(i, 1,i, colFinal).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Center;
                ws.Range(i, 1,i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);

                i++;
                int inicio = i;
                //for (int cl = 0; cl<0; cl++)
                //{
                //    ws.Columns[cl + 2].ColumnWidth = 20;
                //}

                
                this.setFormatCabecera(i, thema, colFinal);

                if (data != null)
                {
                    foreach(mPorCobrar fila in data)
                    {
                        ws.Cell(i, 1).Value = i - 8;
                        ws.Cell(i, 2).Style.NumberFormat.Format = "@";
                        ws.Cell(i, 2).Value = fila.secuencial.ToString();
                        ws.Cell(i, 3).Value = fila.fecha;
                        ws.Cell(i, 4).Value = fila.cantidad;
                        ws.Cell(i, 5).Value = fila.total;
                        ws.Cell(i, 6).Value = fila.descripcion;
                        ws.Cell(i, 7).Value = fila.persona;
                        i++;
                    }
                }

                ws.Range(inicio, 1,i, 1).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Left;
                ws.Range(inicio, 4,i, 4).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Right;

                ws.Range(inicio, 4,i, 4).Style.NumberFormat.Format = FormatoNumerico;
                this.setColoresContenido(inicio, i-1, colFinal, thema);

                ws.PageSetup.PrintAreas.Add("A1:N" + i);
                this.setBorders("A" + inicio + ":E" + i);
                this.setPrintTitle("$A3:$A8");

                // Se crea el Sheet con la tabla dinámica
                creaNuevoSheet("PivotPedidos", "REPORTE DE PEDIDOS POR COBRAR", colFinal + 2);

                int ip = 5;
                wsp.Cell(ip, 1).Value = "DESDE : ";
                wsp.Cell(ip, 1).Style.Font.Bold = true;
                wsp.Cell(ip, 2).Value = desde;
                wsp.Cell(ip, 2).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Left;
                wsp.Cell(ip, 3).Value = "HASTA : ";
                wsp.Cell(ip, 3).Style.Font.Bold = true;
                wsp.Cell(ip, 4).Value = hasta;
                wsp.Cell(ip, 4).Style.Alignment.Horizontal=  XLAlignmentHorizontalValues.Left;
                ip = 6;
                wsp.Cell(ip, 1).Value = "CLIENTE : ";
                wsp.Cell(ip, 1).Style.Font.Bold = true;
                wsp.Cell(ip, 2).Value = cliente;


                // Se define el rango de la data de la tabla dinámica
                if (i <= inicio) i = inicio + 1;

                /*   SECCION DE PIVOT TABLE    HAY QUE MIGRARLO*/
                /*
                Range pivotData = ws.Range("A" + (inicio -1) + ":G" + (i - 1));
                Range pivotDestino = wsp.Range("A" + (inicio + 1));
                PivotCache pivotCache = wb.PivotCaches().Add(XlPivotTableSourceType.xlDatabase, pivotData);
                PivotTable pivotTable = wsp.PivotTables().Add(pivotCache, pivotDestino, "Reporte de Pedidos por Cobrar");

                PivotField cantidad = pivotTable.PivotFields("CANTIDAD");
                cantidad.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlDataField;
                cantidad.Function = XlConsolidationFunction.xlSum;

                PivotField total = pivotTable.PivotFields("TOTAL");
                total.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlDataField;
                total.NumberFormat = "######.00";
                total.Function = XlConsolidationFunction.xlSum;


                PivotField fechas = pivotTable.PivotFields("FECHA");
                fechas.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlRowField;
                PivotField secuencial = pivotTable.PivotFields("SECUENCIAL");
                secuencial.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlRowField;
                PivotField persona = pivotTable.PivotFields("PERSONA");
                persona.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlRowField;

                PivotField productos = pivotTable.PivotFields("DESCRIPCIÓN");
                productos.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlColumnField;

                // Definir filtro en tabla dinámica.
                PivotField fsecuencial = pivotTable.PivotFields("SECUENCIAL");
                fsecuencial.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlPageField;
                

                // Aplicar style a la tabla dinámica.
                pivotTable.TableStyle2 = getPivotTableStyle();*/

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

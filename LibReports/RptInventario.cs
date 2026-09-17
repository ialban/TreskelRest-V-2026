using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;

namespace LibReports
{
    public class RptInvOpciones
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public string tipo { get; set; }
    }

    public class RptInventario : ExcelBase
    {
        private string archivo = "";

        public RptInventario(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptInventario");
        }

        public Boolean crear(RptInvOpciones opciones)
        {
            switch (opciones.tipo)
            {
                case "K":
                    return Kardex(opciones.anio, opciones.mes);
                    break;
                case "M":
                    return Movimiento(opciones.desde, opciones.hasta);
                    break;
                default:
                    return false;
                    break;
            }
        }

        private Boolean Kardex(int anio, int mes)
        {
            Items items = new Items(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = items.reporteKardex(anio, mes);
            String empresa = emp.getNombre();
            
            try
            {
                int colFinal = 6;
                this.setEmpresaTitulo("Kardex", "KARDEX ANUAL", colFinal);

                int i = 5;
                ws.Cell(i, 1).Value = "AÑO : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = anio;
                ws.Cell(i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 

                if (mes > 0)
                {
                    ws.Cell(i, 3).Value = "MES : ";
                    ws.Cell(i, 3).Style.Font.Bold = true;
                    ws.Cell(i, 4).Value = General.getMesName(mes);
                    ws.Cell(i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                }
                

                i = 8;
                int inicio = i;

                if (mes > 0)
                {
                    ws.Cell(i, 1).Value = "PRODUCTO";
                    ws.Cell(i, 2).Value = "AÑO";
                    ws.Cell(i, 3).Value = "INCIO";
                    ws.Cell(i, 4).Value = "INGRESOS";
                    ws.Cell(i, 5).Value = "EGRESO";
                    ws.Cell(i, 6).Value = "SALDO";

                    colFinal = 6;

                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 12;
                    ws.Column(3).Width = 12;
                    ws.Column(4).Width = 12;
                    ws.Column(5).Width = 12;
                    ws.Column(6).Width = 12;
                }
                else
                {
                    ws.Cell(i, 1).Value = "PRODUCTO";
                    ws.Cell(i, 2).Value = "AÑO";

                    ws.Cell(i - 1, 3).Value = "ENERO";
                    ws.Range(i - 1, 3, i - 1, 6).Merge();
                    ws.Range(i - 1, 3, i - 1, 3).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 3).Value = "INCIO";
                    ws.Cell(i, 4).Value = "INGRESOS";
                    ws.Cell(i, 5).Value = "EGRESO";
                    ws.Cell(i, 6).Value = "SALDO";

                    ws.Cell(i - 1, 7).Value = "FEBRERO";
                    ws.Range(i - 1, 7, i - 1, 10).Merge();
                    ws.Range(i - 1, 7, i - 1, 10).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 7).Value = "INCIO";
                    ws.Cell(i, 8).Value = "INGRESOS";
                    ws.Cell(i, 9).Value = "EGRESO";
                    ws.Cell(i, 10).Value = "SALDO";

                    ws.Cell(i - 1, 11).Value = "MARZO";
                    ws.Range(i - 1, 11, i - 1, 14).Merge();
                    ws.Range(i - 1, 11, i - 1, 14).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 11).Value = "INCIO";
                    ws.Cell(i, 12).Value = "INGRESOS";
                    ws.Cell(i, 13).Value = "EGRESO";
                    ws.Cell(i, 14).Value = "SALDO";

                    ws.Cell(i - 1, 15).Value = "ABRIL";
                    ws.Range(i - 1, 15, i - 1, 18).Merge();
                    ws.Range(i - 1, 15, i - 1, 18).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 15).Value = "INCIO";
                    ws.Cell(i, 16).Value = "INGRESOS";
                    ws.Cell(i, 17).Value = "EGRESO";
                    ws.Cell(i, 18).Value = "SALDO";

                    ws.Cell(i - 1, 19).Value = "MAYO";
                    ws.Range(i - 1, 19, i - 1, 22).Merge();
                    ws.Range(i - 1, 19, i - 1, 22).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 19).Value = "INCIO";
                    ws.Cell(i, 20).Value = "INGRESOS";
                    ws.Cell(i, 21).Value = "EGRESO";
                    ws.Cell(i, 22).Value = "SALDO";

                    ws.Cell(i - 1, 23).Value = "JUNIO";
                    ws.Range(i - 1, 23, i - 1, 26).Merge();
                    ws.Range(i - 1, 23, i - 1, 26).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 23).Value = "INCIO";
                    ws.Cell(i, 24).Value = "INGRESOS";
                    ws.Cell(i, 25).Value = "EGRESO";
                    ws.Cell(i, 26).Value = "SALDO";

                    ws.Cell(i - 1, 27).Value = "JULIO";
                    ws.Range(i - 1, 27, i - 1, 30).Merge();
                    ws.Range(i - 1, 27, i - 1, 30).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 27).Value = "INCIO";
                    ws.Cell(i, 28).Value = "INGRESOS";
                    ws.Cell(i, 29).Value = "EGRESO";
                    ws.Cell(i, 30).Value = "SALDO";

                    ws.Cell(i - 1, 31).Value = "AGOSTO";
                    ws.Range(i - 1, 31, i - 1, 34).Merge();
                    ws.Range(i - 1, 31, i - 1, 34).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 31).Value = "INCIO";
                    ws.Cell(i, 32).Value = "INGRESOS";
                    ws.Cell(i, 33).Value = "EGRESO";
                    ws.Cell(i, 34).Value = "SALDO";

                    ws.Cell(i - 1, 35).Value = "SEPTIEMBRE";
                    ws.Range(i - 1, 35, i - 1, 38).Merge();
                    ws.Range(i - 1, 35, i - 1, 38).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 35).Value = "INCIO";
                    ws.Cell(i, 36).Value = "INGRESOS";
                    ws.Cell(i, 37).Value = "EGRESO";
                    ws.Cell(i, 38).Value = "SALDO";

                    ws.Cell(i - 1, 39).Value = "OCTUBRE";
                    ws.Range(i - 1, 39, i - 1, 42).Merge();
                    ws.Range(i - 1, 39, i - 1, 42).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 39).Value = "INCIO";
                    ws.Cell(i, 40).Value = "INGRESOS";
                    ws.Cell(i, 41).Value = "EGRESO";
                    ws.Cell(i, 42).Value = "SALDO";

                    ws.Cell(i - 1, 43).Value = "NOVIEMBRE";
                    ws.Range(i - 1, 43, i - 1, 46).Merge();
                    ws.Range(i - 1, 43, i - 1, 46).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 43).Value = "INCIO";
                    ws.Cell(i, 44).Value = "INGRESOS";
                    ws.Cell(i, 45).Value = "EGRESO";
                    ws.Cell(i, 46).Value = "SALDO";

                    ws.Cell(i - 1, 47).Value = "DICMIEMBRE";
                    ws.Range(i - 1, 47, i - 1, 50).Merge();
                    ws.Range(i - 1, 47, i - 1, 50).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Center; 
                    ws.Cell(i, 47).Value = "INCIO";
                    ws.Cell(i, 48).Value = "INGRESOS";
                    ws.Cell(i, 49).Value = "EGRESO";
                    ws.Cell(i, 50).Value = "SALDO";

                    colFinal = 50;

                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 12;
                    for (int col = 3; col<= colFinal; col++)
                    {
                        ws.Column(col).Width = 12;
                    }
                    

                }

                ws.Row(i - 1).Style.Font.Bold = true;
                ws.Row(i).Style.Font.Bold = true;

                this.setFormatCabecera(i - 1, thema, 3, colFinal);
                this.setFormatCabecera(i, thema, colFinal);

                i++;
                int[] cantidades = new int[4];
                double[] totales = new double[4];

                if(mes > 0)
                {
                    if (data != null)
                    {
                        foreach (rMovItemsMes rpedido in data)
                        {
                            ws.Cell(i, 1).Value = rpedido.producto;
                            ws.Cell(i, 2).Value = rpedido.anio;
                            ws.Cell(i, 3).Value = rpedido.inicioMes;
                            ws.Cell(i, 4).Value = rpedido.ingresosMes;
                            ws.Cell(i, 5).Value = rpedido.egresosMes;
                            ws.Cell(i, 6).Value = rpedido.saldosMes;

                            i++;
                        }
                    }

                    i--;

                    ws.Range(inicio + 1, 1, i, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                    ws.Range(inicio + 1, 2, i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                    ws.Range(inicio + 1, 3, i, colFinal).Style.NumberFormat.Format = FormatoNumerico;

                    ws.PageSetup.PrintAreas.Add("A1:F" + i);
                    this.setBorders("A" + inicio + ":F" + i);
                    this.setPrintTitle("$A1:$F8");
                }
                else
                {
                    foreach (rMovItemsKrdx rpedido in data)
                    {
                        ws.Cell(i, 1).Value = rpedido.producto;
                        ws.Cell(i, 2).Value = rpedido.anio;

                        ws.Cell(i, 3).Value = rpedido.inicioEnero;
                        ws.Cell(i, 4).Value = rpedido.ingresosEnero;
                        ws.Cell(i, 5).Value = rpedido.egresosEnero;
                        ws.Cell(i, 6).Value = rpedido.saldosEnero;

                        ws.Cell(i, 7).Value = rpedido.inicioFebrero;
                        ws.Cell(i, 8).Value = rpedido.ingresosFebrero;
                        ws.Cell(i, 9).Value = rpedido.egresosFebrero;
                        ws.Cell(i, 10).Value = rpedido.saldosFebrero;

                        ws.Cell(i, 11).Value = rpedido.inicioMarzo;
                        ws.Cell(i, 12).Value = rpedido.ingresosMarzo;
                        ws.Cell(i, 13).Value = rpedido.egresosMarzo;
                        ws.Cell(i, 14).Value = rpedido.saldosMarzo;

                        ws.Cell(i, 15).Value = rpedido.inicioAbril;
                        ws.Cell(i, 16).Value = rpedido.ingresosAbril;
                        ws.Cell(i, 17).Value = rpedido.egresosAbril;
                        ws.Cell(i, 18).Value = rpedido.saldosAbril;

                        ws.Cell(i, 19).Value = rpedido.inicioMayo;
                        ws.Cell(i, 20).Value = rpedido.ingresosMayo;
                        ws.Cell(i, 21).Value = rpedido.egresosMayo;
                        ws.Cell(i, 22).Value = rpedido.saldosMayo;

                        ws.Cell(i, 23).Value = rpedido.inicioJunio;
                        ws.Cell(i, 24).Value = rpedido.ingresosJunio;
                        ws.Cell(i, 25).Value = rpedido.egresosJunio;
                        ws.Cell(i, 26).Value = rpedido.saldosJunio;

                        ws.Cell(i, 27).Value = rpedido.inicioJulio;
                        ws.Cell(i, 28).Value = rpedido.ingresosJulio;
                        ws.Cell(i, 29).Value = rpedido.egresosJulio;
                        ws.Cell(i, 30).Value = rpedido.saldosJulio;

                        ws.Cell(i, 31).Value = rpedido.inicioAgosto;
                        ws.Cell(i, 32).Value = rpedido.ingresosAgosto;
                        ws.Cell(i, 33).Value = rpedido.egresosAgosto;
                        ws.Cell(i, 34).Value = rpedido.saldosAgosto;

                        ws.Cell(i, 35).Value = rpedido.inicioSeptiembre;
                        ws.Cell(i, 36).Value = rpedido.ingresosSeptiembre;
                        ws.Cell(i, 37).Value = rpedido.egresosSeptiembre;
                        ws.Cell(i, 38).Value = rpedido.saldosSeptiembre;

                        ws.Cell(i, 39).Value = rpedido.inicioOctubre;
                        ws.Cell(i, 40).Value = rpedido.ingresosOctubre;
                        ws.Cell(i, 41).Value = rpedido.egresosOctubre;
                        ws.Cell(i, 42).Value = rpedido.saldosOctubre;

                        ws.Cell(i, 43).Value = rpedido.inicioNoviembre;
                        ws.Cell(i, 44).Value = rpedido.ingresosNoviembre;
                        ws.Cell(i, 45).Value = rpedido.egresosNoviembre;
                        ws.Cell(i, 46).Value = rpedido.saldosNoviembre;

                        ws.Cell(i, 47).Value = rpedido.inicioDiciembre;
                        ws.Cell(i, 48).Value = rpedido.ingresosDiciembre;
                        ws.Cell(i, 49).Value = rpedido.egresosDiciembre;
                        ws.Cell(i, 50).Value = rpedido.saldosDiciembre;

                        i++;
                    }

                    i--;

                    ws.Range(inicio + 1, 1, i, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                    ws.Range(inicio + 1, 2, i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                    ws.Range(inicio + 1, 3, i, colFinal).Style.NumberFormat.Format = FormatoNumerico;

                    ws.PageSetup.PrintAreas.Add("A1:AX" + i);
                    this.setBorders("C" + (inicio - 1) + ":AX" + i);
                    this.setBorders("A" + inicio + ":AX" + i);
                    this.setPrintTitle("$A1:$J8");
                }

                this.abrirReporte(this.archivo);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private Boolean Movimiento(string desde, string hasta)
        {
            Items items = new Items(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = items.reporteMovimientoInventario(desde, hasta);
            String empresa = emp.getNombre();

            try
            {
                int colFinal = 6;
                this.setEmpresaTitulo("Inventario", "REPORTE DE MOVIMIENTO DE INVENTARIO", colFinal);

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
                ws.Cell(i, 2).Value = "PRODUCTO";
                ws.Cell(i, 3).Value = "MOTIVO";
                ws.Cell(i, 4).Value = "TIPO";
                ws.Cell(i, 5).Value = "CANTIDAD";
                ws.Cell(i, 6).Value = "UNIDAD";

                colFinal = 6;

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 25;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 12;


                ws.Cell(i, 1).Style.Font.Bold = true;

                this.setFormatCabecera(i, thema, colFinal);

                i++;
                int[] cantidades = new int[4];
                double[] totales = new double[4];

                foreach (rMovItems rMovimientos in data)
                {
                    ws.Cell(i, 1).Value = rMovimientos.fecha;

                    ws.Cell(i, 2).Value = rMovimientos.producto;

                    ws.Cell(i, 3).Value = rMovimientos.descripcion;

                    ws.Cell(i, 4).Value = rMovimientos.movimiento;

                    ws.Cell(i, 5).Value = rMovimientos.cantidad;
                    ws.Cell(i, 6).Value = rMovimientos.unidad;

                    i++;
                }

                i--;

                ws.Range(inicio + 1, 1, i, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                ws.Range(inicio + 1, 2, i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                ws.Range(inicio + 1, 3, i, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                ws.Range(inicio + 1, 4, i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 

                ws.PageSetup.PrintAreas.Add("A1:F" + i);

                this.setBorders("A" + inicio + ":F" + i);
                this.setPrintTitle("$A1:$F8");

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

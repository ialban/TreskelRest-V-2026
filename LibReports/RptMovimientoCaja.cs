using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;
using System.Collections.Generic;

namespace LibReports
{
    public class RptMovimientoCaja : ExcelBase
    {
        private String archivo = "";

        public RptMovimientoCaja(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptMovimientoCaja");
        }

        public Boolean crear(string opcion, int anio, int mes, int dia, Boolean detalle)
        {
            MovimientoCaja mov = new MovimientoCaja(this.connectionString);            

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            List<mControlCaja> data = null;
            
            data = mov.reporteCaja(opcion, anio, mes, dia, detalle);

            String empresa = emp.getNombre();            

            try
            {
                int colFinal = 6;
                if (detalle) colFinal = 7;

                if (opcion == "M")
                    this.setEmpresaTitulo("Movimiento Caja Mensual","REPORTE DE MOVIMIENTO DE CAJA MENSUAL",colFinal);
                if (opcion == "D")
                {
                    colFinal = 9;
                    this.setEmpresaTitulo("Movimiento Caja Diario", "REPORTE DE MOVIMIENTO DE CAJA DIARIO", colFinal);
                }
                    

                int i = 5;
                ws.Cell(i, 1).Value = "AÑO : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = anio.ToString();
                ws.Cell(i, 2).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "MES : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = General.getMesName(mes);
                ws.Cell(i, 4).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Left;

                if(opcion == "D")
                {
                    ws.Cell(i, 5).Value = "DÍA : ";
                    ws.Cell(i, 5).Style.Font.Bold = true;
                    ws.Cell(i, 6).Value = dia;
                    ws.Cell(i, 6).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Left;
                }

                i = 8;
                int inicio = i;

                ws.Cell(i, 1).Value = "FECHA";
                ws.Cell(i, 2).Value = "CAJA";

                if(opcion == "D")
                {
                    ws.Cell(i, 3).Value = "DETALLE";
                    ws.Cell(i, 4).Value = "INICIO";
                    ws.Cell(i, 5).Value = "INGRESOS";
                    ws.Cell(i, 6).Value = "EGRESOS";
                    ws.Cell(i, 7).Value = "CIERRE";
                    ws.Cell(i, 8).Value = "SISTEMA";
                    ws.Cell(i, 9).Value = "DIFERENCIA";
                }
                else
                {
                    ws.Cell(i, 3).Value = "INICIO";
                    if (detalle)
                    {
                        ws.Cell(i, 4).Value = "INGRESO";
                        ws.Cell(i, 5).Value = "EGRESO";
                        ws.Cell(i, 6).Value = "CIERRE";
                        ws.Cell(i, 7).Value = "DIFERENCIA";
                    }
                    else
                    {
                        ws.Cell(i, 4).Value = "CIERRE";
                        ws.Cell(i, 5).Value = "SISTEMA";
                        ws.Cell(i, 6).Value = "DIFERENCIA";
                    }
                }
                
                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = opcion == "D" ? 25 : 12;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 12;
                ws.Column(7).Width = 12;
                if(opcion == "D")
                {
                    ws.Column(8).Width = 12;
                    ws.Column(9).Width = 12;
                }

                ws.Row(i).Style.Font.Bold = true;

                this.setFormatCabecera(i, thema, colFinal);

                i++;
                double[] totales;
                if (detalle)
                {
                    totales = new double[5];
                }
                else
                {
                    if(opcion == "D")
                    {
                        totales = new double[6];
                    }
                    else
                    {
                        totales = new double[4];
                    }
                }

                if (data != null)
                {
                    foreach (mControlCaja control in data)
                    {
                        ws.Cell(i, 1).Value = control.fecha;
                        ws.Range(i, 1, i, 1).Style.Alignment.Horizontal =  XLAlignmentHorizontalValues.Left;
                        ws.Cell(i, 2).Value = control.caja;

                        if (opcion == "D")
                        {
                            ws.Cell(i, 3).Value = control.detalle;
                            ws.Cell(i, 4).Value = control.inicio;
                        }
                        else
                        {
                            ws.Cell(i, 3).Value = control.inicio;
                        }

                        totales[0] += control.inicio;
                        if (detalle)
                        {
                            ws.Cell(i, 4).Value = control.ingreso;
                            ws.Cell(i, 5).Value = control.egreso;
                            ws.Cell(i, 6).Value = control.cierre;
                            ws.Cell(i, 7).Value = control.diferencia;

                            ws.Range(i, 2, i, colFinal).Style.NumberFormat.Format = FormatoNumerico;

                            totales[1] += control.ingreso;
                            totales[2] += control.egreso;
                            totales[3] += control.cierre;
                            totales[4] += control.diferencia;

                            this.setColoresContenido(i, colFinal, thema);
                        }
                        else
                        {
                            if (opcion == "D")
                            {
                                ws.Cell(i, 4).Value = control.inicio;
                                ws.Cell(i, 5).Value = control.ingreso;
                                ws.Cell(i, 6).Value = control.egreso;
                                ws.Cell(i, 7).Value = control.cierre;
                                ws.Cell(i, 8).Value = control.sistema;
                                ws.Cell(i, 9).Value = control.diferencia;

                                totales[1] += control.ingreso;
                                totales[2] += control.egreso;
                                totales[3] += control.cierre;
                                totales[4] += control.sistema;
                                totales[5] += control.diferencia;
                            }
                            else
                            {
                                ws.Cell(i, 4).Value = control.cierre;
                                ws.Cell(i, 5).Value = control.sistema;
                                ws.Cell(i, 6).Value = control.diferencia;

                                totales[1] += control.cierre;
                                totales[2] += control.sistema;
                                totales[3] += control.diferencia;
                            }

                            this.setColoresContenido(i, colFinal, thema);
                        }

                        i++;
                    }

                }

                //i++;
                ws.Cell(i, 1).Value = "TOTALES";
                if(opcion == "D")
                {
                    ws.Cell(i, 4).Value = totales[0];
                    ws.Cell(i, 5).Value = totales[1];
                    ws.Cell(i, 6).Value = totales[2];
                    ws.Cell(i, 7).Value = totales[3];
                    ws.Cell(i, 8).Value = totales[4];
                    ws.Cell(i, 9).Value = totales[5];
                }
                else
                {
                    ws.Cell(i, 3).Value = totales[0];
                    ws.Cell(i, 4).Value = totales[1];
                    ws.Cell(i, 5).Value = totales[2];
                    ws.Cell(i, 6).Value = totales[3];
                }
                
                if (detalle)
                {
                    ws.Cell(i, 7).Value = totales[4];
                }

                ws.Range(i, 1, i, 2).Style.Font.Bold = true;
                ws.Range(i, 1, i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Range(i, 1, i, 2).Merge();

                if (detalle)
                {
                    ws.Range(i, 1, i, 7).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                    ws.Range(i, 1, i, 7).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                    ws.PageSetup.PrintAreas.Add("A1:G" + i);
                    this.setBorders("A" + inicio + ":G" + i);
                    this.setPrintTitle("$A1:$G8");
                }
                else
                {
                    if (opcion == "D")
                    {
                        ws.Range(i, 1, i, 9).Style.Fill.BackgroundColor  = XLColor.FromHtml(thema.colorFondoCabecera);
                        ws.Range(i, 1, i, 9).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);
                        
                        ws.PageSetup.PrintAreas.Add("A1:I" + i);
                        this.setBorders("A" + inicio + ":I" + i);
                        this.setPrintTitle("$A1:$I8");
                    }
                    else
                    {
                        ws.Range(i, 1, i, 6).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                        ws.Range(i, 1, i, 6).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);
                        
                        ws.PageSetup.PrintAreas.Add("A1:F" + i);
                        this.setBorders("A" + inicio + ":F" + i);
                        this.setPrintTitle("$A1:$F8");
                    }
                    
                }         

                ws.Column(1).Width = 15;
                ws.Column(2).Width = 8;

                this.abrirReporte(this.archivo);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }            
        }
    }
}

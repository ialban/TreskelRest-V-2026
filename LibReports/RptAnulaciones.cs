using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;
using System.Drawing;

namespace LibReports
{
    public class RptAnulaciones : ExcelBase
    {
        private string archivo = "";

        public RptAnulaciones(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptAnulaciones");
        }

        public Boolean crear(int anio, int mes)
        {
            Ventas ventas = new Ventas(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = ventas.reporteAnulaciones(anio, mes);
            String empresa = emp.getNombre();

            try
            {
                int colFinal = 8;
                this.setEmpresaTitulo("ANULACIONES-ELIMINACIONES", "REPORTE DE ANULACIONES/ELIMINACIONES MENSUAL", colFinal);

                int i = 5;
                ws.Cell(i, 1).Value = "AÑO : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = anio.ToString();
                ws.Cell(i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "MES : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = General.getMesName(mes);
                ws.Cell(i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                i = 8;
                int inicio = i;

                ws.Cell(i, 1).Value = "PEDIDO";
                ws.Cell(i, 2).Value = "CAJA";
                ws.Cell(i, 3).Value = "USUARIO";
                ws.Cell(i, 4).Value = "FECHA";
                ws.Cell(i, 5).Value = "NO. FACTURA";
                ws.Cell(i, 6).Value = "VALOR";
                ws.Cell(i, 7).Value = "OBSERVACION";
                ws.Cell(i, 8).Value = "ACCION";


                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 30;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 12;
                ws.Column(7).Width = 40;
                ws.Column(8).Width = 12;

                ws.Row(i).Style.Font.Bold = true;

                this.setFormatCabecera(i, thema, colFinal);

                i++;

                if (data != null)
                {
                    foreach (mAnulaciones venta in data)
                    {
                        ws.Cell(i, 1).Value = venta.pedido;
                        ws.Cell(i, 2).Value = venta.caja;
                        ws.Cell(i, 3).Value = venta.usuario;
                        ws.Cell(i, 4).Value = venta.fecha;
                        ws.Cell(i, 5).Value = venta.factura;
                        ws.Cell(i, 6).Value = venta.total;
                        ws.Cell(i, 7).Value = venta.observacion;
                        ws.Cell(i, 8).Value = venta.estado;

                        i++;
                    }
                }

                ws.Range(inicio + 1, 1, i - 1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Range(inicio + 1, 6, i - 1, 6).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 6, i - 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Range(i, 1, i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                ws.Range(i, 1, i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                ws.PageSetup.PrintAreas.Add("A1:H" + i);
                this.setBorders("A" + inicio + ":H" + i);
                this.setPrintTitle("$A1:$H8");

                this.abrirReporte(this.archivo);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Boolean crearFE(int anio, int mes, int dia)
        {
            Ventas ventas = new Ventas(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = ventas.reporteAnulacionesFE(anio, mes, dia);
            String empresa = emp.getNombre();

            try
            {
                int colFinal = 10;
                this.setEmpresaTitulo("ANULACIONES-ELIMINACIONES", "REPORTE DE ANULACIONESDE FACTURAS ELECTRONICAS", colFinal);

                int i = 5;
                ws.Cell(i, 1).Value = "AÑO : ";
                ws.Cell(i, 1).Style.Font.Bold = true;
                ws.Cell(i, 2).Value = anio.ToString();
                ws.Cell(i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell(i, 3).Value = "MES : ";
                ws.Cell(i, 3).Style.Font.Bold = true;
                ws.Cell(i, 4).Value = mes.ToString().PadLeft(2,'0') + "("+ General.getMesName(mes) + ")";
                ws.Cell(i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell(i, 5).Value = "DIA : ";
                ws.Cell(i, 5).Style.Font.Bold = true;
                ws.Cell(i, 6).Value = dia.ToString().PadLeft(2, '0');
                ws.Cell(i, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                i = 8;
                int inicio = i;

                ws.Cell(i, 1).Value = "PEDIDO";
                ws.Cell(i, 2).Value = "VENTA";
                ws.Cell(i, 3).Value = "CODIGO AUTORIZACION";
                ws.Cell(i, 4).Value = "CAJA";
                ws.Cell(i, 5).Value = "USUARIO";
                ws.Cell(i, 6).Value = "FECHA";
                ws.Cell(i, 7).Value = "NO. FACTURA";
                ws.Cell(i, 8).Value = "VALOR";
                ws.Cell(i, 9).Value = "OBSERVACION";
                ws.Cell(i, 10).Value = "ACCION";


                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 55;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 30;
                ws.Column(6).Width = 12;
                ws.Column(7).Width = 22;
                ws.Column(8).Width = 12;
                ws.Column(9).Width = 40;
                ws.Column(10).Width = 12;

                ws.Row(i).Style.Font.Bold = true;

                this.setFormatCabecera(i, thema, colFinal);

                i++;


                foreach (mAnulacionesFE venta in data)
                {
                    ws.Cell(i, 1).Value = venta.pedido;
                    ws.Cell(i, 2).Value = venta.ventscl;
                    ws.Cell(i, 3).Value = "-"+venta.autosri+"-";
                    ws.Cell(i, 4).Value = venta.caja;
                    ws.Cell(i, 5).Value = venta.usuario;
                    ws.Cell(i, 6).Value = venta.fecha;
                    ws.Cell(i, 7).Value = venta.factura;
                    ws.Cell(i, 8).Value = venta.total;
                    ws.Cell(i, 9).Value = venta.observacion;
                    ws.Cell(i, 10).Value = venta.estado;

                    i++;
                }

                ws.Range(inicio + 1, 1, i - 1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; 
                ws.Range(inicio + 1, 8, i - 1, 8).Style.NumberFormat.Format = FormatoNumerico;
                ws.Range(inicio + 1, 8, i - 1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Range(i, 1, i, colFinal).Style.Fill.BackgroundColor = XLColor.FromHtml(thema.colorFondoCabecera);
                ws.Range(i, 1, i, colFinal).Style.Font.FontColor = XLColor.FromHtml(thema.colorLetrasCabecera);

                ws.PageSetup.PrintAreas.Add("A1:J" + i);
                this.setBorders("A" + inicio + ":J" + i);
                this.setPrintTitle("$A1:$J10");

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

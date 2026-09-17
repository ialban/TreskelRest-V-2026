using ClosedXML.Excel;
using Entidades;
using Entidades.modelos;
using LibReports.modelo;
using System;
using System.Linq;

namespace LibReports
{
    public class RptSRI : ExcelBase
    {
        private string archivo = "";

        public RptSRI(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptSRI");
        }

        public Boolean crear(int anio, int mes, int dia)
        {
            DocsSri docs = new DocsSri(this.connectionString);

            themeExcel thema = new themeExcel();

            thema = this.getTema();

            var data = docs.reporteComprobantes(anio, mes, dia);

            try
            {
                int colFinal = 6;
                this.setEmpresaTitulo("Comprobantes SRI", "REPORTE DE COMPROBANTES DEL SRI", colFinal);

                int i = 5;

                if (dia == 0)
                {
                    ws.Cell(i, 1).Value = "AÑO : ";
                    ws.Cell(i, 1).Style.Font.Bold = true;
                    ws.Cell(i, 2).Value = anio.ToString();
                    ws.Cell(i, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell(i, 3).Value = "MES : ";
                    ws.Cell(i, 3).Style.Font.Bold = true;
                    ws.Cell(i, 4).Value = General.getMesName(mes);
                    ws.Cell(i, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
                else
                {
                    var fecha = anio.ToString() + "-" + mes.ToString().PadLeft(2, '0') + "-" + dia.ToString().PadLeft(2, '0');

                    ws.Cell(i, 1).Value = "FECHA : ";
                    ws.Cell(i, 1).Style.Font.Bold = true;
                    ws.Cell(i, 2).Value = fecha;
                }

                var facturas = data.Where(x => x.tipo == "F");

                i = 8;
                int inicio = i;

                if (facturas != null && facturas.Count() > 0)
                {
                    ws.Cell(i, 1).Value = "FACTURAS";
                    
                    colFinal = 9;
                    i++;

                    ws.Cell(i, 1).Value = "FECHA";
                    ws.Cell(i, 2).Value = "COMPROBANTE";
                    ws.Cell(i, 3).Value = "AUTORIZACION";
                    ws.Cell(i, 4).Value = "IDENTIFICACION";
                    ws.Cell(i, 5).Value = "CLIENTE";
                    ws.Cell(i, 6).Value = "SUBTOTAL 0%";
                    ws.Cell(i, 7).Value = "SUBTOTAL IVA";
                    ws.Cell(i, 8).Value = "IVA";
                    ws.Cell(i, 9).Value = "TOTAL";


                    ws.Column(1).Width = 10;
                    ws.Column(2).Width = 20;
                    ws.Column(3).Width = 52;
                    ws.Column(4).Width = 16;
                    ws.Column(5).Width = 40;
                    ws.Column(6).Width = 17;
                    ws.Column(7).Width = 17;
                    ws.Column(8).Width = 17;
                    ws.Column(9).Width = 17;

                    ws.Row(i).Style.Font.Bold = true;

                    this.setFormatCabecera(i, thema, colFinal);
                    i++;
                    foreach (mDocsSri doc in facturas)
                    {
                        ws.Cell(i, 1).Value = doc.fecha;
                        ws.Cell(i, 2).Value = doc.numeroComprobante;
                        ws.Range(i, 3,i, 3).Style.NumberFormat.Format = "@";
                        ws.Cell(i, 3).Value = doc.numeroAutorizacion;
                        ws.Range(i, 4,i, 4).Style.NumberFormat.Format = "@";
                        ws.Cell(i, 4).Value = doc.identificacion;
                        ws.Cell(i, 5).Value = doc.cliente;
                        ws.Cell(i, 6).Value = doc.subNoIva;
                        ws.Cell(i, 7).Value = doc.subConIva;
                        ws.Cell(i, 8).Value = doc.iva;
                        ws.Cell(i, 9).Value = doc.total;

                        i++;
                    }

                    ws.Range(inicio + 1, 1,i - 1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(inicio + 1, 6,i, colFinal).Style.NumberFormat.Format = "###,##0.00";

                    this.setColoresContenido(inicio + 1, i - 1, colFinal, thema);
                    this.setBorders("A" + (inicio + 1) + ":I" + (i - 1));

                    i += 2;
                }

                var retenciones = data.Where(x => x.tipo == "R");

                if (retenciones != null && retenciones.Count() > 0)
                {
                    ws.Cell(i, 1).Value = "RETENCIONES";

                    colFinal = 10;

                    i++;
                    inicio = i;

                    ws.Cell(i, 1).Value = "FECHA";
                    ws.Cell(i, 2).Value = "COMPROBANTE";
                    ws.Cell(i, 3).Value = "AUTORIZACION";
                    ws.Cell(i, 4).Value = "IDENTIFICACION";
                    ws.Cell(i, 5).Value = "PROVEEDOR";
                    ws.Cell(i, 6).Value = "TIPO IMPUESTO";
                    ws.Cell(i, 7).Value = "BASE IMPONIBLE";
                    ws.Cell(i, 8).Value = "PCTJ RETENCION";
                    ws.Cell(i, 9).Value = "IMPTO RETENIDO";
                    ws.Cell(i, 10).Value = "DOC. SUSTENTO";

                    ws.Column(1).Width = 10;
                    ws.Column(2).Width = 20;
                    ws.Column(3).Width = 52;
                    ws.Column(4).Width = 16;
                    ws.Column(5).Width = 40;
                    ws.Column(6).Width = 17;
                    ws.Column(7).Width = 18;
                    ws.Column(8).Width = 17;
                    ws.Column(9).Width = 17;
                    ws.Column(10).Width = 20;

                    ws.Row(i).Style.Font.Bold = true;

                    this.setFormatCabecera(i, thema, colFinal);
                    i++;
                    foreach (mDocsSri doc in retenciones)
                    {
                        ws.Cell(i, 1).Value = doc.fecha;
                        ws.Cell(i, 2).Value = doc.numeroComprobante;
                        ws.Range(i, 3,i, 3).Style.NumberFormat.Format = "@";
                        ws.Cell(i, 3).Value = doc.numeroAutorizacion;
                        ws.Range(i, 4,i, 4).Style.NumberFormat.Format = "@";
                        ws.Cell(i, 4).Value = doc.identificacion;
                        ws.Cell(i, 5).Value = doc.proveedor;
                        ws.Cell(i, 6).Value = doc.tipoImpuesto;
                        ws.Cell(i, 7).Value = doc.baseImponible;
                        ws.Cell(i, 8).Value = doc.porcentajeRetencion;
                        ws.Cell(i, 9).Value = doc.impuestoRetenido;
                        ws.Cell(i, 10).Value = doc.numeroComprobanteAplica;
                        i++;
                    }

                    ws.Range(inicio, 1,i - 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Range(inicio, 7,i, colFinal - 1).Style.NumberFormat.Format = "###,##0.00";

                    this.setColoresContenido(inicio, i - 1, colFinal, thema);
                    this.setBorders("A" + inicio + ":J" + (i - 1));
                    
                    i += 2;
                }


                ws.PageSetup.PrintAreas.Add("A1:K" + i);

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

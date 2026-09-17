using Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibReports
{
    public class RptPedidosPdf: PdfBase
    {
        private string archivo = "";

        public RptPedidosPdf(string connectionString, int tema) : base(connectionString, tema)
        {
            General.creaDirectory();
            archivo = this.generarFileName("rptPedidos");
        }

        public bool crear(string desde, string hasta)
        {
            Pedido pedido = new Pedido(this.connectionString);

            var data = pedido.reportePedidos(desde, hasta);
            string empr = empresa.getNombre();

            try
            {
                this.setEmpresaTitulo("REPORTE DE PEDIDOS");

                AddText("DESDE : ", 25, 760, true);
                AddText("HASTA : ", 250, 760, true);
                AddText(desde, 100, 760, false);
                AddText(hasta, 350, 760, false);


                //builder.SaveToFile(General.reporte + "" + archivo);

                System.Diagnostics.Process.Start(General.reporte + "" + archivo);
                //byte[] docBytes = builder.Build();

                //File.WriteAllBytes(General.reporte + "" +archivo, docBytes);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}

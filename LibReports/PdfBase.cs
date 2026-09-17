using Entidades;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UglyToad.PdfPig;
//using UglyToad.PdfPig.Content;
//using UglyToad.PdfPig.Core;
//using UglyToad.PdfPig.Fonts.Standard14Fonts;
//using UglyToad.PdfPig.Writer;

namespace LibReports
{
    public class PdfBase
    {
        protected Empresa empresa;
        public string NombreEmpresa;
        public string FormatoNumerico = "###.##0,00";
        protected string connectionString = "";

        //protected PdfDocument builder; // PdfDocumentBuilder builder;
        //protected PdfPageBase page; //PdfPageBuilder page;
        //protected PdfFont font; // PdfDocumentBuilder.AddedFont font;
        //protected PdfFont bold; //PdfDocumentBuilder.AddedFont bold;

        //private PdfStringFormat centertAlignment = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
        //private PdfStringFormat leftAlignment = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
        //private PdfStringFormat rightAlignment = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
        //private PdfStringFormat justifyAlignment = new PdfStringFormat(PdfTextAlignment.Justify, PdfVerticalAlignment.Middle);

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

            return archivo + ".pdf";
        }

        public void setEmpresaTitulo(string titulo)
        {
            //PdfSolidBrush brush = new PdfSolidBrush(Color.FromArgb(10, 0, 255, 0));

            //page.Canvas.DrawString(NombreEmpresa, font, brush, 0, 20, centertAlignment);
            //page.Canvas.DrawString(titulo, font, brush, 0, 40, centertAlignment);
            //page.AddText(NombreEmpresa, 14, new PdfPoint(25, 800), bold);
            //page.AddText(titulo, 14, new PdfPoint(25, 780), bold);
        }

        public void AddText(string texto, int x, int y, bool useBold)
        {
           // page.AddText(texto, 12, new PdfPoint(x, y), useBold ? bold : font);
        }

        public void AddTable()
        {
            //PdfGrid grid = new PdfGrid();
        }

        public PdfBase(string connectionString, int tema)
        {
            this.connectionString = connectionString;
            empresa = new Empresa(this.connectionString);

            //builder = new PdfDocument();
            //page = builder.Pages.Add();

            //font = new PdfFont(PdfFontFamily.Helvetica, 16);
            //bold = new PdfFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            //builder = new PdfDocumentBuilder();
            //page = builder.AddPage(PageSize.A4);
            //font = builder.AddStandard14Font(Standard14Font.Helvetica);
            //bold = builder.AddStandard14Font(Standard14Font.HelveticaBold);

        }
    }
}

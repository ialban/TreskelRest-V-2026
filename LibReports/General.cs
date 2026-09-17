using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibReports
{
    class General
    {
        private static string directorio = Directory.GetCurrentDirectory();
        public static string reporte = directorio + "\\reportes\\";        

        public static void creaDirectory()
        {
            if (!Directory.Exists(directorio + "\\reportes"))
            {
                Directory.CreateDirectory("reportes");
            }
        }

        public static void EliminaArchivo(string archivo)
        {
            try
            {
                File.Delete(archivo);
            }
            catch(Exception e)
            {
                
            }            
        }

        public static string getMesName(int mes)
        {
            string[] meses = {"ENERO", "FEBRERO", "MARZO", "ABRIL",
                              "MAYO", "JUNIO", "JULIO", "AGOSTO",
                              "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"};
            return meses[mes - 1];
        }
    }
}

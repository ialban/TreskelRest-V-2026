using Entidades;
using Entidades.modelos;
using LibPrint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestPrinter
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            EposPrint print = new EposPrint();
            String texto = textBox.Text;
            print.ImprimirTest(texto);


            //print.TestImpresion();
        }
        
        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            EposPrint print = new EposPrint();

            print.cantidadLineas = 5;
            print.printCajaIzq = true;

            String texto = textBox.Text;
            
            //print.ImprimirTest(texto);
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn { ColumnName = "itemNbr" });
            dt.Columns.Add(new DataColumn { ColumnName = "itemCnt" });
            dt.Columns.Add(new DataColumn { ColumnName = "itemPrc" });
            dt.Columns.Add(new DataColumn { ColumnName = "itemIva" });

            for(int i = 0; i< 5; i++) {
                DataRow dr = dt.NewRow();


                dr["itemNbr"] = "POLLO CURRY";
                dr["itemCnt"] = "1";
                dr["itemPrc"] = "5";
                dr["itemIva"] = "0";

                dt.Rows.Add(dr);
            }

            //print.fnFacturaPrnt(dt, null, "625", "002", "10", "0", "1", "11", "0914556634", "UNIDAD EDUCATIVA PARTICULAR SALESIANO CRISTOBAL COLON", "PRADERA 2 MZ D35 VILLA 71", "123456", "0", "0", "F");
            List<ProdPedd> dc = new List<ProdPedd>();
            dc.Add(new ProdPedd
            {
                cantidad = 2,
                producto = "Salteado de Pollo con salsa de champiñones",
                noIngredientes = new List<mIngredientes>{ new mIngredientes { codigo = "001", nombre = "CEBOLLA" },
                 new mIngredientes { codigo = "002", nombre = "PIMIENTO" }}
            });

            //print.fnCocinaPrnt(dc, "002 A", "01/02/2020 08:35", "L", "6257", "PASTUDILLO");
            //print.fnDataPeddDmclo("002 A", "IGNACIO ALBAN", "433562", "PRADERA 2 MZ D75 VILLA 823", "CERCA DE LA DESPENSA MERCEDITA, A DOS VILLAS ENTRANDO POR LA PEATONAL 2");

            print.IniciaImpresion();
        }
    }
}

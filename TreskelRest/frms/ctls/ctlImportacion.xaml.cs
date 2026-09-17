using Entidades;
using Entidades.modelos;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlImportacion.xaml
    /// </summary>
    public partial class ctlImportacion : UserControl, IDisposable
    {
        private bool disposedValue;

        public ctlImportacion()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (txtItems.Text != null)
            {
                try
                {
                    var reader = new StreamReader(File.OpenRead(txtItems.Text));
                    List<mItems> items = new List<mItems>();

                    char separador = ',';

                    switch (cboSeparador.SelectedIndex)
                    {
                        case 0:
                            separador = ',';
                            break;
                        case 1:
                            separador = ';';
                            break;
                        case 2:
                            separador = '\t';
                            break;
                    }

                    bool isFirstLine = true;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(separador);

                        if (chkUseHead.IsChecked == true && isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }

                        items.Add(new mItems
                        {
                            /*
                            0 => CODIGO,
                            1 => DESCRIPCION,
                            2 => GRUPO,
                            3 => SUBGRUPO,
                            4 => PRECIO,
                            5 => UNIDAD,
                            6 => G IVA,
                            7 => I IVA,
                            8 => BARCODE,
                            9 => VISIBLE PARA
                             */
                            itemcod = values[0],
                            itemnbr = values[1],
                            grupo = values[2],
                            subgrupo = values[3],
                            itemprc = General.StringToDouble(values[4]),
                            itemund = values[5],
                            itemgiva = values[6] == "1",
                            itemiva = values[7] == "1",
                            barcode = values[8],
                            itempara = values[9]
                        });
                    }

                    reader.Close();
                    // Grabar a base de datos
                    Items item = new Items(General.connectionString);
                    Respuesta respuesta = item.grabar(items);

                    ObjComportamiento.showMuestraMensaje(respuesta);
                }
                catch(Exception er)
                {
                    ObjComportamiento.showMsgError("Ocurrió un problema al importar, verifique si el archivo tiene el formato correcto.");
                }
            }
        }

        private void btnItems_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selección del logotipo para la factura";
            openFileDialog.Filter = "Imagenes (*.csv;)|*.csv;";
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                try
                {
                    txtItems.Text = fileName;
                }
                catch (OutOfMemoryException err)
                {
                    ObjComportamiento.showMsgError(err.Message);
                }
                catch (Exception err)
                {
                    ObjComportamiento.showMsgError(err.Message);
                }
            }
        }

        private void btnItemsInfo_Click(object sender, RoutedEventArgs e)
        {
            var newLine = System.Environment.NewLine;

            string mensaje = "El formato del archivo para importación de Items debe ser el siguiente:" + newLine;
            mensaje += "" + newLine;
            mensaje += "    COD,DESC,GRP,SGRP,PRC,UND,GIVA,IIVA,BARC" + newLine;
            mensaje += "    6010,chuleta,CERDO,CERDO,7.49,KG,1,1,78634562345" + newLine;
            mensaje += "    ####,TEXTO,TEXTO,TEXTO,DECIMAL,TEXTO,B,B,TEXTO" + newLine;
            mensaje += "" + newLine;
            mensaje += "Recuerde que:" + newLine;
            mensaje += "- La extensión del archivo debe ser csv, por ejemplo: mi_archivo.csv" + newLine;
            mensaje += "- La cabecera puede ser omitida en caso de así desearlo" + newLine;
            mensaje += "- El separador de campos puede ser: coma, punto y coma o un tabulador" + newLine;
            mensaje += "- Los números decimales usarán un PUNTO y deberán tener 2 decimales, por ejemplo: 4.73" + newLine;
            mensaje += "- El campo UND debe tener los valores UND, KG, LB o GR" + newLine;
            mensaje += "- Los campos para GIVA e IIVA, deben tener el valor de 1 o 0" + newLine;
            mensaje += "- El campo BARC es opcional, si desea puede dejarlo en blanco" + newLine;

            ObjComportamiento.showMsgInfo(mensaje);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~ctlImportacion()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

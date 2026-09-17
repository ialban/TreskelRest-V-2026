using System.Windows;
using System.Windows.Controls;
using TreskelRest.cls.Common;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlTipPrints.xaml
    /// </summary>
    public partial class ctlTipPrints : UserControl
    {
        public Window contenedor { get; set; }

        public event AceptaTiposImpresoras aceptaTiposImpresoras;
        public delegate void AceptaTiposImpresoras(ImpresoraDestino impresorasDestino);

        private ImpresoraDestino valoresIniciales;

        public ImpresoraDestino ValoresIniciales
        {
            set
            {
                setTipo(cboTipoFact, value.Facturacion.tipo);
                setTipo(cboTipoCocina, value.Cocina.tipo);
                setTipo(cboTipoParrilla, value.Parrilla.tipo);
                setTipo(cboTipoBar, value.Bar.tipo);

                setMarca(cboMarcaFact, value.Facturacion.marca);
                setMarca(cboMarcaCocina, value.Cocina.marca);
                setMarca(cboMarcaParrilla, value.Parrilla.marca);
                setMarca(cboMarcaBar, value.Bar.marca);

                setUsaEspacio(chkEspFact, value.Facturacion.usaEspacios);
                setUsaEspacio(chkEspCocina, value.Cocina.usaEspacios);
                setUsaEspacio(chkEspParrilla, value.Parrilla.usaEspacios);
                setUsaEspacio(chkEspBar, value.Bar.usaEspacios);
            }
        }

        public ctlTipPrints()
        {
            InitializeComponent();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            ImpresoraDestino destino = new ImpresoraDestino();

            destino.Facturacion = getAccionesImpresora(cboTipoFact, cboMarcaFact, chkEspFact, null);
            destino.Cocina = getAccionesImpresora(cboTipoCocina, cboMarcaCocina, chkEspCocina, chkBigCocina);
            destino.Parrilla = getAccionesImpresora(cboTipoParrilla, cboMarcaParrilla, chkEspParrilla, chkBigCocina);
            destino.Bar = getAccionesImpresora(cboTipoBar, cboMarcaBar, chkEspBar, chkBigCocina);

            aceptaTiposImpresoras(destino);
            contenedor.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        // -------------------------------------------------
        public AccionesImpresoras getAccionesImpresora(ComboBox tipo, ComboBox marca, CheckBox espacios, CheckBox letraGrande)
        {
            return new AccionesImpresoras
            {
                tipo = tipo.SelectedIndex == 0 ? "I" : "T",
                marca = getMarca(marca.SelectedIndex),
                usaEspacios = espacios.IsChecked == true ? "1" : "0",
                letraGrande = letraGrande == null ? "0" : (letraGrande.IsChecked == true ? "1" : "0")
            };
        }

        private string getMarca(int index)
        {
            string marca = "";

            switch (index)
            {
                case 0:
                    marca = "Epson";
                    break;
                case 1:
                    marca = "SAT";
                    break;
                case 2:
                    marca = "ZKTeco";
                    break;
            }

            return marca;
        }

        private void setTipo(ComboBox cbo, string tipo)
        {
            switch (tipo)
            {
                case "T":
                    cbo.SelectedIndex = 1;
                    break;
                case "I":
                default:
                    cbo.SelectedIndex = 0;
                    break;
            }
        }

        private void setMarca(ComboBox cbo, string marca)
        {
            switch (marca)
            {
                case "SAT":
                    cbo.SelectedIndex = 1;
                    break;
                case "ZKTeco":
                    cbo.SelectedIndex = 2;
                    break;
                case "Epson":
                default:
                    cbo.SelectedIndex = 0;
                    break;
            }
        }

        private void setUsaEspacio(CheckBox chk, string valor)
        {
            switch (valor)
            {
                case "1":
                    chk.IsChecked = true;
                    break;
                case "0":
                default:
                    chk.IsChecked = false;
                    break;
            }
        }
    }
}

using Entidades;
using Entidades.modelos;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para ctlCreaBancos.xaml
    /// </summary>
    public partial class ctlCreaBancos : UserControl
    {
        private winGeneral win = new winGeneral();
        public Window contenedor { get; set; }
        public ctlBancos parent { get; set; } = null;

        public string accion { get; set; } = "N";
        private string Banco = "";
        private string nombreBanco = "";
        private clsTextBox clsCuenta;
        private bool bcoSelect = false;

        public ctlCreaBancos()
        {
            InitializeComponent();

            clsCuenta = new clsTextBox(txtCuenta, General.USA_TECLADO_PANTALLA);

            clsCuenta.caracteresPermitidos = clsTextBox.CARACTERES_PERMITIDOS.NUMERICOS;
            clsCuenta.setMaxLength(20);
            clsCuenta.permiteEspacios = false;
        }

        public void setNid(int codigo)
        {
            Bancos bancos = new Bancos(General.connectionString);

            mBancos banco = bancos.getById(codigo);

            if (banco != null)
            {
                txtCuenta.Text = banco.bcosnct;
                txtCuenta.Tag = codigo;

                if (banco.bcosuso == "I")
                {
                    rbInstitucional.IsChecked = true;
                }
                else
                {
                    rbPersonal.IsChecked = true;
                }

                if (banco.bcostip == "A")
                {
                    rbAhorro.IsChecked = true;
                }
                else
                {
                    rbCorriente.IsChecked = true;
                }

                chkPrimaria.IsChecked = banco.bcospri;
                chkActivo.IsChecked = banco.bcosatv;
            }
            
            foreach (var lbancos in lsBancos.Children)
            {
                var btn = (StackPanel)lbancos;
                foreach (var pnl in btn.Children)
                {
                    foreach (var tl in ((StackPanel)pnl).Children)
                    {
                        var tgl = (ToggleButton)tl;
                        tgl.IsChecked = false;
                        if (tgl.Tag.ToString() == banco.bcosacr)
                        {
                            tgl.IsChecked = true;
                        }
                    }
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            contenedor.Close();
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = "";
            if (txtCuenta.Text.Trim() == "")
            {
                mensaje += "- Debe ingresar un número de cuenta";
            }

            if (!bcoSelect)
            {
                mensaje += "- Debe seleccionar una entidad bancaria";
            }

            if (mensaje == "")
            {
                ObjComportamiento.showMsgError(mensaje);
                return;
            }

            if (ObjComportamiento.showMsgYesNo("¿Desea grabar la entidad bancaria ingresada?") == MessageBoxResult.Yes)
            {
                Bancos bancos = new Bancos(General.connectionString);

                bancos.bcosnid = int.Parse(txtCuenta.Tag.ToString());
                bancos.bcosnbr = nombreBanco.ToUpper();
                bancos.bcosnct = txtCuenta.Text;
                bancos.bcosacr = Banco;
                bancos.bcostip = rbCorriente.IsChecked == true ? "C" : "A";
                bancos.bcosuso = rbInstitucional.IsChecked == true ? "I" : "P";
                bancos.bcospri = chkPrimaria.IsChecked == true ? true: false;
                bancos.bcosatv = chkActivo.IsChecked == true ? true : false;

                var respuesta = bancos.grabar(accion);

                if (respuesta.resultado)
                {
                    ObjComportamiento.showMsgInfo(respuesta.mensaje);
                    limpiar();
                }
                else
                {
                    ObjComportamiento.showMsgError(respuesta.mensaje);
                }
            }
        }

        private void Banco_Checked(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Tag.ToString())
            {
                case "BA":
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoAustro.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BA";
                    nombreBanco = "Bco. del Austro";
                    break;
                case "BB":
                    BcoAustro.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoBolivariano.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BB";
                    nombreBanco = "Bco. Bolivariano";
                    break;
                case "BG":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoGquil.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BG";
                    nombreBanco = "Bco. Guayaquil";
                    break;
                case "BI":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoInternacional.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BI";
                    nombreBanco = "Bco. Internacional";
                    break;
                case "BM":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoMachala.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BM";
                    nombreBanco = "Bco. de Machala";
                    break;
                case "PB":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoPacifico.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "PB";
                    nombreBanco = "Bco. del Pacífico";
                    break;
                case "BP":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoPichincha.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BP";
                    nombreBanco = "Bco. de Pichincha";
                    break;
                case "BD":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoProdubanco.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BD";
                    nombreBanco = "Bco. Produbancho";
                    break;
                case "BR":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    BcoRuminahui.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BR";
                    nombreBanco = "Bco. Rumiñahui";
                    break;
                case "BS":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = true;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "BS";
                    nombreBanco = "Bco. Solidario";
                    break;
                case "MP":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = true;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "MP";
                    nombreBanco = "Bco. Mutualista Pichincha";
                    break;
                case "MA":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = true;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "MA";
                    nombreBanco = "Bco. de Ambato";
                    break;
                case "CP":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = true;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "CP";
                    nombreBanco = "Cooperativa Policía Nacional";
                    break;
                case "CO":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = true;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "CO";
                    nombreBanco = "Cooperativa Oscus";
                    break;
                case "CJ":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = true;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = false;
                    this.Banco = "CJ";
                    nombreBanco = "Cooperativa Jep";
                    break;
                case "29":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = true;
                    BanEc.IsChecked = false;
                    this.Banco = "29";
                    nombreBanco = "Cooperativa 29 de Octubre";
                    break;
                case "BE":
                    BcoAustro.IsChecked = false;
                    BcoBolivariano.IsChecked = false;
                    BcoGquil.IsChecked = false;
                    BcoInternacional.IsChecked = false;
                    BcoMachala.IsChecked = false;
                    BcoPacifico.IsChecked = false;
                    BcoPichincha.IsChecked = false;
                    BcoProdubanco.IsChecked = false;
                    BcoRuminahui.IsChecked = false;
                    BcoSolidario.IsChecked = false;
                    MutPichincha.IsChecked = false;
                    MAmbato.IsChecked = false;
                    CoopPN.IsChecked = false;
                    CoopOscus.IsChecked = false;
                    CoopJep.IsChecked = false;
                    Coop29.IsChecked = false;
                    BanEc.IsChecked = true;
                    this.Banco = "BE";
                    nombreBanco = "Bco. Ecuador";
                    break;
            }
            ubicaCursor();
        }

        private void Banco_Unchecked(object sender, RoutedEventArgs e)
        {
            BcoAustro.IsChecked = false;
            BcoBolivariano.IsChecked = false;
            BcoGquil.IsChecked = false;
            BcoInternacional.IsChecked = false;
            BcoMachala.IsChecked = false;
            BcoPacifico.IsChecked = false;
            BcoPichincha.IsChecked = false;
            BcoProdubanco.IsChecked = false;
            BcoRuminahui.IsChecked = false;
            BcoSolidario.IsChecked = false;
            this.Banco = "";
            nombreBanco = "";
            ubicaCursor();
        }

        public void ubicaCursor()
        {
            txtCuenta.Focus();
        }

        private void SeleccionTipoCuenta_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SeleccionMotivo_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCls_Click(object sender, RoutedEventArgs e)
        {
            txtCuenta.Text = "";
            txtCuenta.Focus();
        }

        private void limpiar()
        {
            foreach (var lbancos in lsBancos.Children)
            {
                var btn = (StackPanel)lbancos;
                foreach(var pnl in btn.Children)
                {
                    foreach(var tl in ((StackPanel)pnl).Children)
                    {
                        var tgl = (ToggleButton)tl;
                        tgl.IsChecked = false;
                    }
                }
            }

            txtCuenta.Text = "";
            rbCorriente.IsChecked = true;
            rbInstitucional.IsChecked = true;
            chkActivo.IsChecked = true;
            chkPrimaria.IsChecked = true;
        }
    }
}

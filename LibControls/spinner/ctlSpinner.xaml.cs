using System;
using System.Collections.Generic;
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

namespace LibControls.spinner
{
    /// <summary>
    /// Interaction logic for ctlSpinner.xaml
    /// </summary>
    public partial class ctlSpinner : UserControl
    {
        public int ValorMinimo { get; set; } = 0;
        public int ValorMaximo { get; set; } = 999999999;
        public int incremento { get; set; } = 1;
        public ctlSpinner()
        {
            InitializeComponent();
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            int valor = int.Parse(txt.Text) + incremento;

            if(valor>=ValorMinimo && valor <= ValorMaximo)
            {
                txt.Text = valor.ToString();
            }
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            int valor = int.Parse(txt.Text) - incremento;

            if (valor >= ValorMinimo && valor <= ValorMaximo)
            {
                txt.Text = valor.ToString();
            }
        }

        public string getValue()
        {
            return txt.Text;
        }

        public void setValue(string value)
        {
            txt.Text = value;
        }
    }
}

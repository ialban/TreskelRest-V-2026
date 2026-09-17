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

namespace LibControls.timepicker
{
    /// <summary>
    /// Lógica de interacción para ctlTimePicker.xaml
    /// </summary>
    public partial class ctlTimePicker : UserControl
    {
        public ctlTimePicker()
        {
            InitializeComponent();
        }

        public string getValue()
        {
            return hinicio.Text;
        }

        public void setValue(string value)
        {
            hinicio.Text = value;

            var tiempo = value.Split(':');
            lHora.Content = tiempo[0];
            lMinuto.Content = tiempo[1];
            sHora.Value = Utils.StringToDouble(tiempo[0]);
            sMinuto.Value = Utils.StringToDouble(tiempo[1]);
        }

        private void hInicioOk_Click(object sender, RoutedEventArgs e)
        {
            hinicio.Text = lHora.Content + ":" + lMinuto.Content;
            popupControl.IsOpen = false;
        }

        private void horaUp_Click(object sender, RoutedEventArgs e)
        {
            int hora = int.Parse(lHora.Content.ToString());

            if (hora + 1 > 23)
            {
                return;
            }

            hora++;
            lHora.Content = String.Format("{0:00}", hora);
            sHora.Value = hora;
        }

        private void MinutoUp_Click(object sender, RoutedEventArgs e)
        {
            int minuto = int.Parse(lMinuto.Content.ToString());

            if (minuto + 1 > 59)
            {
                return;
            }

            minuto++;
            lMinuto.Content = String.Format("{0:00}", minuto);
            sMinuto.Value = minuto;
        }

        private void horaDown_Click(object sender, RoutedEventArgs e)
        {
            int hora = int.Parse(lHora.Content.ToString());

            if (hora - 1 < 0)
            {
                return;
            }

            hora--;
            lHora.Content = String.Format("{0:00}", hora);
            sHora.Value = hora;
        }

        private void MinutoDown_Click(object sender, RoutedEventArgs e)
        {
            int minuto = int.Parse(lMinuto.Content.ToString());

            if (minuto - 1 < 0)
            {
                return;
            }

            minuto--;
            lMinuto.Content = String.Format("{0:00}", minuto);
            sMinuto.Value = minuto;
        }

        private void sHora_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lHora.Content = String.Format("{0:00}", sHora.Value);
        }

        private void sMinuto_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lMinuto.Content = String.Format("{0:00}", sMinuto.Value);
        }
    }
}

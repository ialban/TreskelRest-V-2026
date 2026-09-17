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

namespace LibKeys
{
    /// <summary>
    /// Interaction logic for gridKeysControl.xaml
    /// </summary>
    public partial class gridKeysControl : Window
    {
        private string codigo = "";

        public event RetornaValidacion retornaValidacion;
        public delegate void RetornaValidacion(Boolean resultado);

        private enum posicionFocus
        {
            TEXTO1,
            TEXTO2,
            TEXTO3
        }

        private posicionFocus posFocus = posicionFocus.TEXTO1;

        public gridKeysControl()
        {
            InitializeComponent();

            this.Height = 400;
            this.Width = 620;

            Limpia();
            this.generaTablero();
            this.generaRotulos();

            texto1.Focus();            
        }

        public void Limpia()
        {
            texto1.Password = "";
            texto2.Password = "";
            texto3.Password = "";

            Grid.Children.Clear();
        }

        public int RandomBetween(int min, int max)
        {
            Random ramdom = new Random();

            return ramdom.Next(min, max);
        }

        public void generaRotulos()
        {
            string letras = "";

            for (int i = 0; i < 3; i++)
            {
                Boolean existe = false;
                while (!existe)
                {
                    string letra = ((char)RandomBetween(65, 75)).ToString();
                    if (!letras.Contains(letra))
                    {
                        letras += letra;

                        int numero = RandomBetween(1, 6);

                        if (i == 0)
                            rotulo1.Content = letra + numero;
                        if (i == 1)
                            rotulo2.Content = letra + numero;
                        if (i == 2)
                            rotulo3.Content = letra + numero;

                        existe = true;
                    }
                }
            }            
        }

        public void generaTablero()
        {
            string numeros = "";

            for(int i=0; i<10; i++)
            {
                Boolean existe = false;
                while (!existe)
                {
                    int nuevo = RandomBetween(0, 10);

                    if (!numeros.Contains(nuevo.ToString()))
                    {
                        numeros += nuevo.ToString();

                        Button btn = new Button();
                        btn.Tag = nuevo.ToString();

                        StackPanel sp = new StackPanel();
                        sp.Orientation = Orientation.Vertical;

                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/images/PIC_" + nuevo.ToString() + ".png"));

                        sp.Children.Add(img);
                        btn.Content = sp;

                        btn.Click += new RoutedEventHandler(Button_Click);

                        Grid.Children.Add(btn);
                        existe = true;
                    }
                }                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string valor = ((Button)sender).Tag.ToString();
            if (codigo.Length<6)
                codigo += valor;

            if (codigo.Length <= 2)
            {                
                this.posFocus = posicionFocus.TEXTO1;
                texto1.Password += valor;
            }

            if (codigo.Length > 2 && codigo.Length<=4)
            {
                this.posFocus = posicionFocus.TEXTO2;
                texto2.Password += valor;
            }

            if (codigo.Length > 4)
            {
                this.posFocus = posicionFocus.TEXTO3;
                if (texto3.Password.Length < 2)
                {
                    texto3.Password += valor;
                }                
            }
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            texto1.Password = "";
            texto2.Password = "";
            texto3.Password = "";

            codigo = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (this.posFocus == posicionFocus.TEXTO1)
            {
                if (texto1.Password != "") 
                    texto1.Password = texto1.Password.Substring(0, texto1.Password.Length - 1);                
            }

            if (this.posFocus == posicionFocus.TEXTO2)
            {                
                if (texto2.Password == "")
                    this.posFocus = posicionFocus.TEXTO1;
                else
                {
                    texto2.Password = texto2.Password.Substring(0, texto2.Password.Length - 1);
                    if (texto2.Password == "")
                        this.posFocus = posicionFocus.TEXTO1;
                }                       
            }

            if (this.posFocus == posicionFocus.TEXTO3)
            {                
                if (texto3.Password == "")
                    this.posFocus = posicionFocus.TEXTO2;
                else
                {
                    texto3.Password = texto3.Password.Substring(0, texto3.Password.Length - 1);
                    if (texto3.Password == "")
                        this.posFocus = posicionFocus.TEXTO2;
                }
            }

            this.codigo = texto1.Password.ToString() + texto2.Password.ToString() + texto3.Password.ToString();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            GridKeys key = new GridKeys();

            string[] rotulos =
            {
                rotulo1.Content.ToString(),
                rotulo2.Content.ToString(),
                rotulo3.Content.ToString()
            };

            string[] valores =
            {
                texto1.Password.ToString(),
                texto2.Password.ToString(),
                texto3.Password.ToString()
            };

            Boolean correcto = key.validar(rotulos, valores);

            retornaValidacion(correcto);

            this.Hide();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Limpia();
            this.generaTablero();
            this.generaRotulos();
        }
    }
}

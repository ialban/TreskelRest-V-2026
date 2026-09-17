using LibControls.spinner;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LibKeys
{
    /// <summary>
    /// Interaction logic for NumKeyboard.xaml
    /// </summary>
    public partial class NumKeyboard : Window
    {
        public event RetornaTexto retornaTexto;
        public delegate void RetornaTexto(Object txt, String texto);

        private Object _txtBox;

        private int maxLength = 50;

        public int MaxLength
        {
            set
            {
                maxLength = value;
                texto.MaxLength = value;
            }
        }

        public Object setTextBox
        {
            set
            {
                _txtBox = value;
                if (_txtBox.GetType() == typeof(TextBox))
                {
                    texto.Text = ((TextBox)_txtBox).Text;
                }

                if (_txtBox.GetType() == typeof(PasswordBox))
                {
                    texto.Text = ((PasswordBox)_txtBox).Password;
                    texto.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#password");
                }

                if (_txtBox.GetType() == typeof(ctlSpinner))
                {
                    texto.Text = ((ctlSpinner)_txtBox).getValue();
                }
            }
        }

        public NumKeyboard()
        {
            InitializeComponent();

            this.Height = 390;
            this.Width = 320;

            //this.IsVisibleChanged += DependencyPropertyChangedEventHandler(NumKeyboard_IsVisible);

        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (texto.Text.Length == maxLength)
            {
                texto.Focus();
                return;
            }

            int _idx = texto.SelectionStart;
            String txt1 = "";
            String txt2 = "";

            if (_idx >= 0 && _idx < texto.Text.Length)
            {
                txt1 = texto.Text.Substring(0, _idx);
                txt2 = texto.Text.Substring(_idx);

                if (!((Button)sender).Tag.ToString().Equals(""))
                {
                    texto.Text = txt1 + ((Button)sender).Tag.ToString() + txt2;
                    texto.SelectionStart = _idx + ((Button)sender).Tag.ToString().Length;
                }
                else
                {
                    texto.Text = txt1 + ((Button)sender).Content.ToString() + txt2;
                    texto.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
                }
            }
            else
            {
                if (!((Button)sender).Tag.ToString().Equals(""))
                {
                    texto.Text += ((Button)sender).Tag.ToString();
                    texto.SelectionStart = _idx + ((Button)sender).Tag.ToString().Length;
                }
                else
                {
                    texto.Text += ((Button)sender).Content.ToString();
                    texto.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
                }
            }
            texto.Focus();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            retornaTexto(this._txtBox, texto.Text.ToString());
            this.Hide();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            String _txt = texto.Text.ToString();
            int _idx = texto.SelectionStart;

            if (texto.SelectionLength > 0)
            {
                texto.Text = texto.Text.Remove(_idx, texto.SelectionLength);
                texto.Focus();
                return;
            }

            if (_txt.Length > 0)
            {
                if (_idx > 0 && _idx < texto.Text.Length)
                {
                    String txt1 = texto.Text.Substring(0, _idx);
                    String txt2 = texto.Text.Substring(_idx);

                    txt1 = txt1.Remove(txt1.Length - 1, 1);
                    texto.Text = txt1 + txt2;
                }
                else
                {
                    if (_idx == 0) return;
                    texto.Text = _txt.Substring(0, _txt.Length - 1);
                }
                texto.SelectionStart = (_idx - 1) > 0 ? _idx - 1 : 0;
            }
            texto.Focus();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            texto.Text = "";
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void UniformGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Boolean)e.NewValue)
            {
                texto.Focus();
            }
        }

        private void texto_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                enter.Focus();
            }
        }
    }
}

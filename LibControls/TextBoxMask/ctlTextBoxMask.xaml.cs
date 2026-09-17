using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace LibControls.TextBoxMask
{
    /// <summary>
    /// Lógica de interacción para ctlTextBoxMask.xaml
    /// </summary>
    public partial class ctlTextBoxMask : UserControl
    {
        private bool _showPlaceHolder = false;
        private string _mask = "";
        private string pattern = "";
        private bool _tieneError = false;
        private string _separador = "";

        public string Text
        {
            get
            {
                return txtContent.Text;
            }

            set
            {
                txtContent.Text = value;
            }
        }

        public double Width
        {
            get
            {
                return txtContent.Width;
            }

            set
            {
                this.Width = value;
                txtContent.Width = value;
                lblPlaceHolder.Width = value;
            }
        }

        public string Separador
        {
            get{
                return _separador;
            }
            set
            {
                _separador = value;
            }
        }
        private void showPlaceHolderText()
        {
            lblPlaceHolder.Visibility = Visibility.Visible;
            lblPlaceHolder.Content = _mask;
            lblPlaceHolder.Opacity = 0.5;
            lblPlaceHolder.Background = null;
        }

        public bool ShowPlaceHolder
        {
            get
            {
                return _showPlaceHolder;
            }

            set
            {
                _showPlaceHolder = value;

                if (value)
                {
                    if (!string.IsNullOrEmpty(_mask))
                    {
                        showPlaceHolderText();
                    }
                }
                else
                {
                    lblPlaceHolder.Visibility = Visibility.Hidden;
                }
            }
        }

        public string Mask
        {
            get
            {
                return _mask;
            }

            set
            {
                _mask = value;
                if (_mask.Contains("#"))
                {
                    if (!string.IsNullOrEmpty(_separador))
                    {
                        string[] aMask = _mask.Split(_separador.ToCharArray().First());
                        if (aMask.Count() > 1)
                        {
                            foreach (string msk in aMask)
                            {
                                int cnt = msk.Length;
                                pattern += "\\s{0,9}\\d{" + cnt + "}.";
                            }
                            pattern = pattern.Substring(0, pattern.Length - 1);
                        }
                        else
                        {
                            int cnt = aMask.First().Length;
                            pattern += "\\s{0,9}\\d{" + cnt + "}";
                        }
                    }
                }

                if (_showPlaceHolder)
                {
                    if (!string.IsNullOrEmpty(_mask))
                    {
                        showPlaceHolderText();
                    }
                }
            }
        }

        public bool HasError
        {
            get
            {
                return _tieneError;
            }
        }

        public ctlTextBoxMask()
        {
            InitializeComponent();
        }

        private void txtContent_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void txtContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ShowPlaceHolder && !string.IsNullOrEmpty(_mask))
            {
                if (Regex.IsMatch(txtContent.Text, pattern))
                {
                    _tieneError = false;
                }
                else
                {
                    _tieneError = true;
                }
            }
            else
            {
                _tieneError = true;
            }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibKeys
{
    public class clsTextBox : TextBox
    {
        public enum CARACTERES_PERMITIDOS
        {
            TODOS,
            NUMERICOS,
            DECIMALES,
            LETRAS,
            NUMERICOS_LETRAS,
            CORREO,
            USUARIOS,
            CLAVES,
            SERVIDOR,
            URL
        }
        public event MuestraTeclado muestraTeclado;
        public delegate void MuestraTeclado(object sender);

        public Boolean tecladoPantalla { get; set; } = false;
        public CARACTERES_PERMITIDOS caracteresPermitidos { get; set; } = CARACTERES_PERMITIDOS.TODOS;
        public string otrosCaracteresPermitidos { get; set; } = "";
        public Boolean permiteEspacios { get; set; } = false;
        public Boolean permitePorcentaje { get; set; } = false;
        public Boolean permitePunto { get; set; } = false;
        public int cantidaDecimales = 2;
        
        private object _txt = null;
        private int _minLength = 0;

        public clsTextBox(object obj, Boolean tecladoPantalla)
        {
            this._txt = obj;
            this.tecladoPantalla = tecladoPantalla;

            if (this._txt.GetType() == typeof(PasswordBox))
            {
                ((PasswordBox)this._txt).KeyUp += new KeyEventHandler(clsTextBox_KeyUp);
                ((PasswordBox)this._txt).PreviewKeyDown += new KeyEventHandler(clsTextBox_PreviewKeyDown);
                ((PasswordBox)this._txt).PasswordChanged += new RoutedEventHandler(clsTextBox_PasswordChanged);
            }

            if (this._txt.GetType() == typeof(TextBox))
            {
                ((TextBox)this._txt).KeyUp += new KeyEventHandler(clsTextBox_KeyUp);
                ((TextBox)this._txt).PreviewKeyUp += new KeyEventHandler(clsTextBox_PreviewKeyDown);
                ((TextBox)this._txt).TextChanged += new TextChangedEventHandler(clsTextBox_TextChanged);
            }
        }

        public clsTextBox(object obj, Boolean tecladoPantalla, CARACTERES_PERMITIDOS permitidos, int maxLength, bool permiteEspacios = false, string otrosCaracteres = "")
        {
            this._txt = obj;
            this.tecladoPantalla = tecladoPantalla;
            this.setMaxLength(maxLength);

            if (this._txt.GetType() == typeof(PasswordBox))
            {
                ((PasswordBox)this._txt).KeyUp += new KeyEventHandler(clsTextBox_KeyUp);
                ((PasswordBox)this._txt).PreviewKeyDown += new KeyEventHandler(clsTextBox_PreviewKeyDown);
                ((PasswordBox)this._txt).PasswordChanged += new RoutedEventHandler(clsTextBox_PasswordChanged);
            }

            if (this._txt.GetType() == typeof(TextBox))
            {
                ((TextBox)this._txt).KeyUp += new KeyEventHandler(clsTextBox_KeyUp);
                ((TextBox)this._txt).PreviewKeyUp += new KeyEventHandler(clsTextBox_PreviewKeyDown);
                ((TextBox)this._txt).TextChanged += new TextChangedEventHandler(clsTextBox_TextChanged);
            }

            this.caracteresPermitidos = permitidos;
            if (permiteEspacios) this.permiteEspacios = permiteEspacios;
            if (otrosCaracteres != "") this.otrosCaracteresPermitidos = otrosCaracteres;
        }

        public void setMaxLength(int cantidad)
        {
            if (this._txt.GetType() == typeof(PasswordBox))
            {
                ((PasswordBox)this._txt).MaxLength = cantidad;
            }

            if (this._txt.GetType() == typeof(TextBox))
            {
                ((TextBox)this._txt).MaxLength = cantidad;
            }
        }

        public int getMinLength()
        {
            return _minLength;
        }

        public void setMinLength(int cantidad)
        {
            _minLength = cantidad;
        }

        public new void GotFocus(object sender, EventArgs e)
        {
            if (this.tecladoPantalla)
            {
                muestraTeclado(this._txt);
            }
        }        

        private void clsTextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        public string removerTextoNoPermitido(string texto, string permitidos)
        {
            var longitud = texto.Length;
            string resultado = "";

            for (int i=0; i< longitud; i++)
            {
                var letra = texto.Substring(i, 1);
                if (permitidos.Contains(letra))
                {
                    resultado += letra; 
                }
            }

            return resultado;
        }

        private String removerTexto(string texto, string actual, string permitidos)
        {
            if (permitidos.IndexOf(actual)>=0)
            {
                return texto;
            }else
            {
                return texto.Replace(actual, "");
            }            
        }

        private void clsTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string texto = "";

            if (this._txt.GetType() == typeof(PasswordBox))
            {
                texto = ((PasswordBox)this._txt).Password.ToString();
            }

            string permitidos = "";
            int cnt = texto.Length;

            if (cnt > 0)
            {
                permitidos = "abcdefghijklmnopqrstuvwxyyz";
                permitidos += permitidos.ToUpper();
                permitidos += ".-;_$&#+,@%";
                permitidos += "0123456789";
                String _texto = removerTexto(texto, texto.Substring(cnt - 1 >= 0 ? cnt - 1 : cnt), permitidos);

                if (!_texto.Equals(texto))
                {
                    if (this._txt.GetType() == typeof(PasswordBox))
                    {
                        ((PasswordBox)this._txt).Password = _texto;
                    }
                }                
            }            
        }

        private void clsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = "";
            string _texto = "";
            int pos = ((TextBox)sender).SelectionStart;

            if (this._txt.GetType() == typeof(TextBox))
            {
                texto = ((TextBox)this._txt).Text;
            }
            
            string permitidos = "";
            int cnt = texto.Length;
            switch (this.caracteresPermitidos)
            {
                case CARACTERES_PERMITIDOS.CORREO:
                    permitidos = "0123456789abcdefghijklmnopqrstuvwxyyz@._-";
                    if (pos > 0)
                    {
                        texto = texto.ToLower();
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    }
                    else
                        _texto = texto;
                    break;
                case CARACTERES_PERMITIDOS.USUARIOS:
                    permitidos = "0123456789.-_";
                    permitidos += "abcdefghijklmnopqrstuvwxyyz";
                    permitidos += "abcdefghijklmnopqrstuvwxyyz".ToUpper();
                    if (pos > 0)
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    else
                        _texto = texto;
                    break;
                case CARACTERES_PERMITIDOS.SERVIDOR:
                    permitidos = "0123456789abcdefghijklmnopqrstuvwxyz.";
                    if (pos > 0)
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    else
                        _texto = texto;
                    break;
                case CARACTERES_PERMITIDOS.DECIMALES:
                    permitidos = "0123456789.";
                    if (cnt > 0)
                    {
                        if (pos > 0)
                            _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                        else
                        {
                            _texto = texto;
                        }
                    }
                    String[] sep = _texto.Split('.');
                    if (sep.Length > 1)
                    {
                        if (sep.Length == 2)
                        {
                            if (sep[1].Length > cantidaDecimales) sep[1] = sep[1].Substring(0, cantidaDecimales);
                            _texto = sep[0] + "." + sep[1];
                        }
                        else
                        {
                            _texto = sep[0] + "." + sep[1] + sep[2];
                        }
                    }else
                    {
                        _texto = sep[0];
                    }                    

                    break;
                case CARACTERES_PERMITIDOS.NUMERICOS:
                    permitidos = "0123456789";
                    if (permiteEspacios) permitidos += " ";
                    if (otrosCaracteresPermitidos != "") permitidos += otrosCaracteresPermitidos;
                    if (pos > 0)
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    else
                        _texto = texto;
                    break;
                case CARACTERES_PERMITIDOS.LETRAS:
                    permitidos += "abcdefghijklmnopqrstuvwxyyzñáéíóú";
                    permitidos += "abcdefghijklmnopqrstuvwxyyzñáéíóú".ToUpper();

                    if (otrosCaracteresPermitidos != "") permitidos += otrosCaracteresPermitidos;
                    if (permiteEspacios) permitidos += " ";
                    if (permitePorcentaje) permitidos += "%";
                    if (permitePunto) permitidos += ".";
                    if (pos > 0)
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    else
                        _texto = texto;
                    break;
                case CARACTERES_PERMITIDOS.NUMERICOS_LETRAS:
                    permitidos = "0123456789";
                    permitidos += "abcdefghijklmnopqrstuvwxyyzñáéíóú";
                    permitidos += "abcdefghijklmnopqrstuvwxyyzñáéíóú".ToUpper();
                    if (otrosCaracteresPermitidos != "") permitidos += otrosCaracteresPermitidos;
                    if (permiteEspacios) permitidos += " ";
                    if (permitePorcentaje) permitidos += "%";
                    if (permitePunto) permitidos += ".";
                    if (pos > 0)
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    else
                        _texto = texto;
                    break;
                case CARACTERES_PERMITIDOS.URL:
                    permitidos = "0123456789";
                    permitidos += "abcdefghijklmnopqrstuvwxyyz";
                    permitidos += "abcdefghijklmnopqrstuvwxyyz".ToUpper();
                    permitidos += ":/_-.";

                    if (pos > 0)
                        _texto = removerTexto(texto, texto.Substring(pos - 1, 1), permitidos);
                    else
                        _texto = texto;
                    break;
            }

            if (!_texto.Equals(texto))
            {
                if (this._txt.GetType() == typeof(TextBox))
                {
                    ((TextBox)this._txt).Text = _texto;
                    ((TextBox)this._txt).SelectionStart = pos;
                }
            }
        }

        private void clsTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            int val = (int)e.Key;

            Console.WriteLine(e.Key);
            if (this.permiteEspacios)
            {
                if (e.Key == Key.Space) e.Handled = false;
            }
            
            if (e.Key == Key.Back) e.Handled = false;
            if (e.Key == Key.Tab) e.Handled = false;
            if (e.Key == Key.Home) e.Handled = false;
            if (e.Key == Key.End) e.Handled = false;
            if (e.Key == Key.LeftShift) e.Handled = false;
            if (e.Key == Key.RightShift) e.Handled = false;

            switch (this.caracteresPermitidos)
            {
                case CARACTERES_PERMITIDOS.NUMERICOS:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers == ModifierKeys.None) e.Handled = false;
                    if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) e.Handled = false;

                    break;
                case CARACTERES_PERMITIDOS.DECIMALES:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers == ModifierKeys.None) e.Handled = false;
                    if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) e.Handled = false;
                    if (this._txt.GetType() == typeof(TextBox))
                    {
                        if (((TextBox)this._txt).Text.IndexOf('.') < 0)
                        {
                            if (e.Key == Key.OemPeriod) e.Handled = false;
                        }
                    }

                    break;
                case CARACTERES_PERMITIDOS.LETRAS:
                    if ((e.Key >= Key.A && e.Key <= Key.Z) || Keyboard.Modifiers == ModifierKeys.Shift) e.Handled = false;
                    break;
                case CARACTERES_PERMITIDOS.NUMERICOS_LETRAS:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers == ModifierKeys.None) e.Handled = false;
                    if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) e.Handled = false;
                    if ((e.Key >= Key.A && e.Key <= Key.Z) || Keyboard.Modifiers == ModifierKeys.Shift) e.Handled = false;

                    break;
                case CARACTERES_PERMITIDOS.USUARIOS:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers == ModifierKeys.None) e.Handled = false;
                    if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) e.Handled = false;
                    if ((e.Key >= Key.A && e.Key <= Key.Z) || Keyboard.Modifiers == ModifierKeys.Shift) e.Handled = false;

                    break;
                case CARACTERES_PERMITIDOS.CORREO:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers == ModifierKeys.None) e.Handled = false;
                    if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) e.Handled = false;
                    if ((e.Key >= Key.A && e.Key <= Key.Z) || Keyboard.Modifiers == ModifierKeys.Shift) e.Handled = false;
                                        
                    break;
                case CARACTERES_PERMITIDOS.CLAVES:
                    if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers == ModifierKeys.None) e.Handled = false;
                    if ((e.Key >= Key.A && e.Key <= Key.Z) || Keyboard.Modifiers == ModifierKeys.Shift) e.Handled = false;
                    if (val == 143 || val == 144) e.Handled = false;
                    if (e.Key == Key.OemSemicolon || e.Key == Key.OemPeriod || e.Key == Key.OemMinus) e.Handled = false;
                    if (e.Key == Key.OemPlus || e.Key == Key.OemPipe || e.Key == Key.OemComma) e.Handled = false;
                    if (e.Key == Key.Divide) e.Handled = false;
                    break;
                default:
                    e.Handled = false;

                    break;
            }
        }

        private void clsTextBox_KeyDow(object sender, KeyEventArgs e)
        {
            int val = (int)e.Key;
            e.Handled = true;

            switch (this.caracteresPermitidos)
            {
                case CARACTERES_PERMITIDOS.NUMERICOS:
                    if (val >= 34 && val <= 43) e.Handled = false;
                    
                    break;
                case CARACTERES_PERMITIDOS.DECIMALES:
                    if (val >= 34 && val <= 43) e.Handled = false;
                    if (this._txt.GetType() == typeof(TextBox))
                    {
                        if (((TextBox)this._txt).Text.IndexOf('.') < 0)
                        {
                            if (e.Key == Key.OemPeriod) e.Handled = false;
                        }
                    }
                    
                    break;
                case CARACTERES_PERMITIDOS.LETRAS:
                    break;
                case CARACTERES_PERMITIDOS.NUMERICOS_LETRAS:
                    break;
                case CARACTERES_PERMITIDOS.USUARIOS:
                    if (val >= 34 && val <= 43) e.Handled = false;
                    if (val >= 44 && val <= 69) e.Handled = false;

                    break;
                case CARACTERES_PERMITIDOS.CORREO:
                    if (val >= 34 && val <= 43) e.Handled = false;
                    if (val >= 44 && val <= 69) e.Handled = false;
                    

                    break;
                case CARACTERES_PERMITIDOS.CLAVES:
                    if (val >= 34 && val <= 43) e.Handled = false;
                    if (val >= 44 && val <= 69) e.Handled = false;
                    if (val == 143 || val == 144) e.Handled = false;

                    break;
                default:
                    e.Handled = true;

                    break;
            }
        }
    }
}

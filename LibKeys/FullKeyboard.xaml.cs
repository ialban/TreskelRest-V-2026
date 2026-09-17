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
using System.Windows.Controls.Primitives;

namespace LibKeys
{
    /// <summary>
    /// Interaction logic for FullKeyboard.xaml
    /// </summary>
    public partial class FullKeyboard : Window
    {
        public event RetornaTexto retornaTexto;
        public delegate void RetornaTexto(Object txt, String texto);

        String[] letras = {"a","b","c","d","e","f",
                           "g","h","i","j","k","l",
                           "m","n","ñ","o","p","q",
                           "r","s","t","u","v","w",
                           "x","y","z"};

        String[] otros = {"1","2","3","/","(",")",
                          "4","5","6","$","!","&",
                          "7","8","9","-","+","_",
                          "#","0","*","=","?","¿",
                          "<",">","%"};

        String[] comodines = {"5" ,"10","15" ,"Sr."  ,"Sra.","Srta.",
                              "20","25","30" ,"Av."  ,"Mz." ,"Villa",
                              "35","40","45" ,"Cdla.","gmail","Blq.",
                              "50","55","60",".com" ,".net",".ec",
                              "65","70","75"};

        Button _spc, _punto, _coma, _arroba, _scp, _back, _cls, _enter;
        Object _txtBox;

        ToggleButton _otros, _shift, _comodin;

        private Boolean isNumeric = false;
        private Boolean isDecimal = false;
        private Boolean isMail = false;
        private Boolean isDireccion = false;
        private Boolean isUsuario = false;

        private int maxLength=5000;

        public int MaxLength {
            set {
                maxLength = value;
                texto.MaxLength = value;
            }
        }

        public Object setTextBox
        {
            set
            {
                _txtBox = value;
                if(_txtBox.GetType() == typeof(TextBox))
                {
                    texto.Text = ((TextBox)_txtBox).Text;
                }

                if (_txtBox.GetType() == typeof(PasswordBox))
                {
                    texto.Text = ((PasswordBox)_txtBox).Password;
                    texto.FontFamily= new FontFamily(new Uri("pack://application:,,,/"),"./Fonts/#password");
                }
            }
        }
        
        private void ActivaIsNumeric(Button boton, String _txt)
        {
            try
            {
                int.Parse(_txt);
                boton.IsEnabled = true;
            }
            catch (Exception e)
            {
                boton.IsEnabled = false;
            }
        }

        private void ActivaDireccion(Button boton, String _txt)
        {
            String descactivar = "_+*=&%$¿?!¡<>";
            boton.IsEnabled = true;

            if (descactivar.Contains(_txt))
            {
                boton.IsEnabled = false;
            }

            if (boton.IsEnabled)
            {
                String[] _grps = {".net",".ec"  ,"Sr.",
                                  ".com","gmail","Sra.",
                                  "Srta."};

                foreach (String _grp in _grps)
                {
                    if (_txt.Equals(_grp))
                    {
                        boton.IsEnabled = false;
                    }
                }
            }
        }

        private void ActivaIsMail(Button boton, String _txt)
        {
            String validos = "abcdefghijklmnopqrstuvwwxyz1234567890@_.-";
            boton.IsEnabled = false;
            if (validos.Contains(_txt))
            {
                boton.IsEnabled = true;                
            }

            if (!boton.IsEnabled)
            {
                String[] _grps = { "5" ,"10","15",".com",".net", ".ec",
                                   "20","25","30","35"  ,"40"  ,"45",
                                   "50","55","60","65"  ,"70"  ,"75",
                                   "gmail"};

                foreach (String _grp in _grps)
                {
                    if (_txt.Equals(_grp))
                    {
                        boton.IsEnabled = true;
                    }
                }
            }            
        }

        public void setNumeric()
        {
            this.ActivaTodos();

            this.isNumeric = true;
            this.isDecimal = false;
            this.isMail = false;
            this.isDireccion = false;

            this._otros.IsChecked = true;
            this._otros.IsEnabled = false;

            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            foreach (Button boton in botones)
            {
                this.ActivaIsNumeric(boton, boton.Content.ToString());                
            }

            this.DisabledBotonesNumericos(false);
        }

        public void setUsuario()
        {
            this.ActivaTodos();

            this.isUsuario = true;
            this.isNumeric = false;
            this.isDecimal = false;
            this.isMail = false;
            this.isDireccion = false;

            this._comodin.IsEnabled = false;

        }

        public void ActivaTodos()
        {
            this._spc.IsEnabled = true;
            this._coma.IsEnabled = true;
            this._shift.IsEnabled = true;
            this._arroba.IsEnabled = true;
            this._punto.IsEnabled = true;
            this._otros.IsEnabled = true;
        }

        public void setDireccion()
        {
            this.ActivaTodos();

            this.isDireccion = true;
            this.isMail = false;
            this.isNumeric = false;
            this.isDecimal = false;

            this._arroba.IsEnabled = false;
        }

        public void setMail()
        {
            this.ActivaTodos();

            this.isMail = true;
            this.isNumeric = false;
            this.isDecimal = false;
            this.isDireccion = false;

            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            foreach (Button boton in botones)
            {
                this.ActivaIsMail(boton, boton.Content.ToString());
            }
            this.DisabledBotonesMail();
        }

        public void setDecimal()
        {
            this.ActivaTodos();

            this.isDecimal = true;
            this.isNumeric = false;
            this.isMail = false;
            this.isDireccion = false;

            this._otros.IsChecked = true;
            this._otros.IsEnabled = false;

            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            foreach (Button boton in botones)
            {
                this.ActivaIsNumeric(boton, boton.Content.ToString());
            }

            this.DisabledBotonesNumericos(true);
        }

        public void DisabledBotonesNumericos(Boolean usaPunto)
        {
            this._arroba.IsEnabled = false;
            this._coma.IsEnabled = false;

            if (!usaPunto)
            {                
                this._punto.IsEnabled = false;
            }                
            this._shift.IsEnabled = false;
            this._spc.IsEnabled = false;
        }

        public void DisabledBotonesMail()
        {
            this._spc.IsEnabled = false;
            this._coma.IsEnabled = false;
            this._shift.IsEnabled = false;
        }

        public void DisabledBotonesDireccion()
        {
            this._shift.IsEnabled = false;
        }

        public FullKeyboard()
        {
            InitializeComponent();

            this.Height = 525;
            this.Width = 550;

            this._spc = this.space;
            this._scp = this.cancelar;
            this._punto = this.punto;
            this._coma = this.coma;
            this._arroba = this.arroba;
            this._back = this.back;
            this._cls = this.clear;
            this._enter = this.enter;

            this._otros = this.cares;
            this._shift = this.Shift;
            this._comodin = this.comodin;

            this._spc.Click += new RoutedEventHandler(Btn_Click);
            this._punto.Click += new RoutedEventHandler(Btn_Click);
            this._coma.Click += new RoutedEventHandler(Btn_Click);
            this._arroba.Click += new RoutedEventHandler(Btn_Click);
            this._scp.Click += new RoutedEventHandler(Scp_Click);
            this._back.Click += new RoutedEventHandler(Back_Click);
            this._cls.Click += new RoutedEventHandler(Clear_Click);
            this._enter.Click += new RoutedEventHandler(Enter_Click);

            this._shift.Checked += new RoutedEventHandler(Shift_Checked);
            this._shift.Unchecked += new RoutedEventHandler(Shift_Unchecked);

            this._otros.Checked += new RoutedEventHandler(Cares_Checked);
            this._otros.Unchecked += new RoutedEventHandler(Cares_Unchecked);

            this._comodin.Checked += new RoutedEventHandler(Comodin_Checked);
            this._comodin.Unchecked += new RoutedEventHandler(Comodin_Unchecked);

            for (int i = 0; i < letras.Length; i++)
            {                
                this.creaBotonW(letras[i], Grid, new RoutedEventHandler(Btn_Click));
            }
            texto.Focus();
        }

        private void creaBoton(String texto, StackPanel panel)
        {
            Button btn = new Button()
            {
                Content = texto,
                Height = 60,
                Width = 66,
                FontSize = 20,
                Tag = ""
            };

            panel.Children.Add(btn);
        }

        private void creaBotonW(String texto, UniformGrid panel, RoutedEventHandler hndl)
        {
            Button btn = new Button()
            {
                Content = texto,
                Height = 66,
                Width = 70,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Background = "aeiou".Contains(texto) ? Brushes.DarkSalmon : Brushes.LightGray,
                Tag = ""
            };

            if( hndl != null)
                btn.Click += hndl;

            panel.Children.Add(btn);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (isDecimal){
                if (texto.Text.ToString().LongCount(punto=>punto.ToString() == ".") > 0 &&
                    ((Button)sender).Content.ToString().Equals("."))
                {
                    texto.Focus();
                    return;
                }
            }

            if (texto.Text.Length == maxLength){
                texto.Focus();
                return;
            }

            int _idx = texto.SelectionStart;
            String txt1 = "";
            String txt2 = "";

            if(_idx >= 0 && _idx < texto.Text.Length){
                txt1 = texto.Text.Substring(0, _idx);
                txt2 = texto.Text.Substring(_idx);

                if (!((Button)sender).Tag.ToString().Equals("")){
                    texto.Text = txt1 + ((Button)sender).Tag.ToString() +txt2;
                    texto.SelectionStart = _idx + ((Button)sender).Tag.ToString().Length;
                }
                else{
                    texto.Text = txt1 + ((Button)sender).Content.ToString() + txt2;
                    texto.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
                }                
            }else{
                if (!((Button)sender).Tag.ToString().Equals("")){
                    texto.Text += ((Button)sender).Tag.ToString();
                    texto.SelectionStart = _idx + ((Button)sender).Tag.ToString().Length;
                }
                else{
                    texto.Text += ((Button)sender).Content.ToString();
                    texto.SelectionStart = _idx + ((Button)sender).Content.ToString().Length;
                }
            }
            texto.Focus();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            texto.Text = "";
        }

        private void Scp_Click(object sender, RoutedEventArgs e)
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

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            retornaTexto(this._txtBox,texto.Text.ToString());
            this.Hide();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            String _txt = texto.Text.ToString();
            int _idx = texto.SelectionStart;

            if (texto.SelectionLength > 0){
                texto.Text = texto.Text.Remove(_idx, texto.SelectionLength);
                texto.Focus();
                return;
            }

            if (_txt.Length > 0){                
                if (_idx > 0 && _idx < texto.Text.Length){
                    String txt1 = texto.Text.Substring(0, _idx);
                    String txt2 = texto.Text.Substring(_idx);

                    txt1 = txt1.Remove(txt1.Length - 1, 1);
                    texto.Text = txt1 + txt2;                    
                }
                else{
                    if (_idx == 0) return;
                    texto.Text = _txt.Substring(0, _txt.Length - 1);
                }
                texto.SelectionStart = (_idx - 1) > 0 ? _idx - 1 : 0;
            }
            texto.Focus();
        }

        private void Shift_Change(Boolean marcado)
        {
            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            foreach (Button boton in botones)
            {
                if (marcado)
                {
                    if(this._comodin.IsChecked != true)
                    {
                        boton.Content = boton.Content.ToString().ToUpper();
                    }                    
                }
                else
                {
                    if (this._comodin.IsChecked != true)
                    {
                        boton.Content = boton.Content.ToString().ToLower();
                    }
                }

                if (this.isDireccion)
                {
                    this.ActivaDireccion(boton, boton.Content.ToString());
                }
            }

            this._coma.Content = marcado ? ";" : ",";
            this._punto.Content = marcado ? ":" : ".";
            texto.Focus();
        }

        private void Shift_Checked(object sender, RoutedEventArgs e)
        {
            Shift_Change(true);            
        }

        private void Shift_Unchecked(object sender, RoutedEventArgs e)
        {
            Shift_Change(false);            
        }

        private void Cares_Checked(object sender, RoutedEventArgs e)
        {
            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            for(int i=0; i<botones.Count; i++)
            {
                Button boton = botones[i];

                boton.Content = otros[i];

                if (this.isDireccion)
                {
                    this.ActivaDireccion(boton, boton.Content.ToString());
                }
            }

            this._otros.Content = "abcd";
            this._comodin.IsChecked = false;
            texto.Focus();
        }

        private void Cares_Unchecked(object sender, RoutedEventArgs e)
        {
            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            for (int i = 0; i < botones.Count; i++)
            {
                Button boton = botones[i];

                if (this._shift.IsChecked == true)
                {
                    boton.Content = letras[i].ToUpper();
                }
                else
                {
                    boton.Content = letras[i];
                }

                if (this.isDireccion)
                {
                    this.ActivaDireccion(boton, boton.Content.ToString());
                }
            }

            this._otros.Content = "123#";
            texto.Focus();
        }

        private void Comodin_Checked(object sender, RoutedEventArgs e)
        {
            if (!(this.isNumeric || this.isDecimal))
            {
                this._otros.IsChecked = false;
            }

            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            for (int i = 0; i < botones.Count; i++)
            {
                Button boton = botones[i];

                if (this._shift.IsChecked == true)
                {
                    boton.Content = comodines[i].ToUpper();
                }
                else
                {
                    boton.Content = comodines[i];
                }                    

                if(this.isNumeric == true || this.isDecimal==true)
                {                    
                    this.ActivaIsNumeric(boton, boton.Content.ToString());
                }

                if (this.isMail)
                {
                    this.ActivaIsMail(boton, boton.Content.ToString());
                }

                if (this.isDireccion)
                {
                    this.ActivaDireccion(boton, boton.Content.ToString());
                }
            }
            texto.Focus();
        }

        private void Comodin_Unchecked(object sender, RoutedEventArgs e)
        {
            List<Button> botones = this.Grid.Children.OfType<Button>().ToList();

            for (int i = 0; i < botones.Count; i++)
            {
                Button boton = botones[i];

                if (this._shift.IsChecked == true)
                {
                    if(this._otros.IsChecked == true)
                    {
                        boton.Content = otros[i].ToUpper();
                    }
                    else
                    {
                        boton.Content = letras[i].ToUpper();
                    }                    
                }
                else
                {
                    if (this._otros.IsChecked == true)
                    {
                        boton.Content = otros[i];
                    }
                    else
                    {
                        boton.Content = letras[i];
                    }                    
                }

                if (this.isNumeric == true || this.isDecimal)
                {
                    this.ActivaIsNumeric(boton, boton.Content.ToString());
                }

                if (this.isMail)
                {
                    this.ActivaIsMail(boton, boton.Content.ToString());
                }

                if (this.isDireccion)
                {
                    this.ActivaDireccion(boton, boton.Content.ToString());
                }
            }
            texto.Focus();
        }
    }   
}

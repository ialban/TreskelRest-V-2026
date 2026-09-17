using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibControls.TextBoxMask
{
    public enum TextBoxEnumMasks
    {
        Factura,
        AutorizacionSRI,
        PeriodoFiscal
    }

    public class TextBoxMask : TextBox
    {
        private TextBoxEnumMasks _mask = TextBoxEnumMasks.Factura;

        public TextBoxEnumMasks Masked 
        {
            get {
                return _mask;
            }

            set {
                _mask = value;

                switch (_mask)
                {
                    case TextBoxEnumMasks.Factura:
                        Tag = "###-###-#########";
                        break;
                    case TextBoxEnumMasks.AutorizacionSRI:
                        Tag = "#".PadLeft(49,'#');
                        break;
                    case TextBoxEnumMasks.PeriodoFiscal:
                        Tag = "##/####";
                        break;
                }
            }
        }
        
        public TextBoxMask()
        {
            TextChanged += new TextChangedEventHandler(MaskedTextBox_TextChanged);
            //TextLostFocus += new LostKeyboardFocus();
            //LostKeyboardFocus += new TextBoxMask_LostKeyboardFocus(MaskedTextBox_LostFocus);
            LostKeyboardFocus += TextBoxMask_LostKeyboardFocus;
        }

        private void TextBoxMask_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            switch (_mask)
            {
                case TextBoxEnumMasks.Factura:
                    var texto = Text.ToString().Split('-');
                    switch (texto.Length)
                    {
                        case 3:
                            Text = texto[0].PadLeft(3, '0') + "-" + texto[1].PadLeft(3, '0') + "-" + texto[2].PadLeft(9, '0');
                            break;
                        case 2:
                            Text = texto[0].PadLeft(3, '0') + "-" + texto[1].PadLeft(3, '0') + "-" + "0".PadLeft(9, '0');
                            break;
                        case 1:
                            Text = texto[0].PadLeft(3, '0') + "-" + "0".PadLeft(3, '0') + "-" + "0".PadLeft(9, '0');
                            break;
                        case 0:
                            Text = "0".PadLeft(3, '0') + "-" + "0".PadLeft(3, '0') + "-" + "0".PadLeft(9, '0');
                            break;
                    }
                    
                    break;
                case TextBoxEnumMasks.AutorizacionSRI:
                    Text = "0".PadLeft(49, '0');
                    break;
                case TextBoxEnumMasks.PeriodoFiscal:
                    var txt = Text.ToString().Split('/');
                    switch (txt.Length)
                    {
                        case 2:
                            Text = txt[0].PadLeft(2, '0') + "/" + txt[1].PadLeft(4, '0');
                            break;
                        case 1:
                            Text = txt[0].PadLeft(2, '0') + "/" + "0".PadLeft(4, '0');
                            break;
                        case 0:
                            Text = "0".PadLeft(2, '0') + "/" + "0".PadLeft(4, '0');
                            break;
                    }
                    break;
            }
        }

        public void MaskedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CaretIndex = this.Text.Length;
            var tbEntry = sender as TextBoxMask;

            if (tbEntry != null && tbEntry.Text.Length > 0)
            {
                tbEntry.Text = setFormat(tbEntry.Text, tbEntry.Masked);
            }
        }

        public string setFormat(string valor, TextBoxEnumMasks formato)
        {
            string nuevo = "";
            int x;
            StringBuilder sb = new StringBuilder();

            if (valor != null && valor != "")
            {
                for (int i = 0; i < valor.Length; i++)
                {
                    if (int.TryParse(valor.Substring(i, 1), out x))
                    {
                        sb.Append(x.ToString());
                    }
                }
                switch (formato)
                {
                    case TextBoxEnumMasks.Factura:
                        nuevo = FacturaFormato(sb.ToString());
                        break;
                    case TextBoxEnumMasks.AutorizacionSRI:
                        nuevo = AutorizacionSRIFormato(sb.ToString());
                        break;
                    case TextBoxEnumMasks.PeriodoFiscal:
                        nuevo = PeridoFiscalFormato(sb.ToString());
                        break;
                }
            }

            return nuevo;
        }

        private string AutorizacionSRIFormato(string valor)
        {
            StringBuilder nuevo = new StringBuilder();

            int length = valor.Length;
            if (valor.Length > length - 1) nuevo.Append(valor.Substring(length - 1, 1));

            return nuevo.ToString();
        }

        private string FacturaFormato(string valor)
        {
            StringBuilder nuevo = new StringBuilder();

            if (valor.Length > 0) nuevo.Append(valor.Substring(0, 1));
            if (valor.Length > 1) nuevo.Append(valor.Substring(1, 1));
            if (valor.Length > 2) nuevo.Append(valor.Substring(2, 1));

            if (valor.Length > 3) nuevo.Append("-");

            if (valor.Length > 3) nuevo.Append(valor.Substring(3, 1));
            if (valor.Length > 4) nuevo.Append(valor.Substring(4, 1));
            if (valor.Length > 5) nuevo.Append(valor.Substring(5, 1));

            if (valor.Length > 6) nuevo.Append("-");

            if (valor.Length > 6) nuevo.Append(valor.Substring(6, 1));
            if (valor.Length > 7) nuevo.Append(valor.Substring(7, 1));
            if (valor.Length > 8) nuevo.Append(valor.Substring(8, 1));
            if (valor.Length > 9) nuevo.Append(valor.Substring(9, 1));
            if (valor.Length > 10) nuevo.Append(valor.Substring(10, 1));
            if (valor.Length > 11) nuevo.Append(valor.Substring(11, 1));
            if (valor.Length > 12) nuevo.Append(valor.Substring(12, 1));
            if (valor.Length > 13) nuevo.Append(valor.Substring(13, 1));
            if (valor.Length > 14) nuevo.Append(valor.Substring(14, 1));
            //if (valor.Length > 15) nuevo.Append(valor.Substring(15, 1));

            return nuevo.ToString();
        }

        private string PeridoFiscalFormato(string valor)
        {
            StringBuilder nuevo = new StringBuilder();

            if (valor.Length > 0) nuevo.Append(valor.Substring(0, 1));
            if (valor.Length > 1) nuevo.Append(valor.Substring(1, 1));

            if (valor.Length > 2) nuevo.Append("/");

            if (valor.Length > 2) nuevo.Append(valor.Substring(2, 1));
            if (valor.Length > 3) nuevo.Append(valor.Substring(3, 1));
            if (valor.Length > 4) nuevo.Append(valor.Substring(4, 1));
            if (valor.Length > 5) nuevo.Append(valor.Substring(5, 1));

            return nuevo.ToString();
        }
    }
}

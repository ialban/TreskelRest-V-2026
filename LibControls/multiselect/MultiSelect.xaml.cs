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

namespace LibControls.multiselect
{
    /// <summary>
    /// Lógica de interacción para MultiSelect.xaml
    /// </summary>
    /// 
    public class DDLContent
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public bool isChecked { get; set; }
    }

    public partial class MultiSelect : UserControl
    {
        private double _ancho;
        private double _alto;
        private List<DDLContent> _content = new List<DDLContent>();

        public new virtual double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                cboContent.Width = value - 2;
                base.Width = value;
            }
        }

        public new virtual double Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                cboContent.Height = value - 2;
                base.Height = value;
            }
        }

        public double Ancho
        {
            get
            {
                _ancho = base.Width;
                return _ancho;
            }
            set
            {
                _ancho = value;
                cboContent.Height = _ancho;
                Width = value;
            }
        }

        public double Alto
        {
            get
            {
                _alto = base.Height;
                return _alto;
            }
            set
            {
                _alto = value;
                cboContent.Height = _alto;
                Width = value;
            }
        }

        public List<DDLContent> Contenido
        {
            get{
                return _content;
            }
            set
            {
                _content = value;
                cboContent.ItemsSource = _content;
            }
        }

        public MultiSelect()
        {
            InitializeComponent();
        }
    }
}

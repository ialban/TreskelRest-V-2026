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

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Interaction logic for ctlBase.xaml
    /// </summary>
    public partial class ctlBase : UserControl
    {
        public Window contenedor { get; set; }
        public String accion { get; set; } = "N";
        public ctlClientes parent { get; set; } = null;

        public ctlBase()
        {            
            InitializeComponent();
                     
        }

        public void setCod(string cod)
        {
            
        }
    }
}

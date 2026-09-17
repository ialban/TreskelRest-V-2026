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
using System.Windows.Threading;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlEsperar.xaml
    /// </summary>
    public partial class ctlEsperar : UserControl
    {
        public ctlEsperar()
        {
            InitializeComponent();
            
        }
        
        public void setTexto(string texto)
        {
            Mensaje.Content = texto;
        }
    }
}

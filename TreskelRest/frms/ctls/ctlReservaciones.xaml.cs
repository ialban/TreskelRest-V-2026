using Entidades;
using Entidades.modelos;
using LibControls.buttons;
using LibKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreskelRest.cls;

namespace TreskelRest.frms.ctls
{
    /// <summary>
    /// Lógica de interacción para ctlReservaciones.xaml
    /// </summary>
    public partial class ctlReservaciones : UserControl, IDisposable
    {
        private bool disposedValue;
        public string codigo = "";
        private NumKeyboard nKeyboard = new NumKeyboard();

        public ctlReservaciones()
        {
            InitializeComponent();

            LlenaMesas(General.CANTIDAD_MESAS, principal, principalContainer, "P", "PR");
            LlenaMesas(General.CANTIDAD_MESAS_ALTERNA, alterno, alternoContainer, "A","AL");
            LlenaMesas(General.CANTIDAD_MESAS_EXTERIOR, exterior, exteriorContainer, "E","EX");
            LlenaMesas(General.CANTIDAD_MESAS_TERRAZA, terraza, terrazaContainer, "T","TR");
            LlenaMesas(General.CANTIDAD_MESAS_2DO_PISO, sdopiso, sdopisoContainer, "S","SP");

            Limpiar();

            nKeyboard.retornaTexto += new LibKeys.NumKeyboard.RetornaTexto(TextoNumerico_Retorna);
        }

        private void Limpiar()
        {
            DateTime actual = DateTime.Now;
            DateTime siguiente = actual.AddHours(1);

            string hora = string.Format("{0:00}", actual.Hour.ToString());
            string hora2 = string.Format("{0:00}", siguiente.Hour.ToString());
            string minutos = string.Format("{0:00}", DateTime.Now.Minute.ToString());

            if(btnNinguno != null)
            {
                btnNinguno.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

            txtCedula.Text = "";
            txtCliente.Text = "";
            txtCntPersonas.setValue("0");
            fecha.SelectedDate = DateTime.Now;
            Desde.setValue(hora + ":" + minutos);
            Hasta.setValue(hora2 + ":" + minutos);
            txtNota.Text = "";
            ReservasContainer.Children.Clear();
        }

        private void HabilitaBotones(bool valor)
        {
            btnNinguno.IsEnabled = valor;
            btnGrabar.IsEnabled = valor;
            btnCancelar.IsEnabled = valor;
        }

        public void HabilitaBotonNinguno()
        {
            if (ReservasContainer.Children.Count > 0)
            {
                HabilitaBotones(true);
            }
            else
            {
                HabilitaBotones(false);
            }
        }

        private void LlenaMesas(int cantidad,TabItem item, WrapPanel panel, string tipo, string codigoAlterno)
        {
            int factor = 0;
            if (codigoAlterno == "PR") factor = 100;
            if (codigoAlterno == "AL") factor = 200;
            if (codigoAlterno == "EX") factor = 300;
            if (codigoAlterno == "TR") factor = 400;
            if (codigoAlterno == "SP") factor = 500;

            if (cantidad > 0)
            {
                for (int i = 1; i <= cantidad; i++)
                {
                    ctlToggleButton btn = new ctlToggleButton();

                    btn.Width = 120;
                    btn.Height = 70;
                    btn.Codigo = tipo;
                    btn.Contenido = i.ToString().PadLeft(3, '0');
                    //btn.titul
                    btn.CodigoAlterno = codigoAlterno;
                    btn.Imagen = "pack://application:,,,/images/table.png";

                    btn._ctlButtonClick += new ctlToggleButton.ctlButton_Click(ctlButton_Click);
                    btn._ctlButtonUnCheck += new ctlToggleButton.ctlButton_UnCheck(ctlButton_UnCheck);

                    panel.Children.Add(btn);
                }
            }
            else
            {
                tabControl.Items.Remove(item);
            }
        }

        private void DefineMesasReservadas(string contenido)
        {
            Label lbl = new Label();
            lbl.Content = contenido;
            lbl.Background = Brushes.Blue;
            lbl.Foreground = Brushes.White;
            lbl.Height = 30;
            lbl.Width = 80;

            ReservasContainer.Children.Add(lbl);
        }
        public void ctlButton_Click(object sender, RoutedEventArgs e, string respuesta)
        {
            DefineMesasReservadas(getContentFromToggle((ToggleButton)sender));

            HabilitaBotonNinguno();
        }

        public void ctlButton_UnCheck(object sender, RoutedEventArgs e, string respuesta)
        {
            foreach(var item in ReservasContainer.Children)
            {
                if(item.GetType() == typeof(Label))
                {
                    if (((Label)item).Content.ToString() == getContentFromToggle((ToggleButton)sender))
                    {
                        ReservasContainer.Children.Remove((Label)item);
                        HabilitaBotonNinguno();
                        break;
                    }
                }
            }
        }

        private string getContentFromToggle(ToggleButton sender)
        {
            var content = ((StackPanel)sender.Content).Children;
            var content2 = ((StackPanel)content[1]).Children;
            string texto = ((Label)content2[0]).Content + ":" +((TextBlock)content2[1]).Text;
            return texto;
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            txtCedula.Text = "";
            txtCliente.Text = "";
        }

        private void btnBscCli_Click(object sender, RoutedEventArgs e)
        {
            Clientes cliente = new Clientes(General.connectionString);

            cliente.nidNid = txtCedula.Text.ToString();
            if (cliente.getById())
            {
                txtCliente.Text = cliente.nidNbr;
            }
            else
            {
                winGeneral win = new winGeneral();
                win.Title = "Creación de cliente";
                win.Width = 710;
                win.Height = 430;

                ctlCreaCliente crea = new ctlCreaCliente();
                crea.contenedor = win;
                crea.callFromFact = true;
                crea.accion = "N";
                crea.creacionCliente += new ctlCreaCliente.CreacionCliente(Cliente_Creado);
                crea.setNid(cliente.nidNid);
                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
        }

        private void Cliente_Creado(string cliente, string cedula, string direccion, string telefono, string tipo, string email, bool clieCdto, string emprruc, string emprnbr, bool emprPaga)
        {
            txtCliente.Text = cliente;
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Clear();

            this.Dispose();
        }

        private string generaMesas()
        {
            List<string> mesas = new List<string>();

            foreach (var item in ReservasContainer.Children)
            {
                if (item.GetType() == typeof(Label))
                {
                    mesas.Add(((Label)item).Content.ToString());
                }
            }

            if(mesas.Count > 0)
            {
                return string.Join(";", mesas.ToArray());
            }
            else
            {
                return "";
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            string msgError = "";

            if (txtCliente.Text == "")
            {
                msgError += "- Debe seleccionar un cliente" + System.Environment.NewLine;
            }

            if (ReservasContainer.Children.Count == 0)
            {
                msgError += "- Debe seleccionar por lo menos una mesa para reservar." + System.Environment.NewLine;
            }

            if (txtCntPersonas.getValue().ToString() == "0")
            {
                msgError += "- La cantidad de personas debe ser mayor a cero." + System.Environment.NewLine;
            }

            if (msgError != "")
            {
                ObjComportamiento.showMsgError(msgError);
                return;
            }

            if(ObjComportamiento.showMsgYesNo("¿Desea grabar la reservación?") == MessageBoxResult.Yes)
            {
                Reservacion reserv = new Reservacion(General.connectionString);

                reserv.reseNid = codigo;
                reserv.nidNid = txtCedula.Text;
                reserv.reseCnt = int.Parse(txtCntPersonas.getValue());

                string fch = fecha.SelectedDate.Value.Date.ToString("yyyy-MM-dd");

                reserv.reseDes = fch + " " + Desde.getValue() + ":00";
                reserv.reseHas = fch + " " + Hasta.getValue() + ":00";

                var motivo = cboMotivo.SelectedIndex;

                switch (motivo)
                {
                    case 0:
                        reserv.reseMot = "N";
                        break;
                    case 1:
                        reserv.reseMot = "C";
                        break;
                    case 2:
                        reserv.reseMot = "R";
                        break;
                    case 3:
                        reserv.reseMot = "L";
                        break;
                    case 4:
                        reserv.reseMot = "F";
                        break;
                    case 6:
                        reserv.reseMot = "P";
                        break;
                    case 7:
                        reserv.reseMot = "M";
                        break;
                }

                reserv.reseNot = txtNota.Text;
                reserv.reseMes = generaMesas();

                var result = reserv.grabar(codigo == "" ? "N": "M");

                ObjComportamiento.showMuestraMensaje(result);

                if (result.resultado)
                {
                    Limpiar();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectUnselect(bool accion)
        {
            if((TabItem)tabControl.SelectedItem != null)
            {
                var tabItem = (WrapPanel)((TabItem)tabControl.SelectedItem).Content;
                foreach (var mesas in tabItem.Children)
                {
                    if (mesas.GetType() == typeof(ctlToggleButton))
                    {
                        ((ctlToggleButton)mesas).Checked = accion;
                    }
                }
            }
        }

        private void btnTodos_Click(object sender, RoutedEventArgs e)
        {
            SelectUnselect(true);
        }

        private void btnNinguno_Click(object sender, RoutedEventArgs e)
        {
            SelectUnselect(false);
        }

        private void DockPanel_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void btnBscRsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                winGeneral win = new winGeneral();
                win.Title = "Buscar Reservaciones";
                win.Width = 860;
                win.Height = 450;

                ctlBscReservacion crea = new ctlBscReservacion();
                crea.contenedor = win;
                crea.reservaSeleccionada += new ctlBscReservacion.ReservaSeleccionada(Reserva_Seleccionada);
                //crea.cancelaBusqueda += new ctlBscPedido.CancelaBusqueda(Busqueda_Cancelada);
                win.Contenido.Children.Add(crea);

                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ResizeMode = ResizeMode.NoResize;
                win.ShowDialog();
            }
            catch (Exception er)
            {

            }
        }

        private void Reserva_Seleccionada(string codigo)
        {
            try
            {
                btnGrabar.IsEnabled = false;

                Reservacion reserv = new Reservacion(General.connectionString);

                mReservacion reservacion = reserv.getReservacion(codigo);

                if(reservacion.reseNid != null && reservacion.reseNid != "")
                {
                    txtCedula.Text = reservacion.nidNid;
                    txtCliente.Text = reservacion.nidNbr;
                    txtCntPersonas.setValue(reservacion.reseCnt.ToString());
                    txtNota.Text = reservacion.reseNot;
                    fecha.SelectedDate = DateTime.Parse( reservacion.reseFec);
                    Desde.setValue(reservacion.reseDes);
                    Desde.setValue(reservacion.reseHas);
                    switch (reservacion.reseMot)
                    {
                        case "N":
                            cboMotivo.SelectedIndex = 0;
                            break;
                        case "C":
                            cboMotivo.SelectedIndex = 1;
                            break;
                        case "R":
                            cboMotivo.SelectedIndex = 2;
                            break;
                        case "L":
                            cboMotivo.SelectedIndex = 3;
                            break;
                        case "F":
                            cboMotivo.SelectedIndex = 4;
                            break;
                        case "P":
                            cboMotivo.SelectedIndex = 5;
                            break;
                        case "M":
                            cboMotivo.SelectedIndex = 6;
                            break;
                    }

                    string[] mesas = reservacion.reseMes.Split(';');
                    var lstMesas = mesas.ToList().Select( c => new mMesa { 
                        tipo = c.Substring(0,1),
                        codigo = c.Substring(2)
                    }).OrderBy(c => c.codigo);

                    ReservasContainer.Children.Clear();

                    foreach (TabItem itm in tabControl.Items)
                    {
                        foreach (var sMesas in ((WrapPanel)itm.Content).Children)
                        {
                            ((ctlToggleButton)sMesas).Checked = false;
                        }
                    }

                    foreach (mMesa mesa in lstMesas)
                    {
                        foreach (TabItem itm in tabControl.Items)
                        {
                            if(itm.Tag.ToString() == mesa.tipo)
                            {
                                foreach (var sMesas in ((WrapPanel)itm.Content).Children)
                                {
                                    string tip = ((ctlToggleButton)sMesas).Codigo.ToString();
                                    string cod = ((ctlToggleButton)sMesas).Contenido;

                                    if (cod == mesa.codigo)
                                    {
                                        ((ctlToggleButton)sMesas).Checked = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        private void btnTeclado_Click(object sender, RoutedEventArgs e)
        {
            nKeyboard.MaxLength = 3;

            nKeyboard.setTextBox = new TextBox();

            nKeyboard.ShowDialog();
        }

        private void TextoNumerico_Retorna(Object txt, String texto)
        {
            if (texto.Equals(""))
            {
                ObjComportamiento.showMsgError("Debe ingresar una cantidad de personas para la reservación");
                return;
            }

            txtCntPersonas.setValue(texto);
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~ctlReservaciones()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

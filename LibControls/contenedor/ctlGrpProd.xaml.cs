using LibControls.buttons;
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

namespace LibControls.contenedor
{
    /// <summary>
    /// Interaction logic for ctlGrpProd.xaml
    /// </summary>
    public partial class ctlGrpProd : UserControl
    {
        public double altura = 0;
        public double ancho = 0;
        private string _organizacion = "H";
        public int cntCols = 1;
        public double anchoContenedor = 0;
        //private int _colsGrid = 0;

        public enum TamanioGroups
        {
            Normal = 1,
            Small = 2
        }

        private TamanioGroups _tamanioGroups = ctlGrpProd.TamanioGroups.Normal;

        public TamanioGroups SetTamanioGroups
        {
            set
            {
                _tamanioGroups = value;
            }
        }

        public int SetColumnasGrid
        {
            set
            {
                if (_organizacion == "V")
                {
                    grupoContainer.Columns = 2;
                }
                else
                {
                    grupoContainer.Columns = value;
                }

                if (_tamanioGroups == TamanioGroups.Normal)
                {
                    grupoContainer.Columns = value;
                    productosContainer.Columns = value -2;
                }
                else
                {
                    switch (cntCols)
                    {
                        case 1:
                            grupoContainer.Columns = 10;
                            productosContainer.Columns = 8;
                            break;
                        case 2:
                            grupoContainer.Columns = 9;
                            productosContainer.Columns = 7;
                            break;
                        case 3:
                            grupoContainer.Columns = 8;
                            productosContainer.Columns = 6;
                            break;
                        default:
                            grupoContainer.Columns = 6;
                            productosContainer.Columns = 5;
                            break;
                    }
                }
            }
        }

        public string Organizacion
        {
            set
            {
                _organizacion = value;

                if (_organizacion == "V")
                {
                    //ctlOrgGrpProd.Orientation = Orientation.Horizontal;
                    contProd.Margin = new Thickness(200, 3, 0, 0);
                    if (_tamanioGroups == TamanioGroups.Normal)
                    {
                        contGrupo.Width = 140 * cntCols;
                    }
                    else
                    {
                        switch (cntCols)
                        {
                            case 1:
                                contGrupo.Width = 125 * cntCols;
                                break;
                            case 2:
                                contGrupo.Width = 210 * cntCols;
                                break;
                            case 3:
                                contGrupo.Width = 305 * cntCols;
                                break;
                            default:
                                contGrupo.Width = 395 * cntCols;
                                break;
                        }
                    }

                    contProd.Width = Math.Abs(ancho - contGrupo.Width - 20);
                    GrupoScroll.Height = Math.Abs(altura - lblGrupo.Height);
                    ProdScroll.Height = Math.Abs(altura - lblProds.Height);
                    grupoContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
                    grupoContainer.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    //ctlOrgGrpProd.Orientation = Orientation.Vertical;
                    
                    if (anchoContenedor <= 35)
                    {
                        GrupoScroll.Width = Double.NaN;
                        ProdScroll.Width = Double.NaN;
                    }
                    else
                    {
                        GrupoScroll.Width = anchoContenedor - 35;
                        ProdScroll.Width = anchoContenedor - 35;
                    }

                    if (anchoContenedor <= 3)
                    {
                        contGrupo.Width = Double.NaN;
                        contProd.Width = Double.NaN;
                    }
                    else
                    {
                        contGrupo.Width = anchoContenedor - 3;
                        contProd.Width = anchoContenedor - 3;
                    }

                    GrupoScroll.Height = 180;
                    //grupoContainer.Height = 190;
                }
            }
        }

        public ctlGrpProd()
        {
            InitializeComponent();

            ctlOrgGrpProd.Margin = new Thickness(0, 0, 0, 0);
            grupoContainer.VerticalAlignment = VerticalAlignment.Stretch;
        }

        public void setHeight()
        {
            if (_organizacion == "V")
            {
                ProdScroll.Height = Math.Abs(altura - lblProds.Height);
            }
            else
            {
                double height = Math.Abs(altura - GrupoScroll.Height - lblGrupo.Height - lblProds.Height);
                ProdScroll.Height = height;
            }
        }

        public void setWidth()
        {
            double width = Math.Abs(ancho - 20);
            ctlOrgGrpProd.Width = width;
        }

        public void addToGroup(UIElement obj)
        {
            grupoContainer.Children.Add(obj);
        }

        public void clearGroup()
        {
            Organizacion = "H";
            grupoContainer.Children.Clear();
        }

        public void addToProduct(UIElement obj)
        {
            productosContainer.Children.Add(obj);
        }

        public void clearProduct()
        {
            productosContainer.Children.Clear();
        }

        public void setTituloGrupo(string titulo)
        {
            lblGrupo.Content = titulo;
        }

        public void setTituloProducto(string titulo)
        {
            lblProds.Content = titulo;
        }

        public UIElementCollection getGrupoContainer()
        {
            return grupoContainer.Children;
        }

        public void resetFilter()
        {
            foreach (var itmsGrp in grupoContainer.Children)
            {
                var btn = (ctlButtonNI)itmsGrp;
                btn.setTituloBold(false);
                btn.setTituloColor(false);
            }
        }

        public void clearProductFilter()
        {
            foreach (var items in productosContainer.Children)
            {
                var btn = (ctlButtonNI)items;
                btn.setTituloBold(false);
                btn.setTituloColor(false);
            }
        }
    }
}

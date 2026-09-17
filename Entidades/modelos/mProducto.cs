using System;

namespace Entidades.modelos
{
    public class mProducto
    {
        public string codigo { get; set; }
        public string item { get; set; }
        public string cantidad { get; set; } = "0";
        public string unidad { get; set; } = "";
        public string inventario { get; set; } = "SI";
        public string precio { get; set; } = "0,00";
        public string iva { get; set; } = "0.00";
        public string total { get; set; } = "0.00";
    }
}

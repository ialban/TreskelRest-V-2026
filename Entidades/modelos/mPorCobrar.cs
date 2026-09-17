using System;

namespace Entidades.modelos
{
    public class mPorCobrar
    {
        public string secuencial { get; set; }
        public DateTime fecha { get; set; }
        public int cantidad { get; set; }
        public double total { get; set; }
        public string descripcion { get; set; }
        public string persona { get; set; }
    }
}

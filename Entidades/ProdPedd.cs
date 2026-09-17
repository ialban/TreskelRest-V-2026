using Entidades.modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Entidades
{
    public class ProdPeddSearch
    {
        String _s;

        public ProdPeddSearch(String s)
        {
            _s = s;
        }

        public bool StartsWith(ProdPedd e)
        {
            return e.codigo.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class ProdPedd
    {
        public String codigo { get; set; }
        public String producto { get; set; }
        public int cantidad { get; set; }
        public Double precio { get; set; }
        public Boolean gravaIva { get; set; }
        public Double iva { get; set; }
        public Double PrctjIva { get; set; } // Porcentaje Iva
        public Double total { get; set; }
        public Boolean usaIva { get; set; }
        public bool precioPorPeso { get; set; }
        public double peso { get; set; }
        [JsonIgnore]
        public List<mIngredientes> noIngredientes { get; set; } = new List<mIngredientes>();
        public String observacion { get; set; } = "";
        public Boolean DelFromCoc { get; set; } = false;

        public override bool Equals(object obj)
        {
            bool igual = true;
            var _ProdPedd = obj as ProdPedd;

            if (_ProdPedd != null)
            {
                if (codigo != _ProdPedd.codigo) igual = false;
                if (producto != _ProdPedd.producto) igual = false;
                if (precio != _ProdPedd.precio) igual = false;
                if (iva != _ProdPedd.iva) igual = false;
                if (total != _ProdPedd.total) igual = false;
                if (usaIva != _ProdPedd.usaIva) igual = false;
                if (observacion != _ProdPedd.observacion) igual = false;
            }
            return igual;
        }
    }
}

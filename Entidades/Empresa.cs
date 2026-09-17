using AccesoDatos;
using System;
using System.Data;

namespace Entidades
{
    public class Empresa : EntidadBase
    {
        public Empresa(string strConnection) : base(strConnection)
        {
        }

        public String getNombre()
        {
            String nombre = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select desencryptar(emprNbr) as empresa from tbAdsiEmpr";

                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        nombre = dr["empresa"].ToString();
                    }
                }

                conn.cerrar();

            }

            return nombre;
        }        
    }
}

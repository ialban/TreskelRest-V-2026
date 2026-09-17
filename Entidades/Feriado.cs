using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Feriado : EntidadBase
    {
        public int frdoNid { get; set; }
        public string frdoDsc { get; set; } = "C";
        public DateTime frdoFin { get; set; }
        public DateTime frdoFfn { get; set; }
        public bool frdoAif { get; set; } = false;
        public bool frdoAtv { get; set; } = true;

        public Feriado(string strConnection) : base(strConnection)
        {

        }

        public DataTable buscar(string descripcion, string estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT  frdoNid AS codigo, frdoDsc AS descripcion, " +
                                "       TO_CHAR(frdoFin,'YYYY-MM-DD') AS desde, " +
                                "       TO_CHAR(frdoFFn,'YYYY-MM-DD') AS hasta, " +
                                "       CASE " +
                                "           WHEN frdoAif = TRUE THEN 'ACTIVO'" +
                                "           ELSE 'INACTIVO' " +
                                "       END AS ivaFeriado, " +
                                "       CASE " +
                                "           WHEN frdoAtv = TRUE THEN 'ACTIVO'" +
                                "           ELSE 'INACTIVO' " +
                                "       END AS estado " +
                                "FROM   tbAdsiFrdos " +
                                "WHERE  (@descrip = '' OR frdodsc LIKE @descrip) " +
                                "        AND(@estado = '' OR frdoAtv = CASE  " +
                                "                                         WHEN @estado = 'A' THEN true " +
                                "                                         ELSE false " +
                                "                                     END " +
                                "            )";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("descrip", '%' + descripcion + '%'));
                parametros.Add(new SqlParameter("estado", estado));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }

            return dt;
        }

        public Boolean getById()
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection); ;
            if (conn.conectar())
            {
                String query = "SELECT * " +
                               "FROM tbAdsiFrdos " +
                               "WHERE frdoNid = @frdoNid";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("frdoNid", this.frdoNid));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                if (dt != null)
                {
                    dt.TableName = "Listado";
                    conn.cerrar();

                    if (dt.Rows.Count > 0)
                    {
                        this.frdoNid = int.Parse(dt.Rows[0]["frdonid"].ToString());
                        this.frdoDsc = dt.Rows[0]["frdodsc"].ToString();
                        this.frdoFin = DateTime.Parse(dt.Rows[0]["frdofin"].ToString());
                        this.frdoFfn = DateTime.Parse(dt.Rows[0]["frdoffn"].ToString());
                        this.frdoAif = (bool)dt.Rows[0]["frdoaif"];
                        this.frdoAtv = (bool)dt.Rows[0]["frdoatv"];
                        
                        res = true;
                    }
                }

            }

            return res;
        }

        public Boolean grabar(string accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("codsec", this.frdoNid));
                parametros.Add(new SqlParameter("nombre", this.frdoDsc.ToUpper()));
                parametros.Add(new SqlParameter("desde", this.frdoFin));
                parametros.Add(new SqlParameter("hasta", this.frdoFfn));
                parametros.Add(new SqlParameter("ivaferiado", this.frdoAif));
                parametros.Add(new SqlParameter("activo", this.frdoAtv));

                string query = "SELECT * FROM fnFeriados(@accion, @codsec::INT, @nombre::varchar, @desde::DATE, @hasta::DATE, @ivaferiado:: BOOLEAN, @activo::BOOLEAN);";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                res.resultado = false;
                res.mensaje = "No se pudo procesar el Feriado";

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
                
                conn.cerrar();
            }

            return res.resultado;
        }
    }
}

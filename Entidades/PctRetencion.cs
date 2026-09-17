using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class PctRetencion : EntidadBase
    {
        public string codigo { get; set; }
        public string codsri { get; set; }
        public string descripcion { get; set; }
        public double valor { get; set; }
        public string tipo { get; set; }
        public bool activo { get; set; }

        public PctRetencion(string strConnection) : base(strConnection)
        {

        }

        public mRetencion GetById(string codigo)
        {
            mRetencion result = new mRetencion();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                String query = "SELECT  retescl AS codigo, retesri AS codsri, " + 
                               "        retedsc AS descripcion, " +
                               "        reteval AS valor, reteapl AS tipo, " +
                               "        CASE WHEN reteact = TRUE THEN 'A' ELSE 'I' END AS estado " +
                               "FROM    tbPctRete WHERE reteScl = @codigo";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.codigo = dr["codigo"].ToString();
                        result.codsri = dr["codsri"].ToString();
                        result.descripcion = dr["descripcion"].ToString();
                        result.valor = Utils.Helper.StringToDouble(dr["valor"].ToString());
                        result.tipo = dr["tipo"].ToString();
                        result.estado = dr["estado"].ToString();
                    }
                }

                conn.cerrar();
            }

            return result;
        }

        public List<mRetencion> GetByType(string tipo)
        {
            List<mRetencion> retenciones = new List<mRetencion>();

            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                String query = "SELECT 	retescl AS codigo, retesri AS codsri, retedsc AS descripcion, reteval AS valor, " +
                               "        reteapl AS tipo, reteact AS estado " + 
                               "FROM    tbPctRete " +
                               "WHERE   reteapl = @tipo AND reteact = true " +
                               "ORDER BY retescl";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("tipo", tipo));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        mRetencion retencion = new mRetencion();

                        retencion.codigo = dr["codigo"].ToString();
                        retencion.codsri = dr["codsri"].ToString();
                        retencion.descripcion = dr["descripcion"].ToString();
                        retencion.valor = Utils.Helper.StringToDouble(dr["valor"].ToString());
                        retencion.tipo = dr["tipo"].ToString();
                        retencion.estado = dr["estado"].ToString();

                        retenciones.Add(retencion);
                    }
                }

                conn.cerrar();
            }

            return retenciones;
        }

        public List<mRetencion> getRetenciones(string tipo = "T", string activo="")
        {
            List<mRetencion> retenciones = new List<mRetencion>();

            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                String query = "SELECT 	retescl AS codigo, retesri AS codsri, retedsc AS descripcion, reteval AS valor, " + 
                               "        CASE " +
                               "            WHEN reteapl = 'I' THEN 'IVA'" +
                               "            WHEN reteapl = 'R' THEN 'RENTA'" +
                               "            ELSE ''" +
                               "        END AS tipo, " + 
                               "        CASE " +
                               "            WHEN reteact = TRUE THEN 'Activo' " +
                               "            ELSE 'Inactivo' " +
                               "        END AS Estado " +
                               "FROM    tbPctRete " +
                               "WHERE   (@tipo = 'T' OR reteapl = @tipo) " + 
                               "        AND (@activo = '' or reteact = CASE " + 
                               "                                            WHEN @activo = 'T' THEN TRUE " + 
                               "                                            WHEN @activo = 'F' THEN FALSE " + 
                               "                                            ELSE NULL " +
                               "                                        END) " +
                               "ORDER BY retescl";

                string valor = "";

                if (activo == "A") valor = "T";
                if (activo == "I") valor = "F";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("tipo", tipo));
                parametros.Add(new SqlParameter("activo", valor));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        mRetencion retencion = new mRetencion();

                        retencion.codigo = dr["codigo"].ToString();
                        retencion.codsri = dr["codsri"].ToString();
                        retencion.descripcion = dr["descripcion"].ToString();
                        retencion.valor = Utils.Helper.StringToDouble(dr["valor"].ToString());
                        retencion.tipo = dr["tipo"].ToString();
                        retencion.estado = dr["estado"].ToString();

                        retenciones.Add(retencion);
                    }
                }

                conn.cerrar();
            }

            return retenciones;
        }
    
        public Respuesta Grabar(string accion)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = new DataTable();

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("secuencia", this.codigo));
                parametros.Add(new SqlParameter("codsri", this.codsri));
                parametros.Add(new SqlParameter("nombre", this.descripcion));
                parametros.Add(new SqlParameter("valor", this.valor));
                parametros.Add(new SqlParameter("tipo", this.tipo));
                parametros.Add(new SqlParameter("activo", this.activo));

                String query = "SELECT * FROM fnGrabaRetenciones(@accion, @secuencia::character varying, " + 
                               "@codsri:: character varying, @nombre::character varying, @valor::numeric, " + 
                               "@tipo::character varying, @activo::boolean)";

                dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }

                conn.cerrar();
            }

            return res;
        }
    }
}

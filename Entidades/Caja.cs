using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Caja : EntidadBase
    {
        public string codigo { get; set; }
        public string nombre { get; set; }

        public Caja(string strConnection) : base(strConnection)
        {
        }

        public List<mCaja> getAll()
        {
            List<mCaja> result = new List<mCaja>();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select * from tbCajaCaja";

                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        mCaja caja = new mCaja();

                        caja.codigo = dr["cajacod"].ToString();
                        caja.nombre = dr["cajanbr"].ToString();

                        result.Add(caja);
                    }
                }

                conn.cerrar();
            }
            return result;
        }

        public mCaja getById(string codigo)
        {
            mCaja result = new mCaja();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select cajacod, cajanbr from tbCajaCaja WHERE cajacod = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    result.codigo = dt.Rows[0]["cajacod"].ToString();
                    result.nombre = dt.Rows[0]["cajanbr"].ToString();
                }

                conn.cerrar();
            }
            return result;
        }

        public Respuesta Grabar(string accion)
        {
            Respuesta respuesta = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select * from fnCaja(@accion, @codigo, @nombre)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("codigo", codigo));
                parametros.Add(new SqlParameter("nombre", nombre));
                
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    respuesta.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    respuesta.mensaje = dt.Rows[0]["descripcion"].ToString();
                }

                conn.cerrar();
            }

            return respuesta;
        }

        public Respuesta InicioCierre(String tipo, string secuencial, string fecha, string hora, string valor, string sistema, string diferencia)
        {
            Respuesta respuesta = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select * from fnControlCaja(@tipo, @caja, @secuencial, @fecha, @hora, @valor, @sistema, @diferencia)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("tipo", tipo));
                parametros.Add(new SqlParameter("caja", this.codigo));
                parametros.Add(new SqlParameter("secuencial", secuencial));
                parametros.Add(new SqlParameter("fecha", fecha));
                parametros.Add(new SqlParameter("hora", hora));
                parametros.Add(new SqlParameter("valor", valor));
                parametros.Add(new SqlParameter("sistema", sistema));
                parametros.Add(new SqlParameter("diferencia", diferencia));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    respuesta.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    respuesta.mensaje = dt.Rows[0]["descripcion"].ToString();
                }

                conn.cerrar();
            }

           return respuesta;
        }
    }
}

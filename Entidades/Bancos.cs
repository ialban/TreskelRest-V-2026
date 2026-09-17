using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Bancos : EntidadBase
    {
        public int bcosnid { get; set; }
        public string bcosnbr { get; set; }
        public string bcosacr { get; set; }
        public string bcosnct { get; set; }
        public string bcostip { get; set; }
        public string bcosuso { get; set; }
        public bool bcospri { get; set; }
        public bool bcosatv { get; set; }

        public Bancos(string strConnection) : base(strConnection)
        {

        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta
            {
                resultado = false,
                mensaje = ""
            };

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("nid", bcosnid));
                parametros.Add(new SqlParameter("nombre", bcosnbr));
                parametros.Add(new SqlParameter("acronimo", bcosacr));
                parametros.Add(new SqlParameter("cuenta", bcosnct));
                parametros.Add(new SqlParameter("tipo", bcostip));
                parametros.Add(new SqlParameter("uso", bcosuso));
                parametros.Add(new SqlParameter("primario", bcospri));
                parametros.Add(new SqlParameter("activo", bcosatv));

                string query = "select * from fnGrabaBcos(@accion,@nid, @nombre, @acronimo, @cuenta, @tipo, @uso, @primario, @activo)";

                var dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    res.resultado = dr["codigo"].ToString() == "1" ? true : false;
                    res.mensaje = dr["descripcion"].ToString();
                }

                conn.cerrar();
            }

            return res;
        }

        public Respuesta grabarTransaccion(int bconid, string descripcion, string tipo, double valor, string cliente, string proveedor, string otro)
        {
            Respuesta res = new Respuesta
            {
                resultado = false,
                mensaje = ""
            };

            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("bancoid", bconid));
                parametros.Add(new SqlParameter("descrip", descripcion));
                parametros.Add(new SqlParameter("tipo", tipo));
                parametros.Add(new SqlParameter("valor", valor));
                parametros.Add(new SqlParameter("cliente", cliente));
                parametros.Add(new SqlParameter("proveedor", proveedor));
                parametros.Add(new SqlParameter("otro", otro));

                string query = "select * from fnGrabaMovBcos(@bancoid::INT, @descrip::CHARACTER VARYING, @tipo::VARCHAR(2), @valor::NUMERIC(10,2), @cliente::VARCHAR(15), @proveedor::VARCHAR(15), @otro::VARCHAR(200))";

                var dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    res.resultado = dr["codigo"].ToString() == "1" ? true : false;
                    res.mensaje = dr["descripcion"].ToString();
                }

                conn.cerrar();
            }

            return res;
        }

        public List<lBancos> buscar(string nombre, string estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<lBancos> resultado = new List<lBancos>();
            if (conn.conectar())
            {
                string query = "select * from fnLstBcos(@nombre,@estado);";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("nombre", nombre));
                parametros.Add(new SqlParameter("estado", estado));

                var dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    foreach(DataRow row in dt.Rows)
                    {
                        resultado.Add(new lBancos
                        {
                            codigo = int.Parse(row["codigo"].ToString()),
                            nombre = row["nombre"].ToString(),
                            cuenta = row["cuenta"].ToString(),
                            acronimo = row["acronimo"].ToString(),
                            tipo = row["tipo"].ToString(),
                            uso = row["uso"].ToString(),
                            estado = row["estado"].ToString()
                        });
                    }
                }
            }

            return resultado;
        }

        public mBancos getById(int codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            mBancos resultado = null;
            if (conn.conectar())
            {
                string query = "SELECT * FROM tbAdsiBcos WHERE bcosnid = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                var dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        resultado = new mBancos
                        {
                            bcosnid = int.Parse(row["bcosnid"].ToString()),
                            bcosnbr = row["bcosnbr"].ToString(),
                            bcosnct = row["bcosnct"].ToString(),
                            bcosacr = row["bcosacr"].ToString(),
                            bcostip = row["bcostip"].ToString(),
                            bcosuso = row["bcosuso"].ToString(),
                            bcospri = (bool)row["bcospri"],
                            bcosatv = (bool)row["bcosatv"]
                        };
                    }
                }
            }

            return resultado;
        }
    }
}

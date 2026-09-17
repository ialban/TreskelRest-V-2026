using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Reservacion : EntidadBase
    {
        public string reseNid { get; set; }
        public string nidNid { get; set; }
        public int reseCnt { get; set; }
        public string reseDes { get; set; }
        public string reseHas { get; set; }
        public string reseMot { get; set; }
        public string reseNot { get; set; }
        public string reseMes { get; set; }

        public Reservacion(string strConnection) : base(strConnection)
        {

        }

        public mReservacion getReservacion(string codigo)
        {
            mReservacion reservacion = new mReservacion();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT 	r.resenid " +
                               "        , r.nidnid " +
                               "        ,c.nidnbr " +
                               "        ,r.resecnt " +
                               "        ,to_char(r.resedes, 'yyyy-mm-dd') AS resefec " +
                               "        , to_char(r.resedes, 'HH24:mi') AS resedes " +
                               "        , to_char(r.resehas, 'HH24:mi') AS resehas " +
                               "        , r.resemot " +
                               "        ,r.resenot " +
                               "        ,r.resemes " +
                               "FROM    tbReserv r " +
                               "        INNER JOIN " +
                               "        tbAdsiNid c ON r.nidnid = c.nidnid " +
                               "WHERE   resenid = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("codigo", codigo));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    reservacion = new mReservacion
                    {
                        reseNid = codigo,
                        nidNid = dt.Rows[0]["nidnid"].ToString(),
                        nidNbr = dt.Rows[0]["nidnbr"].ToString(),
                        reseCnt = int.Parse(dt.Rows[0]["resecnt"].ToString()),
                        reseFec = dt.Rows[0]["resefec"].ToString(),
                        reseDes = dt.Rows[0]["resedes"].ToString(),
                        reseHas = dt.Rows[0]["resehas"].ToString(),
                        reseMot = dt.Rows[0]["resemot"].ToString(),
                        reseNot = dt.Rows[0]["resenot"].ToString(),
                        reseMes = dt.Rows[0]["resemes"].ToString()
                    };
                }
            }

            return reservacion;
        }

        public DataTable getReservasBsc(string filtro)
        {
            DataTable dt = new DataTable();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT 	r.resenid " +
                               "        , r.nidnid " +
                               "        ,c.nidnbr " +
                               "        ,r.resecnt " +
                               "        ,to_char(r.resedes, 'yyyy-mm-dd') AS resefec " +
                               "        , to_char(r.resedes, 'HH24:mi') AS resedes " +
                               "        , to_char(r.resehas, 'HH24:mi') AS resehas " +
                               "        , r.resemot " +
                               "        ,r.resenot " +
                               "        ,r.resemes " +
                               "FROM    tbReserv r " +
                               "        INNER JOIN " +
                               "        tbAdsiNid c ON r.nidnid = c.nidnid " +
                               "WHERE   1 = 1 " +
                               "        AND(@filtro = '' OR " +
                               "        (r.nidnid = @filtro OR c.nidnbr LIKE UPPER('%' || @filtro || '%'))) " +
                               "        AND r.resedes >= NOW()";

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("filtro", filtro));

                dt = conn.ejecutaQuery(query, parametros);

                conn.cerrar();
            }

            return dt;
        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("codres", reseNid));
                parametros.Add(new SqlParameter("cliente", nidNid));
                parametros.Add(new SqlParameter("cantidad", reseCnt));
                parametros.Add(new SqlParameter("desde", reseDes));
                parametros.Add(new SqlParameter("hasta", reseHas));
                parametros.Add(new SqlParameter("motivo", reseMot));
                parametros.Add(new SqlParameter("nota", reseNot));
                parametros.Add(new SqlParameter("mesas", reseMes));

                String query = "SELECT * FROM fnGrabReserva(@accion, @codres, @cliente, @cantidad, @desde, @hasta, @motivo, @nota, @mesas)";
                DataTable dt = conn.ejecutaQuery(query, parametros);

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

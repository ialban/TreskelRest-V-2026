using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Compra : EntidadBase
    {
        public string compscl { get; set; }
        public string codcaja { get; set; }
        public string codempr { get; set; }
        public string numfact { get; set; }
        public string fecha { get; set; }
        public Boolean compatv { get; set; } = true;
        public double subtot0 { get; set; } = 0;
        public double subtot { get; set; } = 0;
        public double descto { get; set; } = 0;
        public double iva { get; set; } = 0;
        public double total { get; set; } = 0;
        public Boolean usaCajaChica { get; set; } = false;

        public List<DetalleCompra> detalle { get; set; }
        public List<mPago> pago { get; set; }

        public Compra(string strConnection) : base(strConnection)
        {
            
        }

        public mCompra getCompra(string cliente, string factura)
        {
            mCompra res = new mCompra();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_cliente", cliente));
                parametros.Add(new SqlParameter("p_factura", factura));

                string query = "SELECT * FROM tbCompDComp WHERE compfac = @p_factura AND provnid = @p_cliente AND compatv = true";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        res.compscl = dr["compnid"].ToString();
                        res.fecha = dr["compfch"].ToString();
                        res.codempr = dr["provnid"].ToString();
                        res.subtot = Utils.Helper.StringToDouble(dr["compsbt"].ToString());
                        res.iva = Utils.Helper.StringToDouble(dr["compiva"].ToString());
                        res.total = Utils.Helper.StringToDouble(dr["comptot"].ToString());
                        res.compatv = bool.Parse(dr["compatv"].ToString());
                    }
                }

                conn.cerrar();
            }
            return res;
        }

        public Respuesta grabar(string accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("usaCajaChica", this.usaCajaChica));
                parametros.Add(new SqlParameter("data", JsonConvert.SerializeObject(
                    new mCompra
                    {
                        codcaja = this.codcaja,
                        codempr = this.codempr,
                        numfact = this.numfact,
                        fecha = this.fecha,
                        compatv = this.compatv,
                        subtot0 = this.subtot0,
                        subtot = this.subtot,
                        descto = this.descto,
                        iva = this.iva,
                        total = this.total
                    }
                )));

                parametros.Add(new SqlParameter("detalle", JsonConvert.SerializeObject(this.detalle)));
                parametros.Add(new SqlParameter("pago", JsonConvert.SerializeObject(this.pago)));

                string query = "SELECT * FROM fnCompras(@accion,  @usaCajaChica, @data::varchar, @detalle::varchar, @pago::varchar);";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                res.resultado = false;
                res.mensaje = "No se pudo procesar la compra";

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

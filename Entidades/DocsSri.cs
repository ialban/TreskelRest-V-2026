using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class DocsSri : EntidadBase
    {
        public DateTime fecha { get; set; }
        public string tipo { get; set; }
        public string tipoImpuesto { get; set; }
        public string numeroComprobante { get; set; }
        public string numeroAutorizacion { get; set; }
        public string identificacion { get; set; }
        public string cliente { get; set; }
        public string proveedor { get; set; }
        public double subNoIva { get; set; }
        public double subConIva { get; set; }
        public double iva { get; set; }
        public double total { get; set; }
        public double baseImponible { get; set; }
        public double porcentajeRetencion { get; set; }
        public double impuestoRetenido { get; set; }
        public string numeroComprobanteAplica { get; set; }


        public DocsSri(string strConnection) : base(strConnection)
        {

        }

        public List<mDocsSri> reporteComprobantes(int anio, int mes, int dia)
        {
            List<mDocsSri> docsSri = new List<mDocsSri>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "select * from fnRptDocsSRI(@anio, @mes, @dia)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("anio", anio));
                parametros.Add(new SqlParameter("mes", mes));
                parametros.Add(new SqlParameter("dia", dia));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        docsSri.Add(new mDocsSri()
                        {
                            tipo = dr["tipo"].ToString(),
                            fecha = DateTime.Parse(dr["emision"].ToString()),
                            numeroComprobante = dr["comprobante"].ToString(),
                            numeroAutorizacion = dr["autorizacion"].ToString(),
                            identificacion = dr["identificacion"].ToString(),
                            cliente = dr["nombre"].ToString(),
                            proveedor = dr["nombre"].ToString(),
                            subNoIva = Utils.Helper.StringToDouble(dr["subnoiva"].ToString()),
                            subConIva = Utils.Helper.StringToDouble(dr["subtotiva"].ToString()),
                            total = Utils.Helper.StringToDouble(dr["total"].ToString()),
                            numeroComprobanteAplica = dr["comprobanteapl"].ToString(),
                            tipoImpuesto = dr["tipoimpuesto"].ToString(),
                            porcentajeRetencion = Utils.Helper.StringToDouble( dr["pctjretenido"].ToString()),
                            impuestoRetenido= Utils.Helper.StringToDouble( dr["imptoretenido"].ToString()),
                            baseImponible = Utils.Helper.StringToDouble( dr["baseimponible"].ToString()),
                            iva = Utils.Helper.StringToDouble(dr["iva"].ToString())
                        });
                    }
                }

                conn.cerrar();
            }

            return docsSri;
        }
    }
}

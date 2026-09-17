using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class NotaCredito : EntidadBase
    {
        public string ntcrscl { get; set; }
        public string usrnid { get; set; }
        public string cajacod { get; set; }
        public string ventfac { get; set; }
        public string ntcraut { get; set; }
        public string ntcrusu { get; set; }
        public string ntcrmtv { get; set; }
        public DateTime ntcrfch { get; set; }

        public double ntcrsbt { get; set; }
        public double ntcrdsc { get; set; }
        public double ntcriva { get; set; }
        public double ntcrtot { get; set; }

        public string ntcrobv { get; set; }

        public List<NotaCreditoDetail> notaCreditoDetalle { get; set; }


        //public NotaCredito() { }

        public NotaCredito(string strConnection) : base(strConnection)
        {
        }

        public List<mNotaCredito> GetNotasCredito(string filtro, DateTime desde, DateTime hasta)
        {
            List<mNotaCredito> respuesta = new List<mNotaCredito>();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_filtro", filtro));
                parametros.Add(new SqlParameter("p_desde", desde));
                parametros.Add(new SqlParameter("p_hasta", hasta));

                String query = "select * from fnGetNotasCredito(@p_filtro,@p_desde::date,@p_hasta::date);";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        mNotaCredito res = new mNotaCredito();

                        res.ntcrscl = dr["ntcrscl"].ToString();
                        res.ventscl = dr["ventscl"].ToString();
                        res.ntcrmtv = dr["ntcrmtv"].ToString();
                        res.numefac = dr["numefac"].ToString();
                        res.ntcrfch = dr["ntcrfch"].ToString();
                        res.nidnid = dr["nidnid"].ToString();
                        res.nidnbr = dr["nidnbr"].ToString();
                        res.etdonce = dr["etdonce"].ToString();
                        res.etdonbr = dr["etdonbr"].ToString();

                        respuesta.Add(res);
                    }
                }

                conn.cerrar();
            }

            return respuesta;
        }

        public Respuesta AnularNotaCredito(string ntcrscl)
        {
            Respuesta respuesta = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_ntcrscl", ntcrscl));

                String query = "select * from fnAnulaNotaCredito (@p_ntcrscl)";

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

        public Respuesta grabar(string accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));

                parametros.Add(new SqlParameter("data",
                        JsonConvert.SerializeObject(
                            new cNotaCredito
                            {
                                ventscl = ntcrscl,
                                empdnid = usrnid,
                                cajacod = cajacod,
                                ventaut = ntcraut,
                                ventfac = ventfac,
                                ntcrdsc = (decimal)ntcrdsc,
                                ntcrsbt = (decimal)ntcrsbt,
                                ntcriva = (decimal)ntcriva,
                                ntcrtot = (decimal)ntcrtot,
                                ntcrobv = ntcrobv,
                                ntcrmtv = ntcrmtv
                            }
                        )
                    ));

                parametros.Add(new SqlParameter("detalle", JsonConvert.SerializeObject(notaCreditoDetalle)));

                string query = "SELECT * FROM fnNotasCredito(@accion, @data, @detalle)";

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

        public string verificaSclEnNotaCreditoElect(string ntcrscl)
        {
            string secuencial = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ntcrscl", ntcrscl));

                string query = "SELECT DISTINCT secuencial FROM tbNotCredElect WHERE ntcrscl = @ntcrscl LIMIT 1";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        secuencial = dt.Rows[0]["secuencial"].ToString();
                    }
                }

                conn.cerrar();
            }
            return secuencial;
        }

        public string getClaveAcceso(string ntcrscl)
        {
            string secuencial = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ventscl", ntcrscl));

                string query = "SELECT DISTINCT autosri FROM tbNotCredElect WHERE ntcrscl = @ntcrscl LIMIT 1";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        secuencial = dt.Rows[0]["autosri"].ToString();
                    }
                }

                conn.cerrar();
            }
            return secuencial;
        }

        public Respuesta actualizaNotaCreditoElect(string ntcrscl, string secuencial, string claveAcceso, string estado, string estadomail, string mensaje)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_ntcrscl", ntcrscl));
                parametros.Add(new SqlParameter("p_secuencial", secuencial));
                parametros.Add(new SqlParameter("p_claveacceso", claveAcceso));
                parametros.Add(new SqlParameter("p_estado", estado));
                parametros.Add(new SqlParameter("p_estadomail", estadomail));
                parametros.Add(new SqlParameter("p_mensaje", mensaje ?? ""));

                string query = "SELECT * FROM actualizaNotaCreditoElect(@p_ntcrscl, @p_secuencial, @p_claveacceso, @p_estado, @p_estadomail, @p_mensaje)";
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

        public int obtenerSecuencialNotaCredito()
        {
            int secuencial = 1;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnNextNotaCreditoSecuencial() AS nextseq";
                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    secuencial = int.Parse(dt.Rows[0]["nextseq"].ToString());
                }

                conn.cerrar();
            }

            return secuencial;
        }

        public vNotaCredito getNotaCredito(string ntcrscl)
        {
            vNotaCredito result = null;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ntcrscl", ntcrscl));

                string query = "SELECT 	a.ntcrscl, " +
                               "        v.nidnid, " +
                               "        c.nidnbr, " +
                               "        f.autosri, " +
                               "        n.autosri AS ncautosri, " +
                               "		f.numefac, " +
                               "        a.cajacod, " +
                               "        a.usuacod, " +
                               "        a.ventscl, " +
                               "        a.ntcrfch, " +
                               "        a.ntcrhra, " +
                               "        a.ntcrsbt, " +
                               "        a.ntcrdsc, " +
                               "        a.ntcriva, " +
                               "        a.ntcrtot, " +
                               "        a.ntcrobv, " +
                               "        a.ntcrmtv, " +
                               "        a.ntcratv, " +
                               "        ( " +
                               "            SELECT json_agg( " +
                               "                        d " +
                               "                ) AS detalle " +
                               "            FROM( " +
                               "                SELECT 	n.ntcrscl, " +
                               "                        COALESCE(m.itemcod, d.itemcod) AS itemcod, " +
                               "                        COALESCE(m.itemcnt, d.itemcnt) AS itemcnt, " +
                               "                        COALESCE(m.itemprc, d.itemprc) AS itemprc, " +
                               "                        CASE " +
                               "                            WHEN m.itempiv IS NULL THEN true " +
                               "                            WHEN m.itempiv > 0 THEN true " +
                               "                            ELSE false " +
                               "                        END usaiva, " +
                               "                        COALESCE(m.itempiv, d.itemiva) AS itemiva, " +
                               "                        COALESCE(m.itempto, d.itempto) AS itempto, " +
                               "                        CASE " +
                               "                            WHEN m.itemcod IS NULL THEN false " +
                               "                            ELSE true " +
                               "                        END AS itemsel " +
                               "                FROM    tbPeddMVent d " +
                               "                        INNER JOIN tbPeddDVent v ON d.ventscl = v.ventscl " +
                               "                        INNER JOIN tbPeddDNcre n ON n.ventscl = v.ventscl " +
                               "                        LEFT JOIN tbPeddMNcre m on m.ntcrscl = n.ntcrscl and m.itemcod = d.itemcod " +
                               "               WHERE n.ntcrscl = a.ntcrscl " +
                               "            ) d " +
                               "        ) AS ntcrdet " +
                               "FROM TbPeddDNcre a " +
                               "     INNER JOIN tbPeddDVent v ON v.ventscl = a.ventscl " +
                               "     INNER JOIN tbAdsiNid c ON c.nidnid = v.nidnid " +
                               "     LEFT JOIN tbFactElect f ON f.ventscl = a.ventscl " +
                               "     LEFT JOIN tbNotCredElect n ON n.ntcrscl = a.ntcrscl " +
                               "WHERE   a.ntcrscl = @ntcrscl";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result = new vNotaCredito
                        {
                            ntcrscl = dr["ntcrscl"].ToString(),
                            cajacod = dr["cajacod"].ToString(),
                            ntcrusu = dr["usuacod"].ToString(),
                            ventaut = dr["autosri"].ToString(),
                            ntcraut = dr["ncautosri"].ToString(),
                            numfact = dr["numefac"].ToString(),
                            nidnid = dr["nidnid"].ToString(),
                            nidnbr = dr["nidnbr"].ToString(),
                            ventfac = dr["ventscl"].ToString(),
                            ntcrmtv = dr["ntcrmtv"].ToString(),
                            ntcrfch = DateTime.Parse(dr["ntcrfch"].ToString()),
                            ntcrsbt = double.Parse(dr["ntcrsbt"].ToString()),
                            ntcrdsc = double.Parse(dr["ntcrdsc"].ToString()),
                            ntcriva = double.Parse(dr["ntcriva"].ToString()),
                            ntcrtot = double.Parse(dr["ntcrtot"].ToString()),
                            ntcrobv = dr["ntcrobv"].ToString()
                        };

                        string detalle = dr["ntcrdet"].ToString();

                        result.notaCreditoDetalle = JsonConvert.DeserializeObject<List<vNotaCreditoDetail>>(detalle);
                    }

                    conn.cerrar();
                }
            }

            return result;
        }

        public List<NotaCreditoDetail> getDetalleNotaCredito(string ntcrscl)
        {
            List<NotaCreditoDetail> result = new List<NotaCreditoDetail>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ntcrscl", ntcrscl));

                string query = "SELECT  * " +
                               "FROM    tbPeddMNcre r " +
                               "WHERE   r.ntcrscl = @ntcrscl";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var detalle = new NotaCreditoDetail();

                        detalle.ntcrscl = dr["ntcrscl"].ToString();
                        detalle.itemcod = dr["itemcod"].ToString();
                        detalle.itemcnt = int.Parse(dr["itemcnt"].ToString());
                        detalle.itemiva = bool.Parse(dr["itemiva"].ToString());
                        detalle.itemprc = Utils.Helper.StringToDouble(dr["itemprc"].ToString());
                        detalle.itempiv = Utils.Helper.StringToDouble(dr["itempiv"].ToString());
                        detalle.itempto = Utils.Helper.StringToDouble(dr["itempto"].ToString());

                        result.Add(detalle);
                    }

                    conn.cerrar();
                }
            }

            return result;
        }

        public Respuesta registraNotaCreditoElect(string ncrscl, string secuencial, string claveAcceso)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_ntcrscl", ncrscl));
                parametros.Add(new SqlParameter("p_secuencial", secuencial));
                parametros.Add(new SqlParameter("p_claveacceso", claveAcceso));

                string query = "SELECT * FROM creaNotaCreditoElect(@p_ventscl, @p_secuencial, @p_claveacceso)";
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

        public mFacturasSRI getNotaCreditoToSRIByNtcrScl(string notacreditoScl)
        {
            mFacturasSRI fact = new mFacturasSRI();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("vsecuencial", notacreditoScl)
                };

                string query = "SELECT * FROM fntoelectfactByScl(@vsecuencial, 'C')";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    fact.ventscl = row["ventscl"]?.ToString();
                    fact.cajacod = row["cajacod"]?.ToString();
                    fact.usuacod = row["usuacod"]?.ToString();
                    fact.ventfch = row["ventfch"]?.ToString();
                    fact.nidnid = row["nidnid"]?.ToString();
                    fact.nidtip = row["nidtip"]?.ToString();
                    fact.nidnbr = row["nidnbr"]?.ToString();
                    fact.niddir = row["niddir"]?.ToString();
                    fact.nideml = row["nideml"]?.ToString();
                    fact.nidtlf = row["nidtlf"]?.ToString();
                    fact.docuToolTip = "";

                    string tipo = row["tipo"]?.ToString();

                    switch (tipo)
                    {
                        case "F":
                            fact.docutip = "Factura";
                            fact.docuTipRet = "";
                            break;
                        case "A":
                        case "D":
                            fact.docutip = "NotaDeCredito";
                            fact.docuTipRet = (tipo == "A") ? "Anulación" : "Devolución";
                            break;
                        default:
                            fact.docutip = "Retención";
                            fact.docuTipRet = (tipo == "C") ? "Compra" : "Venta";
                            break;
                    }

                    fact.ventmsa = row["ventmsa"]?.ToString();
                    fact.ventlta = row["ventlta"]?.ToString();
                    fact.venttip = row["venttip"]?.ToString();
                    fact.peddscl = row["peddscl"]?.ToString();
                    fact.venthra = row["venthra"]?.ToString();

                    // OPTIMIZACIÓN 4: Conversión directa y segura de fechas sin usar string como intermediario si ya viene como DateTime
                    if (row["fechemi"] != DBNull.Value && !string.IsNullOrEmpty(row["fechemi"].ToString()))
                    {
                        fact.fechemi = row["fechemi"] is DateTime time ? time : DateTime.Parse(row["fechemi"].ToString());
                    }

                    // OPTIMIZACIÓN 5: Evitar conversiones de String a Double si la base de datos ya devuelve valores numéricos
                    fact.ventsbt = Math.Round(row["ventsbt"] is double dsbt ? dsbt : Utils.Helper.StringToDouble(row["ventsbt"]?.ToString()), 6);
                    fact.ventsbn = Math.Round(row["ventstn"] is double dsbn ? dsbn : Utils.Helper.StringToDouble(row["ventstn"]?.ToString()), 6);
                    fact.ventdsc = Math.Round(row["ventdsc"] is double ddsc ? ddsc : Utils.Helper.StringToDouble(row["ventdsc"]?.ToString()), 6);
                    fact.ventiva = Math.Round(row["ventiva"] is double diva ? diva : Utils.Helper.StringToDouble(row["ventiva"]?.ToString()), 6);
                    fact.venttot = Math.Round(row["venttot"] is double dtot ? dtot : Utils.Helper.StringToDouble(row["venttot"]?.ToString()), 6);
                    fact.ventrcb = Math.Round(row["ventrcb"] is double drcb ? drcb : Utils.Helper.StringToDouble(row["ventrcb"]?.ToString()), 6);
                    fact.ventvto = Math.Round(row["ventvto"] is double dvto ? dvto : Utils.Helper.StringToDouble(row["ventvto"]?.ToString()), 6);

                    // OPTIMIZACIÓN 6: Switch expression (disponible en C# moderno) para asignar directamente
                    fact.etdofac = row["etdofac"]?.ToString() switch
                    {
                        "A" => "Autorizado",
                        "D" => "Devuelta",
                        "P" => "Pendiente",
                        "V" => "Validado",
                        "N" => "No Procesado",
                        "L" => "Anulado",
                        _ => ""
                    };

                    fact.etdofacdet = row["errmsg"]?.ToString() ?? "";

                    string errCod = row["errcod"]?.ToString();
                    if (errCod == "0101")
                    {
                        fact.etdofacdet = $"{fact.etdofacdet}{System.Environment.NewLine} >> No será procesada ni enviada nuevamente al SRI";
                    }

                    fact.etdofacImg = "";
                    fact.etdoeml = row["etdoeml"]?.ToString();
                    fact.docuSri = row["docusri"]?.ToString();
                    fact.errCode = errCod;
                    fact.secuencial = row["secuencial"]?.ToString();
                    fact.autosri = row["autosri"]?.ToString();
                    fact.claveDownload = row["ventclv"]?.ToString();
                }
            }

            return fact;
        }
    }
}

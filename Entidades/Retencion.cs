using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Retencion : EntidadBase
    {
        public string reteScl { get; set; }
        public string ventScl { get; set; }
        public string reteCaj { get; set; } = "001";// Caja
        public string reteUsr { get; set; }
        public DateTime reteFch { get; set; } = DateTime.Now;
        public string reteTip { get; set; } // Retencion en compra /venta
        public string docuSri { get; set; } // Numero de documento ###-###-#########
        public string docuNAu { get; set; } // Numero de autorización en caso de provenir de una factura electrónica
        public string reteNum { get; set; }
        public string reteDTp { get; set; } // Documento relacionado Factura/Nota de venta/Liquidaciones, Tickets, Otro
        public string reteCli { get; set; }
        public string reteCTip { get; set; } = "R"; // Tipo de documento de identificación del cliente 
        public double reteTot { get; set; } = 0;
        public string reteEtd { get; set; } // Estado de la retención A = Activo, I = Inactivo
        public List<RetencionDetail> reteDet { get; set; } = new List<RetencionDetail>();

        public Retencion(string strConnection) : base(strConnection)
        {

        }

        public string getClaveAcceso(string retescl)
        {
            string secuencial = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("retescl", retescl));

                string query = "SELECT DISTINCT autosri FROM tbReteElect WHERE retescl = @retescl AND etdofac = 'N' LIMIT 1";
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

        public Respuesta grabar(string accion)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = new DataTable();

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_accion", accion));
                if (reteScl ==  null)
                {
                    parametros.Add(new SqlParameter("p_retescl", -1));
                }
                else
                {
                    parametros.Add(new SqlParameter("p_retescl", reteScl));
                }
                
                parametros.Add(new SqlParameter("p_ventscl", ventScl == null ? "": ventScl));
                parametros.Add(new SqlParameter("p_cajacod", reteCaj));
                parametros.Add(new SqlParameter("p_usuacod", reteUsr));
                parametros.Add(new SqlParameter("p_retetip", reteTip));
                parametros.Add(new SqlParameter("p_retefch", string.Format("{0:yyyy-MM-dd}",reteFch)));
                parametros.Add(new SqlParameter("p_cliente", reteCli));
                parametros.Add(new SqlParameter("p_docutip", reteCTip));
                parametros.Add(new SqlParameter("p_docusri", docuSri));
                parametros.Add(new SqlParameter("p_docunau", docuNAu));
                parametros.Add(new SqlParameter("p_retetot", reteTot));
                parametros.Add(new SqlParameter("p_reteetd", reteEtd));

                parametros.Add(new SqlParameter("p_detalle", JsonConvert.SerializeObject(reteDet)));

                string query = "SELECT * FROM fnGrabaRetencion(@p_accion::character varying, " +
                                "@p_retescl::character varying, @p_ventscl::character varying, " +
                                "@p_cajacod::character varying, @p_usuacod::character varying, " +
                                "@p_retetip::character varying, @p_retefch::Date, " +
                                "@p_cliente::character varying, @p_docutip::character varying, " +
                                "@p_docusri::character varying, @p_docunau::character varying, " +
                                "@p_retetot::numeric, @p_reteetd::character varying, " + 
                                "@p_detalle::text)";

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

        public Respuesta actualizaReteElect(string ventscl, string secuencial, string claveAcceso, string estado, string estadomail, string mensaje)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_retescl", ventscl));
                parametros.Add(new SqlParameter("p_secuencial", secuencial));
                parametros.Add(new SqlParameter("p_claveacceso", claveAcceso));
                parametros.Add(new SqlParameter("p_estado", estado));
                parametros.Add(new SqlParameter("p_estadomail", estadomail));
                parametros.Add(new SqlParameter("p_mensaje", mensaje ?? ""));

                String query = "SELECT * FROM actualizaReteElect(@p_retescl, @p_secuencial, @p_claveacceso, @p_estado, @p_estadomail, @p_mensaje)";
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

        public List<RetencionDetail> getDetalleRetencion(string retescl)
        {
            List<RetencionDetail> result = new List<RetencionDetail>();

            DbConnection conn = new DbConnection(strConnection);
            
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("retescl", retescl));

                string query = "SELECT  * " +
                               "FROM    tbPeddMRete r " +
                               "WHERE   r.retescl = @retescl";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var detalle = new RetencionDetail();

                        detalle.secuencial = dr["retescl"].ToString();
                        detalle.codigo = dr["retecod"].ToString();
                        detalle.codsri = dr["retesri"].ToString();
                        detalle.aniofisc = dr["aniofis"].ToString();
                        detalle.baseimpo = Utils.Helper.StringToDouble(dr["retebas"].ToString());
                        detalle.pctrete = Utils.Helper.StringToDouble(dr["retepct"].ToString());
                        detalle.valorrete = Utils.Helper.StringToDouble(dr["reteval"].ToString());
                        detalle.tipoimpo = dr["retetip"].ToString();
                        //detalle.tipoRetenc = dr["nidtip"].ToString();

                        result.Add(detalle);
                    }

                    conn.cerrar();
                }
            }

            return result;
        }

        public string verificaSclEnRetencionElect(string retescl)
        {
            string secuencial = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("retescl", retescl));

                string query = "SELECT DISTINCT secuencial FROM tbReteElect WHERE retescl = @retescl AND etdofac = 'N' LIMIT 1";
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

        public int obtenerSecuencialRetencion()
        {
            int secuencial = 1;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT COALESCE(MAX(secuencial::bigint) + 1, 1) AS secuencial FROM tbReteElect";
                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    secuencial = int.Parse(dt.Rows[0]["secuencial"].ToString());
                }

                conn.cerrar();
            }

            return secuencial;
        }

        public Respuesta registraRetencionElect(string retescl, string secuencial, string retetip, string claveAcceso){
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("p_retescl", retescl));
                parametros.Add(new SqlParameter("p_secuencial", secuencial));
                parametros.Add(new SqlParameter("p_retetip", retetip == "Compra" ? "C" : "V"));
                parametros.Add(new SqlParameter("p_claveacceso", claveAcceso));

                string query = "SELECT * FROM creaReteElect(@p_retescl, @p_secuencial, @p_retetip ,@p_claveacceso)";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }

                conn.cerrar();
            }
            return res;
        }

        public void undoSecuencialRetencion()
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("tipo", "R")
                };

                string query = "SELECT * FROM fnUndoSecuencial(@tipo)";
                conn.ejecutaQuery(query, parametros);

                conn.cerrar();
            }
        }

        public mFacturasSRI getRetencionesToSRIByReteScl(string retencionScl)
        {
            mFacturasSRI fact = new mFacturasSRI();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("vsecuencial", retencionScl)
                };

                string query = "SELECT * FROM fntoelectfactByScl(@vsecuencial, 'R')";
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
                    fact.rucDev = row["rucdev"]?.ToString();
                }
            }

            return fact;
        }
    }
}

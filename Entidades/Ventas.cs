using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Entidades
{
    public class Ventas : EntidadBase
    {
        public int ventscl { get; set; } = -1;
        public String cajacod { get; set; } = "0001";
        public String empdnid { get; set; } = "0001";
        public string celEmpl { get; set; } = "";
        public string empdNbr { get; set; } = "";
        public string emprruc { get; set; } = "";
        public string emprnbr { get; set; } = "";
        public String nidnid { get; set; } = "9999999999";
        public String nidnbr { get; set; } = "CLIENTE FINAL";
        public String niddir { get; set; } = "";
        public String nidtlf { get; set; } = "";
        public String nideml { get; set; } = "";
        public bool nidcdto { get; set; } = false;
        public bool cdtopem { get; set; } = false;
        public String venttip { get; set; } = "L";
        public String ventcrr { get; set; } = "0000";
        public String ventccd { get; set; } = ""; // Código generado del carrier
        public String ventcro { get; set; } = ""; // Observación del pedido por carrier
        public Double crrprc { get; set; } = 0;
        public Boolean crrpct { get; set; } = false;
        public String ventmsa { get; set; }
        public String ventlta { get; set; } = "A";
        public String peddscl { get; set; }
        public string peddSclTmp { get; set; } = "";
        public Double ventsbt { get; set; } = 0;
        public Double ventsbn { get; set; } = 0;
        public Double ventdsc { get; set; } = 0;
        public int ventdpc { get; set; } = 0;
        public Double ventiva { get; set; } = 0;
        public Double venttot { get; set; } = 0;
        public Double ventrcb { get; set; } = 0;
        public Double ventvto { get; set; } = 0;
        public String ventobv { get; set; } = "";
        public Boolean ventatv { get; set; } = true;
        public Boolean ventfac { get; set; } = false;
        public Boolean ventpgd { get; set; } = false;
        public string ventStd { get; set; } = "";
        public string ventsep { get; set; } = "C"; // C = Completo : S = Separado
        public string vtaCliDo { get; set; } = ""; // Cliente Domicilio
        public string vtaCliDoObv { get; set; } = ""; // Observacion cliente domicilio
        public string vtaCliDoNom { get; set; } = ""; // Nombre cliente domicilio

        public String cliNid { get; set; }
        public String cliNbr { get; set; }
        public String cliTlf { get; set; }
        public String cliDir { get; set; }
        public String cliRef { get; set; }

        public List<VentaDetalle> detVenta = new List<VentaDetalle>();
        public List<mPago> detPago = new List<mPago>();

        public string etdofac { get; set; }
        public string secuencial { get; set; }
        public string clave_download { get; set; }

        // ------------------------------------------------------------------------------
        public bool ntcrcrd { get; set; } = false; // Nota de credito existente
        public string ntcrscl { get; set; }
        // ------------------------------------------------------------------------------

        public Ventas(string strConnection) : base(strConnection)
        {
        }

        public DataTable getPedidosFacturar()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnGetPedidosFacturar();";

                try
                {
                    dt = conn.ejecutaQuery(query);
                }
                finally
                {
                    conn.cerrar();
                }
            }

            if (dt != null)
            {
                dt.TableName = "Listado";
            }
            else
            {
                dt = new DataTable("Listado");
            }

            return dt;
        }

        public void getPedido(String codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnListPeddVent(@codigo);";

                List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow mainRow = dt.Rows[0];

                    this.ventscl = mainRow["ventScl"] is int vScl ? vScl : int.Parse(mainRow["ventScl"].ToString());
                    this.peddscl = mainRow["peddScl"]?.ToString();
                    this.ventlta = mainRow["peddLta"]?.ToString();
                    this.ventmsa = mainRow["peddMsa"]?.ToString();
                    this.empdnid = mainRow["usuaCod"]?.ToString();
                    this.venttip = mainRow["peddTip"]?.ToString();
                    this.ventcrr = mainRow["ventCrr"]?.ToString();

                    this.ventfac = mainRow["ventfac"] is bool vFac ? vFac : Convert.ToBoolean(mainRow["ventfac"]);

                    this.ventsbt = mainRow["ventsbt"] is double vSbt ? vSbt : Utils.Helper.StringToDouble(mainRow["ventsbt"]?.ToString());
                    this.ventsbn = mainRow["ventsbn"] is double vSbn ? vSbn : Utils.Helper.StringToDouble(mainRow["ventsbn"]?.ToString());
                    this.ventdsc = mainRow["ventdsc"] is double vDsc ? vDsc : Utils.Helper.StringToDouble(mainRow["ventdsc"]?.ToString());
                    this.ventiva = mainRow["ventiva"] is double vIva ? vIva : Utils.Helper.StringToDouble(mainRow["ventiva"]?.ToString());
                    this.venttot = mainRow["venttot"] is double vTot ? vTot : Utils.Helper.StringToDouble(mainRow["venttot"]?.ToString());
                    this.ventrcb = mainRow["ventrcb"] is double vRcb ? vRcb : Utils.Helper.StringToDouble(mainRow["ventrcb"]?.ToString());
                    this.ventvto = mainRow["ventvto"] is double vVto ? vVto : Utils.Helper.StringToDouble(mainRow["ventvto"]?.ToString());

                    this.nidnbr = mainRow["nidnbr"]?.ToString();
                    this.nidnid = mainRow["nidnid"]?.ToString();

                    this.crrprc = mainRow["crrprc"] is double cPrc ? cPrc : Utils.Helper.StringToDouble(mainRow["crrprc"]?.ToString());
                    this.crrpct = mainRow["crrpct"] is bool cPct ? cPct : Convert.ToBoolean(mainRow["crrpct"]);
                    this.ventStd = mainRow["ventstd"]?.ToString();

                    this.cliNid = mainRow["clieNid"]?.ToString();
                    this.cliNbr = mainRow["clieNbr"]?.ToString();
                    this.cliDir = mainRow["clieDir"]?.ToString();
                    this.cliTlf = mainRow["clieTlf"]?.ToString();
                    this.cliRef = mainRow["clieRef"]?.ToString();
                    this.peddSclTmp = mainRow["peddSclTmp"]?.ToString();

                    this.detVenta.Clear();

                    if (conn.conectar())
                    {
                        query = @"SELECT p.peddscl, p.itemcod, p.itemcnt, p.itemiva, p.itemprc, p.itempiv, p.itempto, 
                                         CASE itemSbg 
                                             WHEN TRUE THEN s.sgrpnbr 
                                             ELSE i.itemnbr 
                                         END itemnbr 
                                  FROM   tbPeddMPedd p 
                                         INNER JOIN tbIvtrItem i ON i.itemcod = p.itemcod 
                                         INNER JOIN tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod 
                                  WHERE  p.peddscl = @codigo 
                                         AND p.itemcod NOT IN ( 
                                             SELECT  d.itemcod 
                                             FROM    tbPeddDVent v 
                                                     JOIN tbPeddMVent d ON v.ventscl = d.ventscl 
                                             WHERE   v.peddscl = p.peddscl 
                                                     AND v.ventsep = 'S' 
                                                     AND v.ventstd <> 'A' 
                                         ) 
                                         AND p.peddatv = true";

                        List<SqlParameter> parametros1 = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                        DataTable dtr = conn.ejecutaQuery(query, parametros1);
                        conn.cerrar();

                        if (dtr != null && dtr.Rows.Count > 0)
                        {
                            this.detVenta.Capacity = dtr.Rows.Count;

                            foreach (DataRow dr in dtr.Rows)
                            {
                                this.detVenta.Add(new VentaDetalle
                                {
                                    itemcod = dr["itemCod"]?.ToString(),
                                    itemcnt = dr["itemcnt"] is int iCnt ? iCnt : int.Parse(dr["itemcnt"].ToString()),
                                    itemiva = dr["itempiv"] is double iIva ? iIva : Utils.Helper.StringToDouble(dr["itempiv"]?.ToString()), 
                                    itemprc = dr["itemprc"] is double iPrc ? iPrc : Utils.Helper.StringToDouble(dr["itemprc"]?.ToString()),
                                    itempto = dr["itempto"] is double iPto ? iPto : Utils.Helper.StringToDouble(dr["itempto"]?.ToString()),
                                    nbrprd = dr["itemNbr"]?.ToString()
                                });
                            }
                        }
                    }
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }
        }

        public void getVendido(String codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                string query = @"SELECT v.ventScl, v.usuaCod, v.nidNid, v.ventTip, v.ventcrr, v.ventMsa, 
                                        v.ventLta, v.peddScl, v.ventSbt, v.ventDpc, v.ventDsc, v.ventIva, 
                                        v.ventTot, v.ventRcb, v.ventVto, c.nidNbr, c.nidDir, c.nidTlf, v.ventpgd, v.ventfac, v.ventstd, 
                                        d.clienid, d.clienbr, d.cliedir, d.clietlf, d.clieref, COALESCE(f.etdofac,'') AS etdofac, 
                                        COALESCE(f.secuencial,'') AS secuencial, v.ventstn 
                                 FROM   tbPeddDVent v 
                                 LEFT OUTER JOIN tbFactElect f ON f.ventscl = v.ventscl 
                                 INNER JOIN tbAdsiNid c ON c.nidNid = v.nidNid 
                                 LEFT OUTER JOIN tbPeddDatCli d ON d.peddscl = v.peddscl 
                                 WHERE  v.ventScl = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow mainRow = dt.Rows[0];

                    this.ventscl = mainRow["ventScl"] is int vScl ? vScl : int.Parse(mainRow["ventScl"].ToString());
                    this.peddscl = mainRow["peddScl"]?.ToString();
                    this.ventlta = mainRow["ventLta"]?.ToString();
                    this.ventmsa = mainRow["ventMsa"]?.ToString();
                    this.empdnid = mainRow["usuaCod"]?.ToString();
                    this.venttip = mainRow["ventTip"]?.ToString();
                    this.ventcrr = mainRow["ventcrr"]?.ToString();

                    this.nidnid = mainRow["nidNid"]?.ToString();
                    this.nidnbr = mainRow["nidNbr"]?.ToString();
                    this.niddir = mainRow["nidDir"]?.ToString();
                    this.nidtlf = mainRow["nidTlf"]?.ToString();

                    this.cliDir = mainRow["cliedir"]?.ToString();
                    this.cliNid = mainRow["clienid"]?.ToString();
                    this.cliNbr = mainRow["clienbr"]?.ToString();
                    this.cliTlf = mainRow["clietlf"]?.ToString();
                    this.cliRef = mainRow["clieref"]?.ToString();

                    this.ventrcb = mainRow["ventRcb"] is double vRcb ? vRcb : Utils.Helper.StringToDouble(mainRow["ventRcb"]?.ToString());
                    this.ventvto = mainRow["ventVto"] is double vVto ? vVto : Utils.Helper.StringToDouble(mainRow["ventVto"]?.ToString());
                    this.ventdpc = mainRow["ventDpc"] is int vDpc ? vDpc : int.Parse(mainRow["ventDpc"].ToString());
                    this.ventsbt = mainRow["ventSbt"] is double vSbt ? vSbt : Utils.Helper.StringToDouble(mainRow["ventSbt"]?.ToString());
                    this.ventsbn = mainRow["ventStn"] is double vStn ? vStn : Utils.Helper.StringToDouble(mainRow["ventStn"]?.ToString());
                    this.ventdsc = mainRow["ventDsc"] is double vDsc ? vDsc : Utils.Helper.StringToDouble(mainRow["ventDsc"]?.ToString());
                    this.venttot = mainRow["ventTot"] is double vTot ? vTot : Utils.Helper.StringToDouble(mainRow["ventTot"]?.ToString());
                    this.ventiva = mainRow["ventIva"] is double vIva ? vIva : Utils.Helper.StringToDouble(mainRow["ventIva"]?.ToString());

                    this.ventpgd = mainRow["ventpgd"] is bool vPgd ? vPgd : Convert.ToBoolean(mainRow["ventpgd"]);
                    this.ventfac = mainRow["ventfac"] is bool vFac ? vFac : Convert.ToBoolean(mainRow["ventfac"]);
                    this.ventStd = mainRow["ventstd"]?.ToString();
                    this.etdofac = mainRow["etdofac"]?.ToString();
                    this.secuencial = mainRow["secuencial"]?.ToString();

                    this.detVenta.Clear();
                    this.detPago.Clear();

                    if (conn.conectar())
                    {
                        query = @"SELECT p.ventscl, p.itemcod, p.itemcnt, p.itemiva, p.itemprc, p.itempto, 
                                         CASE itemSbg 
                                             WHEN TRUE THEN s.sgrpNbr 
                                             ELSE i.itemnbr 
                                         END itemnbr 
                                  FROM   tbPeddMVent p 
                                  INNER JOIN tbIvtrItem i on i.itemcod = p.itemcod 
                                  INNER JOIN tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod 
                                  WHERE  p.ventScl = @codigo";

                        List<SqlParameter> parametros2 = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                        DataTable dtr = conn.ejecutaQuery(query, parametros2);
                        conn.cerrar();

                        if (dtr != null && dtr.Rows.Count > 0)
                        {
                            this.detVenta.Capacity = dtr.Rows.Count;

                            foreach (DataRow dr in dtr.Rows)
                            {
                                this.detVenta.Add(new VentaDetalle
                                {
                                    itemcod = dr["itemCod"]?.ToString(),
                                    itemcnt = dr["itemcnt"] is int iCnt ? iCnt : int.Parse(dr["itemcnt"].ToString()),
                                    itemiva = dr["itemiva"] is double iIva ? iIva : Utils.Helper.StringToDouble(dr["itemiva"]?.ToString()),
                                    itemprc = dr["itemprc"] is double iPrc ? iPrc : Utils.Helper.StringToDouble(dr["itemprc"]?.ToString()),
                                    itempto = dr["itempto"] is double iPto ? iPto : Utils.Helper.StringToDouble(dr["itempto"]?.ToString()),
                                    nbrprd = dr["itemNbr"]?.ToString()
                                });
                            }
                        }
                    }
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }
        }

        public void getPedidoByVentScl(String codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                string query = @"SELECT v.ventScl, v.usuacod, v.nidnid, v.venttip, v.ventcrr, v.ventmsa, v.ventlta, 
                                        v.peddscl, v.ventsbt, v.ventdpc, v.ventdsc, v.ventiva, v.venttot, v.ventrcb, 
                                        v.ventvto, c.nidNbr, c.niddir, c.nidtlf, v.ventpgd, v.ventfac, v.ventstd, v.ventstn 
                                 FROM   tbPeddDPedd p 
                                 JOIN   tbPeddDVent v ON v.peddscl = p.peddscl 
                                 JOIN   tbAdsiNid c ON c.nidnid = v.nidnid 
                                 WHERE  v.ventscl = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow mainRow = dt.Rows[0];

                    this.ventscl = mainRow["ventScl"] is int vScl ? vScl : int.Parse(mainRow["ventScl"].ToString());
                    this.peddscl = mainRow["peddScl"]?.ToString();
                    this.ventlta = mainRow["ventLta"]?.ToString();
                    this.ventmsa = mainRow["ventMsa"]?.ToString();
                    this.empdnid = mainRow["usuaCod"]?.ToString();
                    this.venttip = mainRow["ventTip"]?.ToString();
                    this.ventcrr = mainRow["ventcrr"]?.ToString();

                    this.niddir = mainRow["nidDir"]?.ToString();
                    this.nidnid = mainRow["nidNid"]?.ToString();
                    this.nidnbr = mainRow["nidNbr"]?.ToString();
                    this.nidtlf = mainRow["nidTlf"]?.ToString();

                    if (this.venttip == "D")
                    {
                        this.cliDir = this.niddir;
                        this.cliNid = this.nidnid;
                        this.cliNbr = this.nidnbr;
                        this.cliTlf = this.nidtlf;
                    }

                    this.ventrcb = mainRow["ventRcb"] is double vRcb ? vRcb : Utils.Helper.StringToDouble(mainRow["ventRcb"]?.ToString());

                    double vuelto = mainRow["ventVto"] is double vVto ? vVto : Utils.Helper.StringToDouble(mainRow["ventVto"]?.ToString());
                    this.ventvto = vuelto < 0 ? 0 : vuelto;

                    this.ventdpc = mainRow["ventDpc"] is int vDpc ? vDpc : int.Parse(mainRow["ventDpc"].ToString());
                    this.ventsbt = mainRow["ventsbt"] is double vSbt ? vSbt : Utils.Helper.StringToDouble(mainRow["ventsbt"]?.ToString());
                    this.ventsbn = mainRow["ventStn"] is double vStn ? vStn : Utils.Helper.StringToDouble(mainRow["ventStn"]?.ToString());
                    this.ventdsc = mainRow["ventDsc"] is double vDsc ? vDsc : Utils.Helper.StringToDouble(mainRow["ventDsc"]?.ToString());
                    this.ventiva = mainRow["ventIva"] is double vIva ? vIva : Utils.Helper.StringToDouble(mainRow["ventIva"]?.ToString());
                    this.venttot = mainRow["ventTot"] is double vTot ? vTot : Utils.Helper.StringToDouble(mainRow["venttot"]?.ToString());

                    this.ventpgd = mainRow["ventpgd"] is bool vPgd ? vPgd : Convert.ToBoolean(mainRow["ventpgd"]);
                    this.ventfac = mainRow["ventfac"] is bool vFac ? vFac : Convert.ToBoolean(mainRow["ventfac"]);
                    this.ventStd = mainRow["ventstd"]?.ToString();

                    this.detVenta.Clear();
                    this.detPago.Clear();

                    if (conn.conectar())
                    {
                        query = @"SELECT p.ventscl, p.itemcod, p.itemcnt, p.itemiva, p.itemprc, p.itempto, 
                                         CASE itemSbg 
                                             WHEN TRUE THEN s.sgrpNbr 
                                             ELSE i.itemnbr 
                                         END itemnbr 
                                  FROM   tbPeddMVent p 
                                  INNER JOIN tbIvtrItem i on i.itemcod = p.itemcod 
                                  INNER JOIN tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod 
                                  WHERE  p.ventScl = @codigo";

                        List<SqlParameter> parametros2 = new List<SqlParameter> { new SqlParameter("codigo", codigo) };
                        DataTable dtr = conn.ejecutaQuery(query, parametros2);
                        conn.cerrar();

                        if (dtr != null && dtr.Rows.Count > 0)
                        {
                            this.detVenta.Capacity = dtr.Rows.Count;

                            foreach (DataRow dr in dtr.Rows)
                            {
                                this.detVenta.Add(new VentaDetalle
                                {
                                    itemcod = dr["itemCod"]?.ToString(),
                                    itemcnt = dr["itemcnt"] is int iCnt ? iCnt : int.Parse(dr["itemcnt"].ToString()),
                                    itemiva = dr["itemiva"] is double iIva ? iIva : Utils.Helper.StringToDouble(dr["itemiva"]?.ToString()),
                                    itemprc = dr["itemprc"] is double iPrc ? iPrc : Utils.Helper.StringToDouble(dr["itemprc"]?.ToString()),
                                    itempto = dr["itempto"] is double iPto ? iPto : Utils.Helper.StringToDouble(dr["itempto"]?.ToString()),
                                    nbrprd = dr["itemNbr"]?.ToString()
                                });
                            }
                        }
                    }
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }

        }

        public mVenta getVenta(string cliente, string factura)
        {
            mVenta res = new mVenta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("p_cliente", cliente),
                    new SqlParameter("p_factura", factura)
                };

                string query = "SELECT * FROM fnVerificaVenta(@p_factura, @p_cliente)";
                DataTable dt = null;

                try
                {
                    dt = conn.ejecutaQuery(query, parametros);
                }
                finally
                {
                    conn.cerrar();
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    res.ventscl = dr["ventscl"]?.ToString();
                    res.nidnid = dr["nidnid"]?.ToString();
                    res.ventsbt = dr["ventsbt"] is double dsbt ? dsbt : Utils.Helper.StringToDouble(dr["ventsbt"]?.ToString());
                    res.ventiva = dr["ventiva"] is double diva ? diva : Utils.Helper.StringToDouble(dr["ventiva"]?.ToString());
                    res.venttot = dr["venttot"] is double dtot ? dtot : Utils.Helper.StringToDouble(dr["venttot"]?.ToString());
                }
            }

            return res;

        }

        public String getVentaId(String peddScl)
        {
            string resp = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("pedido", peddScl)
                };

                string query = "SELECT ventScl FROM tbPeddDVent WHERE peddScl = @pedido LIMIT 1";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    resp = dt.Rows[0]["ventScl"]?.ToString() ?? "";
                }
            }

            return resp;

        }

        public Respuesta anulaFactura(String ventScl, String ventObv, string numFact)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("accion", "A"),
                    new SqlParameter("data", ventscl.ToString().PadLeft(10, '0')),
                    new SqlParameter("pago", ""),
                    new SqlParameter("detalle", JsonConvert.SerializeObject(new
                    {
                        ventobv = ventObv,
                        empdnid = empdnid,
                        numfact = numFact
                    }))
                };

                string query = "SELECT * FROM fnVentas(@accion, @data::varchar, @detalle::varchar, @pago::varchar)";
                DataTable dt = null;

                try
                {
                    dt = conn.ejecutaQuery(query, parametros);
                }
                finally
                {
                    conn.cerrar();
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    int codigo = row["codigo"] is int cInt ? cInt : int.Parse(row["codigo"].ToString());

                    res.resultado = (codigo == 1);
                    res.mensaje = row["descripcion"]?.ToString() ?? "";
                }
            }

            return res;

        }

        public Respuesta eliminaFactura(String ventScl, String ventObv, string numFact)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("accion", "E"),
                    new SqlParameter("data", ventscl.ToString().PadLeft(10, '0')),
                    new SqlParameter("pago", ""),
                    new SqlParameter("detalle", JsonConvert.SerializeObject(new
                    {
                        ventobv = ventObv,
                        empdnid = empdnid,
                        numfact = numFact
                    }))
                };

                string query = "SELECT * FROM fnVentas(@accion, @data::varchar, @detalle::varchar, @pago::varchar)";
                DataTable dt = null;

                try
                {
                    dt = conn.ejecutaQuery(query, parametros);
                }
                finally
                {
                    conn.cerrar();
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    int codigo = row["codigo"] is int cInt ? cInt : int.Parse(row["codigo"].ToString());

                    res.resultado = (codigo == 1);
                    res.mensaje = row["descripcion"]?.ToString() ?? "";
                }
            }

            return res;
        }

        public DataTable getMotivosDescuento(String tipo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("tipo", tipo)
                };

                string query = "SELECT mtvoScl AS codigo, mtvoNbr AS descripcion FROM tbAdsiMtvos WHERE mtvoTip = @tipo AND mtvoAtv = TRUE";

                try
                {
                    dt = conn.ejecutaQuery(query, parametros);
                }
                finally
                {
                    conn.cerrar();
                }
            }

            if (dt != null)
            {
                dt.TableName = "Listado";
            }
            else
            {
                dt = new DataTable("Listado");
            }

            return dt;
        }

        public DataTable buscarPedidoFacturado(String codigo)
        {
            DataTable dt = new DataTable("Listado");
            codigo = codigo.PadLeft(10, '0');

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("codigo", codigo)
                    };

                    string query = @"
                            SELECT v.peddScl AS codigo, 
                                   v.ventscl, 
                                   CASE v.venttip WHEN 'L' THEN 'LOCAL' WHEN 'D' THEN 'DOMICILIO' ELSE 'LLEVAR' END AS tipo, 
                                   v.ventmsa || ' ' || v.ventlta AS mesa, 
                                   v.ventfac, 
                                   v.ventfac AS facturado, 
                                   COALESCE(f.etdofac, '') AS etdofac, 
                                   COALESCE(f.secuencial, '') AS secuencial 
                            FROM   tbPeddDVent v 
                                   LEFT OUTER JOIN tbfactelect f ON v.ventscl = f.ventscl 
                            WHERE  v.peddScl = @codigo 
                                   AND v.ventstd <> 'E' 
                            ORDER BY v.ventscl";

                    DataTable dtResultado = conn.ejecutaQuery(query, parametros);

                    if (dtResultado != null)
                    {
                        dt = dtResultado;
                        dt.TableName = "Listado";
                    }

                    conn.cerrar();
                }
            }

            return dt;

        }

        public DataTable buscarUltimo()
        {
            DataTable dt = new DataTable("Listado");

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    string query = @"SELECT v.peddScl AS codigo, 
                                            v.ventscl, 
                                            CASE v.venttip WHEN 'L' THEN 'LOCAL' WHEN 'D' THEN 'DOMICILIO' ELSE 'LLEVAR' END AS tipo, 
                                            v.ventmsa || ' ' || v.ventlta AS mesa, 
                                            v.ventfac, 
                                            v.ventfac AS facturado, 
                                            COALESCE(f.etdofac, '') AS etdofac, 
                                            COALESCE(f.secuencial, '') AS secuencial 
                                    FROM   tbPeddDVent v 
                                            LEFT OUTER JOIN tbfactelect f ON v.ventscl = f.ventscl 
                                    WHERE  v.ventstd <> 'E' 
                                    ORDER BY v.ventscl DESC 
                                    LIMIT 1";

                    var parametros = new List<SqlParameter>();

                    DataTable dtResultado = conn.ejecutaQuery(query, parametros);

                    if (dtResultado != null)
                    {
                        dt = dtResultado;
                        dt.TableName = "Listado";
                    }

                    conn.cerrar();
                }
            }

            return dt;

        }

        private string decodificaTipoVenta(string tipo)
        {
            switch (tipo)
            {
                case "L":
                    return "Local";
                case "D":
                    return "Domicilio";
                case "C":
                    return "Llevar";
                case "R":
                    return "Retirar";
                case "E":
                    return "Delivery";
                default:
                    return "";
            }
        }

        public bool checkIfVentWasProcess(string ventscl)
        {
            DbConnection conn = new DbConnection(strConnection);
            bool respuesta = false;

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ventscl", ventscl));

                string query = "SELECT * from tbFactelect " +
                               "WHERE   ventscl = @ventscl " +
                               "        AND etdofac = 'A'";
                DataTable dt = conn.ejecutaQuery(query, parametros);
                if(dt != null)
                {
                    if (dt.Rows.Count > 0)
                        respuesta = true;
                    
                }
            }

            return respuesta;
        }

        public List<mFacturasSRI> getFacturasToSRI(string fecha, string filtro = "")
        {
            List<mFacturasSRI> resp = new List<mFacturasSRI>();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("fecha", fecha),
                    new SqlParameter("filtro", filtro)
                };

                string query = "SELECT * FROM fnToElectFact(@fecha, @filtro)";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null && dt.Rows.Count > 0)
                {
                    resp.Capacity = dt.Rows.Count;

                    foreach (DataRow dr in dt.Rows)
                    {
                        mFacturasSRI fact = new mFacturasSRI();

                        fact.ventscl = dr["ventscl"]?.ToString();
                        fact.cajacod = dr["cajacod"]?.ToString();
                        fact.usuacod = dr["usuacod"]?.ToString();
                        fact.ventfch = dr["ventfch"]?.ToString();
                        fact.nidnid = dr["nidnid"]?.ToString();
                        fact.nidtip = dr["nidtip"]?.ToString();
                        fact.nidnbr = dr["nidnbr"]?.ToString();
                        fact.niddir = dr["niddir"]?.ToString();
                        fact.nideml = dr["nideml"]?.ToString();
                        fact.nidtlf = dr["nidtlf"]?.ToString();
                        fact.rucDev = dr["rucdev"]?.ToString();

                        string tipo = dr["tipo"]?.ToString();

                        switch (tipo)
                        {
                            case "F":
                                fact.docutip = "Factura";
                                fact.docuToolTip = "Factura";
                                fact.docuTipRet = "";
                                break;
                            case "A":
                            case "D":
                                fact.docutip = "NotaDeCredito";
                                fact.docuToolTip = "Nota de Crédito";
                                fact.docuTipRet = (tipo == "A") ? "Anulación" : "Devolución";
                                break;
                            default:
                                fact.docutip = "Retención";
                                fact.docuToolTip = "Retención";
                                fact.docuTipRet = (tipo == "C") ? "Compra" : "Venta";
                                break;
                        }

                        fact.ventmsa = dr["ventmsa"]?.ToString();
                        fact.ventlta = dr["ventlta"]?.ToString();
                        fact.venttip = dr["venttip"]?.ToString();
                        fact.peddscl = dr["peddscl"]?.ToString();
                        fact.venthra = dr["venthra"]?.ToString();

                        if (dr["fechemi"] != DBNull.Value && !string.IsNullOrEmpty(dr["fechemi"].ToString()))
                        {
                            fact.fechemi = dr["fechemi"] is DateTime time ? time : DateTime.Parse(dr["fechemi"].ToString());
                        }

                        fact.ventsbt = Math.Round(dr["ventsbt"] is double dsbt ? dsbt : Utils.Helper.StringToDouble(dr["ventsbt"]?.ToString()), 6);
                        fact.ventsbn = Math.Round(dr["ventstn"] is double dsbn ? dsbn : Utils.Helper.StringToDouble(dr["ventstn"]?.ToString()), 6);
                        fact.ventdsc = Math.Round(dr["ventdsc"] is double ddsc ? ddsc : Utils.Helper.StringToDouble(dr["ventdsc"]?.ToString()), 6);
                        fact.ventiva = Math.Round(dr["ventiva"] is double diva ? diva : Utils.Helper.StringToDouble(dr["ventiva"]?.ToString()), 6);
                        fact.venttot = Math.Round(dr["venttot"] is double dtot ? dtot : Utils.Helper.StringToDouble(dr["venttot"]?.ToString()), 6);
                        fact.ventrcb = Math.Round(dr["ventrcb"] is double drcb ? drcb : Utils.Helper.StringToDouble(dr["ventrcb"]?.ToString()), 6);
                        fact.ventvto = Math.Round(dr["ventvto"] is double dvto ? dvto : Utils.Helper.StringToDouble(dr["ventvto"]?.ToString()), 6);

                        string errCod = dr["errcod"]?.ToString();
                        string etdofac = dr["etdofac"]?.ToString();

                        var (estPro, estProImg) = etdofac switch
                        {
                            "A" => ("Autorizado", "../../images/authorize2.png"),
                            "D" => ("Devuelta", errCod == "0101" ? "../../images/noreturned.png" : "../../images/returned.png"),
                            "P" => ("Pendiente", "../../images/pending.png"),
                            "V" => ("Validado", "../../images/isvalid.png"),
                            "N" => ("No Procesado", "../../images/toprocess.png"),
                            "L" => ("Anulado", "../../images/cancel.png"),
                            _ => ("", "")
                        };

                        fact.etdofac = estPro;
                        fact.etdofacdet = dr["errmsg"]?.ToString() ?? "";

                        if (errCod == "0101")
                        {
                            fact.etdofacdet = $"{fact.etdofacdet}{System.Environment.NewLine} >> No será procesada ni enviada nuevamente al SRI";
                        }

                        fact.etdofacImg = estProImg;
                        fact.etdoeml = dr["etdoeml"]?.ToString();
                        fact.docuSri = dr["docusri"]?.ToString();
                        fact.errCode = errCod;
                        fact.secuencial = dr["secuencial"]?.ToString();
                        fact.autosri = dr["autosri"]?.ToString();
                        fact.claveDownload = dr["ventclv"]?.ToString();

                        resp.Add(fact);
                    }
                }
            }
            return resp;
        }

        public mFacturasSRI getFacturasToSRIByVentcls(string ventaScl)
        {
            mFacturasSRI fact = new mFacturasSRI();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("vsencuencial", ventaScl)
                };

                string query = "SELECT * FROM fntoelectfactByScl(@vsencuencial, 'F')";
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
                            fact.docuTipRet = (tipo == "A") ? "Anulación" : "Devolución"; // Operador ternario más rápido
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

        public FactElect getFacturaElectronica(string ventscl)
        {
            var respuesta = new FactElect();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("ventscl", ventscl)
                    };

                    string query = @"SELECT ventscl, autosri, secuencial, numefac, etdofac, etdoeml, mensaje 
                                     FROM   tbFactElect 
                                     WHERE  ventscl = @ventscl";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];

                        respuesta.ventscl = dr["ventscl"]?.ToString() ?? string.Empty;
                        respuesta.autosri = dr["autosri"]?.ToString() ?? string.Empty;
                        respuesta.secuencial = dr["secuencial"]?.ToString() ?? string.Empty;
                        respuesta.numefac = dr["numefac"]?.ToString() ?? string.Empty;
                        respuesta.etdofac = dr["etdofac"]?.ToString() ?? string.Empty;
                        respuesta.etdoeml = dr["etdoeml"]?.ToString() ?? string.Empty;
                        respuesta.mensaje = dr["mensaje"]?.ToString() ?? string.Empty;
                    }

                    conn.cerrar();
                }
            }

            return respuesta;
        }

        public List<mPago> getPagosVenta(string secuencial)
        {
            var resp = new List<mPago>();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("ventscl", secuencial)
                    };

                    string query = @"SELECT fpgobco, fpgoval, fpgovto, fpgotip, fpgotpt, fpgomss 
                                     FROM   tbPedddFPgo 
                                     WHERE  ventscl = @ventscl";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string fpgotip = dr["fpgotip"]?.ToString() ?? string.Empty;
                            string fpgotpt = dr["fpgotpt"]?.ToString() ?? string.Empty;

                            var pago = new mPago
                            {
                                banco = dr["fpgobco"]?.ToString() ?? string.Empty,
                                valor = Utils.Helper.StringToDouble(dr["fpgoval"]?.ToString() ?? "0"),
                                vuelto = Utils.Helper.StringToDouble(dr["fpgovto"]?.ToString() ?? "0"),

                                isefectivo = fpgotip == "E",
                                ischeque = fpgotip == "C",
                                iscorriente = fpgotip == "T" && fpgotpt == "C",
                                isdiferido = fpgotip == "T" && fpgotpt == "D",

                                meses = int.TryParse(dr["fpgomss"]?.ToString(), out int m) ? m : 0
                            };

                            resp.Add(pago);
                        }
                    }

                    conn.cerrar();
                }
            }

            return resp;

        }

        public List<mDetalleVenta> getDetalleVenta(string secuencial)
        {
            var resp = new List<mDetalleVenta>();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("ventscl", secuencial)
                    };

                    string query = @"SELECT d.ventscl, d.itemcod, d.itemcnt, d.itemprc, d.itemiva, d.itempto, i.itemNbr 
                                     FROM   tbPeddMVent d 
                                            INNER JOIN tbIvtrItem i ON d.itemcod = i.itemcod 
                                     WHERE  d.ventscl = @ventscl";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            var detalle = new mDetalleVenta
                            {
                                ventscl = dr["ventscl"]?.ToString() ?? string.Empty,
                                itemcod = dr["itemcod"]?.ToString() ?? string.Empty,
                                itemnbr = dr["itemnbr"]?.ToString() ?? string.Empty,
                                itemcnt = int.TryParse(dr["itemcnt"]?.ToString(), out int cnt) ? cnt : 0,

                                itemprc = Utils.Helper.StringToDouble(dr["itemprc"]?.ToString() ?? "0"),
                                itemiva = Utils.Helper.StringToDouble(dr["itemiva"]?.ToString() ?? "0"),
                                itempto = Utils.Helper.StringToDouble(dr["itempto"]?.ToString() ?? "0")
                            };

                            resp.Add(detalle);
                        }
                    }

                    conn.cerrar();
                }
            }

            return resp;

        }

        public Respuesta grabar()
        {
            var res = new Respuesta();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    double totalPago = this.detPago?.Sum(x => x.valor) ?? 0;
                    bool esEfectivo = this.venttip == "E";

                    double ventrcb = esEfectivo ? this.venttot : totalPago;
                    double ventvto = esEfectivo ? 0 : totalPago - this.venttot;
                    string ventsepFinal = this.ventsep ?? "C";

                    bool esModificacion = this.ventscl != -1;
                    string accion = esModificacion ? "M" : "N";
                    string jsonData;

                    if (esModificacion)
                    {
                        jsonData = JsonConvert.SerializeObject(new mVenta
                        {
                            ventscl = this.ventscl.ToString().PadLeft(10, '0'),
                            cajacod = this.cajacod,
                            empdnid = this.empdnid,
                            nidnid  = this.nidnid,
                            peddscl = this.peddscl,
                            ventmsa = this.ventmsa,
                            ventlta = this.ventlta,
                            venttip = this.venttip,
                            ventccr = this.ventcrr,
                            ventsbt = this.ventsbt,
                            ventsbn = this.ventsbn,
                            ventdsc = this.ventdsc,
                            ventdpc = this.ventdpc,
                            ventiva = this.ventiva,
                            venttot = this.venttot,
                            ventobv = this.ventobv,
                            ventatv = this.ventatv,
                            ventfac = this.ventfac,
                            ventrcb = ventrcb,
                            ventvto = ventvto,
                            ventsep = ventsepFinal
                        });
                    }
                    else
                    {
                        jsonData = JsonConvert.SerializeObject(new cVenta
                        {
                            cajacod = this.cajacod,
                            empdnid = this.empdnid,
                            nidnid  = this.nidnid,
                            peddscl = this.peddscl,
                            ventmsa = this.ventmsa,
                            ventlta = this.ventlta,
                            venttip = this.venttip,
                            ventccr = this.ventcrr,
                            ventsbt = this.ventsbt,
                            ventsbn = this.ventsbn,
                            ventdsc = this.ventdsc,
                            ventdpc = this.ventdpc,
                            ventiva = this.ventiva,
                            venttot = this.venttot,
                            ventobv = this.ventobv,
                            ventatv = this.ventatv,
                            ventfac = this.ventfac,
                            ventrcb = ventrcb,
                            ventvto = ventvto,
                            ventsep = ventsepFinal
                        });
                    }

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("accion", accion),
                        new SqlParameter("data", jsonData),
                        new SqlParameter("detalle", JsonConvert.SerializeObject(this.detVenta)),
                        new SqlParameter("pago", JsonConvert.SerializeObject(this.detPago))
                    };

                    string query = "SELECT codigo, descripcion FROM fnVentas(@accion, @data::varchar, @detalle::varchar, @pago::varchar)";
                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];

                        res.resultado = fila["codigo"]?.ToString() == "1";
                        res.mensaje = fila["descripcion"]?.ToString() ?? string.Empty;
                    }

                    conn.cerrar();
                }
            }

            return res;
        }

        public Respuesta grabarPedidoFactura()
        {
            var res = new Respuesta();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    double totalPago = this.detPago?.Sum(x => x.valor) ?? 0;
                    bool esEfectivo = this.venttip == "E";

                    double ventrcb = esEfectivo ? this.venttot : totalPago;
                    double ventvto = esEfectivo ? 0 : totalPago - this.venttot;
                    string ventsepFinal = this.ventsep ?? "C";

                    bool esModificacion = this.ventscl != -1;
                    string accion = esModificacion ? "M" : "N";
                    string jsonData;

                    if (esModificacion)
                    {
                        jsonData = JsonConvert.SerializeObject(new mVenta
                        {
                            ventscl = this.ventscl.ToString().PadLeft(10, '0'),
                            cajacod = this.cajacod,
                            empdnid = this.empdnid,
                            nidnid  = this.nidnid,
                            peddscl = this.peddscl,
                            ventmsa = this.ventmsa,
                            ventlta = this.ventlta,
                            venttip = this.venttip,
                            ventccr = this.ventcrr,
                            ventccd = this.ventccd,
                            ventcro = this.ventcro,
                            ventsbt = this.ventsbt,
                            ventsbn = this.ventsbn,
                            ventdsc = this.ventdsc,
                            ventdpc = this.ventdpc,
                            ventiva = this.ventiva,
                            venttot = this.venttot,
                            ventobv = this.ventobv,
                            ventatv = this.ventatv,
                            ventfac = this.ventfac,
                            ventrcb = ventrcb,
                            ventvto = ventvto,
                            ventsep = ventsepFinal
                        });
                    }
                    else
                    {
                        jsonData = JsonConvert.SerializeObject(new cFactura
                        {
                            cajacod = this.cajacod,
                            empdnid = this.empdnid,
                            nidnid  = this.nidnid,
                            cliedir = this.cliDir,
                            clienbr = this.cliNbr,
                            clieref = this.cliRef,
                            clietlf = this.cliTlf,
                            peddscl = "0000000000",
                            ventmsa = this.ventmsa,
                            ventlta = this.ventlta,
                            venttip = this.venttip,
                            ventccr = this.ventcrr,
                            ventccd = this.ventccd,
                            ventcro = this.ventcro,
                            ventsbt = this.ventsbt,
                            ventsbn = this.ventsbn,
                            ventdsc = this.ventdsc,
                            ventdpc = this.ventdpc,
                            ventiva = this.ventiva,
                            venttot = this.venttot,
                            ventobv = this.ventobv,
                            ventatv = this.ventatv,
                            ventfac = this.ventfac,
                            ventrcb = ventrcb,
                            ventvto = ventvto,
                            ventsep = ventsepFinal
                        });
                    }

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("accion", accion),
                        new SqlParameter("data", jsonData),
                        new SqlParameter("detalle", JsonConvert.SerializeObject(this.detVenta)),
                        new SqlParameter("pago", JsonConvert.SerializeObject(this.detPago))
                    };

                    string query = "SELECT codigo, descripcion FROM fnVentasPD(@accion, @data::varchar, @detalle::varchar, @pago::varchar)";
                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];
                        res.resultado = fila["codigo"]?.ToString() == "1";
                        res.mensaje = fila["descripcion"]?.ToString() ?? string.Empty;
                    }

                    if (res.resultado && !string.IsNullOrWhiteSpace(res.mensaje))
                    {
                        string ventsclExtraido = res.mensaje.Split(' ').LastOrDefault()?.ToString();

                        if (!string.IsNullOrEmpty(ventsclExtraido))
                        {
                            parametros.Clear();
                            parametros.Add(new SqlParameter("ventscl", ventsclExtraido));

                            query = "SELECT ventscl, peddscl FROM tbPeddDVent WHERE ventscl = @ventscl";
                            DataTable dts = conn.ejecutaQuery(query, parametros);

                            if (dts != null && dts.Rows.Count > 0)
                            {
                                DataRow drs = dts.Rows[0];
                                int.TryParse(drs["ventscl"]?.ToString(), out int vScl);
                                this.ventscl = vScl;
                                this.peddscl = drs["peddscl"]?.ToString() ?? string.Empty;
                            }
                        }
                    }

                    conn.cerrar();
                }
            }

            return res;
        }

        public Respuesta registraFactElect(string ventscl, string secuencial, string claveAcceso, DateTime fecha,
                                           string tipodoc, string ambiente, string numfact, string ruc, string codigo, string emision)
        {
            var res = new Respuesta();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("p_ventscl", ventscl),
                        new SqlParameter("p_secuencial", secuencial),
                        new SqlParameter("p_claveacceso", claveAcceso),
                        new SqlParameter("p_fecha", fecha),
                        new SqlParameter("p_tipodoc", tipodoc),
                        new SqlParameter("p_ambiente", ambiente),
                        new SqlParameter("p_numfact", numfact),
                        new SqlParameter("p_ruc", ruc),
                        new SqlParameter("p_codigo", codigo),
                        new SqlParameter("p_emision", emision)
                    };

                    string query = @"SELECT codigo, descripcion 
                                     FROM   creaFactElect(@p_ventscl, @p_secuencial, @p_claveacceso, 
                                                          @p_fecha::DATE, @p_tipodoc, @p_ambiente, @p_numfact, 
                                                          @p_ruc, @p_codigo, @p_emision)";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];

                        res.resultado = fila["codigo"]?.ToString() == "1";
                        res.mensaje = fila["descripcion"]?.ToString() ?? string.Empty;
                    }

                    conn.cerrar();
                }
            }

            return res;
        }


        public void verificaFactElect(string ventscl, ref string secuencial, ref string claveAcceso)
        {
            var res = new Respuesta();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("p_ventscl", ventscl)
                    };

                    string query = @"SELECT secuencial, autosri 
                         FROM   tbFactElect 
                         WHERE  ventscl = @p_p_ventscl 
                         LIMIT  1";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];

                        secuencial = fila["secuencial"]?.ToString() ?? string.Empty;
                        claveAcceso = fila["autosri"]?.ToString() ?? string.Empty;
                    }

                    conn.cerrar();
                }
            }

        }

        public Respuesta actualizaFactElect(string ventscl, string secuencial, string claveAcceso, string estado, string estadomail, string mensaje)
        {
            var res = new Respuesta();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("p_ventscl", ventscl),
                        new SqlParameter("p_secuencial", secuencial),
                        new SqlParameter("p_claveacceso", claveAcceso),
                        new SqlParameter("p_estado", estado),
                        new SqlParameter("p_estadomail", estadomail),
                        new SqlParameter("p_mensaje", mensaje ?? string.Empty)
                    };

                    string query = @"SELECT codigo, descripcion 
                                     FROM   actualizaFactElect(@p_ventscl, @p_secuencial, @p_claveacceso, 
                                                               @p_estado, @p_estadomail, @p_mensaje)";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];

                        res.resultado = fila["codigo"]?.ToString() == "1";
                        res.mensaje = fila["descripcion"]?.ToString() ?? string.Empty;
                    }

                    conn.cerrar();
                }
            }

            return res;

        }

        public bool verificaExisteNumeroFactura(string numero, string tipo)
        {
            bool respuesta = false;

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("numero", numero),
                        new SqlParameter("tipo", tipo)
                    };

                    string query = @"SELECT 1 
                                     FROM   tbPeddHVent 
                                     WHERE  histtip = @tipo 
                                            AND ventnfc = @numero 
                                     LIMIT  1";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        respuesta = true;
                    }

                    conn.cerrar();
                }
            }

            return respuesta;
        }

        public Respuesta grabarRegalo()
        {
            return ProcesarVentaRegalo("fnVentas");
        }

        public Respuesta grabarRegaloFP()
        {
            return ProcesarVentaRegalo("fnVentasPD");
        }

        private Respuesta ProcesarVentaRegalo(string nombreFuncionPostgres)
        {
            var res = new Respuesta();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    string dataJson = JsonConvert.SerializeObject(new cVenta
                    {
                        cajacod = this.cajacod,
                        empdnid = this.empdnid,
                        nidnid = this.nidnid,
                        peddscl = this.peddscl,
                        ventmsa = this.ventmsa,
                        ventlta = this.ventlta,
                        venttip = this.venttip,
                        ventsbt = this.ventsbt,
                        ventdsc = this.ventdsc,
                        ventiva = this.ventiva,
                        venttot = this.venttot,
                        ventobv = this.ventobv,
                        ventatv = this.ventatv
                    });

                    string detalleJson = JsonConvert.SerializeObject(this.detVenta);

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("accion", "G"),
                        new SqlParameter("data", dataJson),
                        new SqlParameter("detalle", detalleJson)
                    };

                    string query = $"SELECT codigo, descripcion FROM {nombreFuncionPostgres}(@accion, @data::varchar, @detalle::varchar, '')";
                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];
                        res.resultado = fila["codigo"]?.ToString() == "1";
                        res.mensaje = fila["descripcion"]?.ToString() ?? string.Empty;
                    }

                    conn.cerrar();
                }
            }

            return res;
        }

        public string getClaveAcceso(string ventscl)
        {
            string secuencial = string.Empty;

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("ventscl", ventscl)
                    };

                    string query = @"SELECT autosri, etdofac 
                                     FROM   tbFactElect 
                                     WHERE  ventscl = @ventscl 
                                     LIMIT  1";

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow fila = dt.Rows[0];

                        string autosri = fila["autosri"]?.ToString() ?? string.Empty;
                        string etdofac = fila["etdofac"]?.ToString() ?? string.Empty;

                        secuencial = $"{autosri}:{etdofac}";
                    }

                    conn.cerrar();
                }
            }

            return secuencial;
        }

        public string verificaSclEnFacturaElect(string ventscl)
        {
            string secuencial = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ventscl", ventscl));

                string query = "SELECT DISTINCT secuencial FROM tbFactElect WHERE ventscl = @ventscl AND etdofac = 'N' LIMIT 1";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    if(dt.Rows.Count > 0)
                    {
                        secuencial = dt.Rows[0]["secuencial"].ToString();
                    }
                }

                conn.cerrar();
            }
            return secuencial;
        }

        public int obtenerSecuencialFactura()
        {
            int secuencial = 1;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "SELECT COALESCE(MAX(secuencial::bigint) + 1, 1) AS secuencial FROM tbFactElect";
                DataTable dt = conn.ejecutaQuery(query);

                if (dt != null)
                {
                    secuencial = int.Parse(dt.Rows[0]["secuencial"].ToString());
                }

                conn.cerrar();
            }

            return secuencial;
        }

        public List<mVentas> reporteVentas(int anio, int mes, bool detalle)
        {
            var ventas = new List<mVentas>();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    string query = @"SELECT fecha, tipo, subtotal, descuento, iva, total, efectivo, tarjeta, cheque, transferencia, vuelto 
                                     FROM fnRptVentas(@anio, @mes, @detalle)";

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("anio", anio),
                        new SqlParameter("mes", mes),
                        new SqlParameter("detalle", detalle)
                    };

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            ventas.Add(new mVentas
                            {
                                fecha = dr["fecha"]?.ToString() ?? string.Empty,
                                tipo = dr["tipo"]?.ToString() ?? string.Empty,

                                subtotal = Utils.Helper.StringToDouble(dr["subtotal"]?.ToString() ?? "0"),
                                descuento = Utils.Helper.StringToDouble(dr["descuento"]?.ToString() ?? "0"),
                                iva = Utils.Helper.StringToDouble(dr["iva"]?.ToString() ?? "0"),
                                total = Utils.Helper.StringToDouble(dr["total"]?.ToString() ?? "0"),
                                efectivo = Utils.Helper.StringToDouble(dr["efectivo"]?.ToString() ?? "0"),
                                tarjeta = Utils.Helper.StringToDouble(dr["tarjeta"]?.ToString() ?? "0"),
                                cheque = Utils.Helper.StringToDouble(dr["cheque"]?.ToString() ?? "0"),
                                transferencia = Utils.Helper.StringToDouble(dr["transferencia"]?.ToString() ?? "0"),
                                vuelto = Utils.Helper.StringToDouble(dr["vuelto"]?.ToString() ?? "0")
                            });
                        }
                    }

                    conn.cerrar();
                }
            }

            return ventas;
        }

        public List<mPorCobrar> reporteVentasCobrar(string desde, string hasta, string cliente, int tipo)
        {
            var ventas = new List<mPorCobrar>();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    string query = @"SELECT secuencial, fecha, cantidad, total, descripcion, persona 
                                     FROM fnRptVentasCobrar(@desde, @hasta, @cliente, @tipo)";

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("desde", desde),
                        new SqlParameter("hasta", hasta),
                        new SqlParameter("cliente", cliente),
                        new SqlParameter("tipo", tipo)
                    };

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            DateTime.TryParse(dr["fecha"]?.ToString(), out DateTime fechaMapeada);

                            int.TryParse(dr["cantidad"]?.ToString(), out int cantidadMapeada);

                            ventas.Add(new mPorCobrar
                            {
                                secuencial = dr["secuencial"]?.ToString() ?? string.Empty,
                                fecha = fechaMapeada,
                                cantidad = cantidadMapeada,
                                total = Utils.Helper.StringToDouble(dr["total"]?.ToString() ?? "0"),
                                descripcion = dr["descripcion"]?.ToString() ?? string.Empty,
                                persona = dr["persona"]?.ToString() ?? string.Empty
                            });
                        }
                    }

                    conn.cerrar();
                }
            }

            return ventas;
        }


        public List<mRegalos> reporteRegalos(string desde, string hasta, bool detalle)
        {
            var regalos = new List<mRegalos>();

            using (DbConnection conn = new DbConnection(strConnection))
            {
                if (conn.conectar())
                {
                    string query = detalle
                        ? @"SELECT secuencial, fecha, tipo, caja, empleado, motivo 
                            FROM fnRptRegalos(@desde, @hasta)"
                        : @"SELECT tipo, fecha, caja, empleado, COUNT(*) AS cantidad 
                            FROM fnRptRegalos(@desde, @hasta) GROUP BY tipo, fecha, caja, empleado";

                    var parametros = new List<SqlParameter>
                    {
                        new SqlParameter("desde", desde),
                        new SqlParameter("hasta", hasta)
                    };

                    DataTable dt = conn.ejecutaQuery(query, parametros);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            var regalo = new mRegalos
                            {
                                fecha = dr["fecha"]?.ToString() ?? string.Empty,
                                tipo = dr["tipo"]?.ToString() ?? string.Empty,
                                caja = dr["caja"]?.ToString() ?? string.Empty,
                                empleado = dr["empleado"]?.ToString() ?? string.Empty
                            };

                            if (detalle)
                            {
                                regalo.secuencial = dr["secuencial"]?.ToString() ?? string.Empty;
                                regalo.motivo = dr["motivo"]?.ToString() ?? string.Empty;
                            }
                            else
                            {
                                int.TryParse(dr["cantidad"]?.ToString(), out int cantidadMapeada);
                                regalo.cantidad = cantidadMapeada;
                            }

                            regalos.Add(regalo);
                        }
                    }

                    conn.cerrar();
                }
            }

            return regalos;
        }

        public List<mAnulaciones> reporteAnulaciones(int anio, int mes)
        {
            List<mAnulaciones> anulaciones = new List<mAnulaciones>();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {

                string query = "SELECT * FROM fnRptFactAnuEli(@anio, @mes)";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("anio", anio),
                    new SqlParameter("mes", mes)
                };

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar(); 

                if (dt != null && dt.Rows.Count > 0)
                {
                    anulaciones.Capacity = dt.Rows.Count;

                    foreach (DataRow dr in dt.Rows)
                    {
                        anulaciones.Add(new mAnulaciones
                        {
                            pedido = dr["peddscl"]?.ToString(),
                            caja = dr["cajanbr"]?.ToString(),
                            usuario = dr["usuanbr"]?.ToString(),
                            fecha = dr["ventfch"]?.ToString(),
                            factura = dr["ventnfc"]?.ToString(),
                            total = dr["venttot"] is double dTot ? dTot : Utils.Helper.StringToDouble(dr["venttot"]?.ToString()),
                            observacion = dr["ventobv"]?.ToString(),
                            estado = dr["ventstd"]?.ToString()
                        });
                    }
                }
            }

            return anulaciones;
        }

        public List<mAnulacionesFE> reporteAnulacionesFE(int anio, int mes, int dia)
        {
            List<mAnulacionesFE> anulaciones = new List<mAnulacionesFE>();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                string query = "SELECT * FROM fnRptFactAnuEliFE(@anio, @mes, @dia)";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("anio", anio),
                    new SqlParameter("mes", mes),
                    new SqlParameter("dia", dia)
                };

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    anulaciones.Capacity = dt.Rows.Count;

                    foreach (DataRow dr in dt.Rows)
                    {
                        anulaciones.Add(new mAnulacionesFE
                        {
                            pedido = dr["peddscl"]?.ToString(),
                            ventscl = dr["ventscl"]?.ToString(),
                            autosri = dr["autosri"]?.ToString(),
                            caja = dr["cajanbr"]?.ToString(),
                            usuario = dr["usuanbr"]?.ToString(),
                            fecha = dr["ventfch"]?.ToString(),
                            factura = dr["ventnfc"]?.ToString(),
                            total = dr["venttot"] is double dTot ? dTot : Utils.Helper.StringToDouble(dr["venttot"]?.ToString()),
                            observacion = dr["ventobv"]?.ToString(),
                            estado = dr["ventstd"]?.ToString()
                        });
                    }
                }
            }

            return anulaciones;
        }

        public string getMensajeFacturacion(string ventscl, string tipo, string estado)
        {
            string mensaje = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("ventscl", ventscl)
                };

                string query;

                if (estado == "L")
                {
                    string tabla = (tipo == "F") ? "tbPeddDVent" : "tbPeddDRete";
                    string campo = (tipo == "F") ? "ventscl" : "retescl";

                    query = $"SELECT ventobv AS mensaje FROM {tabla} WHERE {campo} = @ventscl AND ventstd IN ('A','E')";
                }
                else
                {
                    parametros.Add(new SqlParameter("estado", estado));

                    string tabla = (tipo == "F") ? "tbFactElect" : "tbReteElect";
                    query = $"SELECT mensaje FROM {tabla} WHERE ventscl = @ventscl AND etdofac = @estado";
                }

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    mensaje = dt.Rows[0]["mensaje"]?.ToString() ?? "";
                }
            }

            return mensaje;
        }

        public List<mDocSriAutorizados> getFacturaSriPorCliente(string cliente, string pedido, DateTime desde, DateTime hasta)
        {
            List<mDocSriAutorizados> resp = new List<mDocSriAutorizados>();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                desde = desde.Date;
                hasta = hasta.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("cliente", cliente),
                    new SqlParameter("pedido", pedido),
                    new SqlParameter("desde", desde),
                    new SqlParameter("hasta", hasta)
                };

                string query = @"SELECT * FROM fnBuscaFacturaSRIByCliente(
                                    @cliente, @pedido, @desde::TIMESTAMP WITHOUT TIME ZONE, @hasta::TIMESTAMP WITHOUT TIME ZONE
                                )";

                DataTable dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    resp.Capacity = dt.Rows.Count;

                    foreach (DataRow dr in dt.Rows)
                    {
                        mDocSriAutorizados fact = new mDocSriAutorizados
                        {
                            secPeddRet = dr["secprcd"]?.ToString(),
                            ventscl = dr["peddvent"]?.ToString(),
                            autosri = dr["autosri"]?.ToString(),
                            secuencial = dr["secuencial"]?.ToString(),
                            fecha = dr["fecha"]?.ToString(),
                            nidnid = dr["nidnid"]?.ToString(),
                            nombre = dr["nombre"]?.ToString(),
                            docref = dr["docref"]?.ToString(),

                            total = dr["total"] is double dTot ? dTot : (dr["total"] is decimal dDec ? (double)dDec : double.Parse(dr["total"].ToString()))
                        };

                        (fact.tipo, fact.tipoImg) = dr["tipo"]?.ToString() switch
                        {
                            "F" => ("Factura", "../../images/factura.png"),
                            "R" => ("Retención", "../../images/retencion.png"),
                            "C" => ("Nota de Crédito", "../../images/notaCredito.png"),
                            "D" => ("Nota de Débito", "../../images/notaDebito.png"),
                            _ => ("Desconocido", "")
                        };

                        resp.Add(fact);
                    }
                }
            }

            return resp;
        }

        public void undoSecuencialFactura()
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("tipo", System.Data.SqlDbType.VarChar) { Value = "F" }
                };

                string query = "SELECT * FROM fnUndoSecuencial(@tipo)";

                try
                {
                    conn.ejecutaQuery(query, parametros);
                }
                finally
                {
                    conn.cerrar();
                }
            }

        }

        public void getVentaAndNotaCreditoByVentScl(string codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                string query = @"SELECT  v.ventScl, v.usuacod, v.nidnid, v.venttip, v.ventcrr, v.ventmsa, v.ventlta, 
                             v.peddscl, v.ventsbt, v.ventdpc, v.ventdsc, v.ventiva, v.venttot, v.ventrcb, 
                             v.ventvto, c.nidNbr, c.niddir, c.nidtlf, v.ventpgd, v.ventfac, v.ventstd, v.ventstn, 
                             COALESCE(n.ntcrscl,'') AS ntcrscl, CASE WHEN n.ntcrscl IS NULL THEN false ELSE true END ntcrcrd 
                     FROM    tbPeddDPedd p 
                     JOIN    tbPeddDVent v ON v.peddscl = p.peddscl 
                     LEFT JOIN tbPeddDNcre n ON n.ventscl = v.ventscl 
                     JOIN    tbAdsiNid c ON c.nidnid = v.nidnid 
                     WHERE   v.ventscl = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow mainRow = dt.Rows[0];

                    this.ventscl = mainRow["ventScl"] is int vScl ? vScl : int.Parse(mainRow["ventScl"].ToString());
                    this.peddscl = mainRow["peddScl"]?.ToString();
                    this.ventlta = mainRow["ventLta"]?.ToString();
                    this.ventmsa = mainRow["ventMsa"]?.ToString();
                    this.empdnid = mainRow["usuaCod"]?.ToString();
                    this.venttip = mainRow["ventTip"]?.ToString();
                    this.ventcrr = mainRow["ventcrr"]?.ToString();

                    this.niddir = mainRow["nidDir"]?.ToString();
                    this.nidnid = mainRow["nidNid"]?.ToString();
                    this.nidnbr = mainRow["nidNbr"]?.ToString();
                    this.nidtlf = mainRow["nidTlf"]?.ToString();

                    if (this.venttip == "D")
                    {
                        this.cliDir = this.niddir;
                        this.cliNid = this.nidnid;
                        this.cliNbr = this.nidnbr;
                        this.cliTlf = this.nidtlf;
                    }

                    this.ventrcb = mainRow["ventRcb"] is double vRcb ? vRcb : Utils.Helper.StringToDouble(mainRow["ventRcb"]?.ToString());

                    double vuelto = mainRow["ventVto"] is double vVto ? vVto : Utils.Helper.StringToDouble(mainRow["ventVto"]?.ToString());
                    this.ventvto = vuelto < 0 ? 0 : vuelto;

                    this.ventdpc = mainRow["ventDpc"] is int vDpc ? vDpc : int.Parse(mainRow["ventDpc"].ToString());
                    this.ventsbt = mainRow["ventsbt"] is double vSbt ? vSbt : Utils.Helper.StringToDouble(mainRow["ventsbt"]?.ToString());
                    this.ventsbn = mainRow["ventStn"] is double vStn ? vStn : Utils.Helper.StringToDouble(mainRow["ventStn"]?.ToString());
                    this.ventdsc = mainRow["ventDsc"] is double vDsc ? vDsc : Utils.Helper.StringToDouble(mainRow["ventDsc"]?.ToString());
                    this.ventiva = mainRow["ventIva"] is double vIva ? vIva : Utils.Helper.StringToDouble(mainRow["ventIva"]?.ToString());
                    this.venttot = mainRow["ventTot"] is double vTot ? vTot : Utils.Helper.StringToDouble(mainRow["ventTot"]?.ToString());

                    this.ventpgd = mainRow["ventpgd"] is bool vPgd ? vPgd : Convert.ToBoolean(mainRow["ventpgd"]);
                    this.ventfac = mainRow["ventfac"] is bool vFac ? vFac : Convert.ToBoolean(mainRow["ventfac"]);
                    this.ventStd = mainRow["ventstd"]?.ToString();

                    this.ntcrcrd = mainRow["ntcrcrd"] is bool nCrd ? nCrd : Convert.ToBoolean(mainRow["ntcrcrd"]);
                    this.ntcrscl = mainRow["ntcrscl"]?.ToString();

                    this.detVenta.Clear();
                    this.detPago.Clear();

                    if (conn.conectar())
                    {
                        query = @"SELECT p.ventscl, p.itemcod, p.itemcnt, i.itemiva, p.itemprc, p.itemiva AS itempiva, p.itempto, 
                                         CASE itemSbg 
                                             WHEN TRUE THEN s.sgrpNbr 
                                             ELSE i.itemnbr 
                                         END itemnbr 
                                  FROM   tbPeddMVent p 
                                  INNER JOIN tbIvtrItem i on i.itemcod = p.itemcod 
                                  INNER JOIN tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod 
                                  WHERE  p.ventScl = @codigo";

                        List<SqlParameter> parametros1 = new List<SqlParameter> { new SqlParameter("codigo", codigo) };

                        DataTable dtr = conn.ejecutaQuery(query, parametros1);
                        conn.cerrar();

                        if (dtr != null && dtr.Rows.Count > 0)
                        {
                            this.detVenta.Capacity = dtr.Rows.Count;

                            foreach (DataRow dr in dtr.Rows)
                            {
                                this.detVenta.Add(new VentaDetalle
                                {
                                    itemcod = dr["itemCod"]?.ToString(),
                                    itemcnt = dr["itemcnt"] is int iCnt ? iCnt : int.Parse(dr["itemcnt"].ToString()),

                                    usaiva = dr["itemiva"] is bool uIva ? uIva : Convert.ToBoolean(dr["itemiva"]),

                                    itemiva = dr["itempiva"] is double iIva ? iIva : Utils.Helper.StringToDouble(dr["itempiva"]?.ToString()),
                                    itemprc = dr["itemprc"] is double iPrc ? iPrc : Utils.Helper.StringToDouble(dr["itemprc"]?.ToString()),
                                    itempto = dr["itempto"] is double iPto ? iPto : Utils.Helper.StringToDouble(dr["itempto"]?.ToString()),
                                    itemdto = 0,
                                    nbrprd = dr["itemNbr"]?.ToString(),
                                    itemsel = true,
                                    itemEnable = true
                                });
                            }
                        }
                    }
                }
                else
                {
                    dt = new DataTable("Listado");
                }
            }

        }
    }

    public class VentasMarket : Ventas
    {
        public string ltaPedd { get; set; }
        public string msaPedd { get; set; }
        public string tipPedd { get; set; }

        public VentasMarket(string strConnection) : base(strConnection)
        {
        }
    }
}

using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Pedido : EntidadBase
    {        
        public int sclPedd { get; set; } = -1;
        public string codEmpl { get; set; } = "0001";
        public string celEmpl { get; set; } = "";
        public string empdNbr { get; set; } = "";
        public string msaPedd { get; set; }
        public string ltaPedd { get; set; } = "A";
        public string tipPedd { get; set; } = "L";
        public string obvPedd { get; set; } = "";
        public string codCrri { get; set; } = "0000"; // codigo del carrier seleccionado
        public string crrCodi { get; set; } = ""; // cpodigo provisto por el carrier
        public string crrObsv { get; set; } = ""; // observación a ingresar referente al carrier
        public string nbrCrri { get; set; } = ""; // Nombre del carrier seleccionado

        public string cliNid { get; set; }
        public string cliNbr { get; set; }
        public string cliTlf { get; set; }
        public string cliDir { get; set; }
        public string cliRef { get; set; }

        public Double recibido { get; set; }
        public Double vuelto { get; set; }

        public List<PedidoDetalle> detPedd { get; set; } = new List<PedidoDetalle>();

        public Pedido(string strConnection) : base(strConnection)
        {
        }

        public String getLetraPedido(String codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            String letra = "";

            if (conn.conectar())
            {
                String query = "SELECT * FROM tbPeddDPedd WHERE peddScl = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    letra = dt.Rows[0]["peddLta"].ToString();
                }

                conn.cerrar();
            }

            return letra;
        }

        public void getPedido(String codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                String query = "SELECT * FROM tbPeddDPedd WHERE peddScl = @codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    this.sclPedd = int.Parse(dt.Rows[0]["peddScl"].ToString());
                    this.ltaPedd = dt.Rows[0]["peddLta"].ToString();
                    this.msaPedd = dt.Rows[0]["peddMsa"].ToString();
                    this.codEmpl = dt.Rows[0]["usuaCod"].ToString();
                    this.tipPedd = dt.Rows[0]["peddTip"].ToString();

                    this.detPedd.Clear();

                    if (conn.conectar())
                    {
                        query = "SELECT p.peddscl, p.itemcod, p.itemcnt, p.itemiva, p.itemprc, p.itempiv, p.itempto, i.itemnbr, i.itemdelcoc, i.itemgiva " +
                                "FROM   tbPeddMPedd p INNER JOIN tbIvtrItem i on i.itemcod = p.itemcod " +
                                "WHERE  p.peddScl = @codigo AND p.peddAtv = true";

                        List<SqlParameter> parametros1 = new List<SqlParameter>();

                        parametros1.Add(new SqlParameter("codigo", codigo));

                        DataTable dtr = conn.ejecutaQuery(query, parametros);

                        foreach(DataRow dr in dtr.Rows)
                        {
                            PedidoDetalle detalle = new PedidoDetalle();

                            detalle.codprd = dr["itemCod"].ToString();
                            detalle.cntprd = int.Parse(dr["itemcnt"].ToString());
                            detalle.usaiva = (Boolean)dr["itemiva"];
                            detalle.ivaprd = Utils.Helper.StringToDouble(dr["itempiv"].ToString());
                            detalle.prcprd = Utils.Helper.StringToDouble(dr["itemprc"].ToString());
                            detalle.ptoprd = Utils.Helper.StringToDouble(dr["itempto"].ToString());
                            detalle.nbrprd = dr["itemNbr"].ToString();
                            detalle.itemdelcoc = (Boolean)dr["itemdelcoc"];
                            detalle.itemgiva = (Boolean)dr["itemgiva"];

                            this.detPedd.Add(detalle);
                        }
                    }
                }
                else
                {
                    dt = new DataTable("Listado");
                }

                conn.cerrar();
            }
        }
        
        public DataTable getPedidosVigentes()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                //String query = "SELECT * FROM tbPeddDPedd WHERE peddAtv=true AND peddEli=false AND peddFec=CURRENT_DATE AND peddHfn is NULL ORDER BY peddscl";
                string query = "SELECT  * " +
                               "FROM    tbPeddDPedd " +
                               "WHERE   peddAtv = true AND peddEli = false " +
                               "        AND peddFec >= CASE " +
                               "                        WHEN CURRENT_TIME >= '00:00:00'::TIME WITHOUT TIME ZONE " + 
                               "                            THEN CURRENT_DATE - interval '16hours' " +
                               "                        ELSE CURRENT_DATE " +
                               "                      END " +
                               "        AND peddFec <= CASE " +
                               "                        WHEN CURRENT_TIME >= '00:00:00'::TIME WITHOUT TIME ZONE " +
                               "                            THEN CURRENT_DATE + interval '4hours' " +
                               "                        ELSE(CURRENT_DATE::CHARACTER VARYING || '23:59:59')::TIMESTAMP WITHOUT TIME ZONE " +
                               "                        END " +
                               "        AND peddHfn IS NULL";
                
                dt = conn.ejecutaQuery(query);
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

        public DataTable getPedidosFinalizados()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                String query = "SELECT * " +
                               "FROM    tbPeddDPedd p " +
                               "WHERE   p.peddAtv = false " +
                               "        AND p.peddEli = false " + 
                               "        AND p.peddFec = CURRENT_DATE " + 
                               "        AND p.peddHfn is NOT NULL " +
                               "        AND p.peddscl NOT IN (SELECT v.peddscl FROM tbPeddDVent v WHERE v.ventstd NOT IN ('A'))  " +
                               "        AND p.peddscl NOT IN (SELECT g.peddscl FROM tbPeddDGift g)" +
                               "ORDER BY p.peddscl";


                dt = conn.ejecutaQuery(query);
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

        public DataTable getMotivosEliminacion()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                String query = "SELECT mtvoScl AS codigo, mtvoNbr AS descripcion FROM tbAdsiMtvos WHERE mtvoTip = 'P' AND mtvoAtv=TRUE";


                dt = conn.ejecutaQuery(query);
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

        public Respuesta EliminacionPedido(String codigo, String mensaje)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("secuencial", codigo));
                parametros.Add(new SqlParameter("mensaje", mensaje));

                String query = "UPDATE tbPeddDPedd SET peddObv=@mensaje, peddEli=true WHERE peddScl=@secuencial";
                var result = conn.updateQuery(query, parametros);

                res.resultado = result.resultado;

                if (res.resultado)
                {
                    res.mensaje = "Pedido eliminado correctamente";
                }else
                {
                    res.mensaje = "Pedido no pudo ser eliminado" + System.Environment.NewLine + result.detalle;
                }
            }

            return res;
        }

        public Respuesta finalizar(String codigo)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", "F"));
                parametros.Add(new SqlParameter("data", codigo));
                parametros.Add(new SqlParameter("detalle", "[]"));
                parametros.Add(new SqlParameter("vpedido", codigo));
                parametros.Add(new SqlParameter("reduceinv", false));

                String query = "SELECT * FROM fnPedidos(@accion, @data::varchar, @detalle::varchar, @vpedido::character varying, @reduceinv::boolean)";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
            }

            return res;
        }

        public Respuesta revertir(String codigo)
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", "U"));
                parametros.Add(new SqlParameter("data", codigo));
                parametros.Add(new SqlParameter("detalle", "[]"));
                parametros.Add(new SqlParameter("vpedido", codigo));
                parametros.Add(new SqlParameter("reduceinv", false));

                String query = "SELECT * FROM fnPedidos(@accion, @data::varchar, @detalle::varchar, @vpedido::character varying, @reduceinv::boolean)";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
            }

            return res;
        }

        public Respuesta grabar()
        {
            Respuesta res = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", this.sclPedd != -1 ? "M" : "N"));

                if (this.sclPedd == -1)
                {
                    parametros.Add(new SqlParameter("data",
                        JsonConvert.SerializeObject(
                            new cPedido
                            {
                                ltapedd = this.ltaPedd,
                                msapedd = this.msaPedd,
                                tippedd = this.tipPedd,
                                codcrri = this.codCrri,
                                usuacod = this.codEmpl,
                                clienid = this.cliNid,
                                clienbr = this.cliNbr,
                                cliedir = this.cliDir,
                                clieref = this.cliRef,
                                clietlf = this.cliTlf,
                                crricod = this.crrCodi,
                                crriobv = this.crrObsv
                            }
                        )
                    ));
                }
                else
                {
                    parametros.Add(new SqlParameter("data",
                        JsonConvert.SerializeObject(
                            new mPedido
                            {
                                sclpedd = this.sclPedd.ToString().PadLeft(10, '0'),
                                ltapedd = this.ltaPedd,
                                msapedd = this.msaPedd,
                                tippedd = this.tipPedd,
                                usuacod = this.codEmpl,
                                obvpedd = this.obvPedd,
                                codcrri = this.codCrri,
                            }
                        )
                    ));
                }

                parametros.Add(new SqlParameter("detalle", JsonConvert.SerializeObject(this.detPedd)));
                if (sclPedd == -1)
                {
                    parametros.Add(new SqlParameter("vpedido", ""));
                    parametros.Add(new SqlParameter("reduceInv", false));
                }
                else
                {
                    parametros.Add(new SqlParameter("vpedido", sclPedd.ToString().PadLeft(10,'0')));
                    parametros.Add(new SqlParameter("reduceInv", true));
                }

                String query = "SELECT * FROM fnPedidos(@accion::varchar, @data::varchar, @detalle::varchar, @vpedido::varchar, @reduceInv)";

                DataTable dt = conn.ejecutaQuery(query, parametros);
               
                if(dt != null)
                {
                    res.resultado= int.Parse(dt.Rows[0]["codigo"].ToString())==1?true:false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }

                conn.cerrar();
            }

            return res;
        }

        public void grabarPedidoCocina(string peddscl, List<ProdPedd> pedido, string usuacod)
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("vPeddScl", peddscl));
                parametros.Add(new SqlParameter("data", JsonConvert.SerializeObject(pedido)));
                parametros.Add(new SqlParameter("vUsuaCod", usuacod));

                string query = "SELECT * FROM fnCocnaSav(@vPeddScl, @data, @vUsuaCod);";
                
                DataTable dt = conn.ejecutaQuery(query, parametros);

                conn.cerrar();
            }
        }

        public List<ProdPedd> getPedidoCocina(string peddscl)
        {
            List<ProdPedd> pedido = null;

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("vPeddScl", peddscl));
                
                string query = "SELECT * FROM fnLstCocna(@vPeddScl);";

                DataTable dt = conn.ejecutaQuery(query, parametros);


                if (dt != null)
                {
                    var respuesta = dt.Rows[0]["fnlstcocna"].ToString();
                    pedido = JsonConvert.DeserializeObject<List<ProdPedd>>(respuesta);
                }

                conn.cerrar();
            }

            return pedido;
        }

        public List<rPedido> reportePedidos(String inicio, String fin)
        {
            List<rPedido> pedidos = new List<rPedido>();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "";

                query = "select * from fnRptPedidos(@inicio, @fin)	";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("inicio", Convert.ToDateTime(inicio).ToString("yyyy-MM-dd")));
                parametros.Add(new SqlParameter("fin", Convert.ToDateTime(fin).ToString("yyyy-MM-dd")));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        pedidos.Add(new rPedido()
                        {
                            fecha = Convert.ToDateTime(dr["fecha"]).ToString("yyyy-MM-dd"),
                            tipo = dr["tipo"].ToString(),
                            lcnt = int.Parse(dr["lcnt"].ToString()),
                            lpago = Utils.Helper.StringToDouble(dr["lpago"].ToString()),
                            dcnt = int.Parse(dr["dcnt"].ToString()),
                            dpago = Utils.Helper.StringToDouble(dr["dpago"].ToString()),
                            ccnt = int.Parse(dr["ccnt"].ToString()),
                            cpago = Utils.Helper.StringToDouble(dr["cpago"].ToString()),
                            vcnt = int.Parse(dr["vcnt"].ToString()),
                            vpago = Utils.Helper.StringToDouble(dr["vpago"].ToString()),
                            rcnt = int.Parse(dr["rcnt"].ToString()),
                            rpago = Utils.Helper.StringToDouble(dr["rpago"].ToString()),
                            cantidad = int.Parse(dr["ctotal"].ToString()),
                            total = Utils.Helper.StringToDouble(dr["ptotal"].ToString())
                        });
                    }
                }
            }

            return pedidos;
        }
    }
}

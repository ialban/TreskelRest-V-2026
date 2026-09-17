using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Entidades
{
    public class MovimientoCaja : EntidadBase
    {
        public String tipo { get; set; } = "I";
        public String secuencial { get; set; } = "";
        public String caja { get; set; }
        public String detalle { get; set; }
        public DateTime fecha { get; set; }
        public String valor { get; set; }
        public String usuarioCrea { get; set; }
        public DateTime fechaCrea { get; set; }
        public String usuarioElimina { get; set; }
        public DateTime fechaElimina { get; set; }

        public MovimientoCaja(string strConnection) : base(strConnection)
        {

        }

        public String getFechaBase()
        {
            String fecha = "";
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select to_char(current_time::time, 'HH24:MI') as hora from current_time ";
                DataTable dt = conn.ejecutaQuery(query);
                                
                if (dt != null)
                {
                    fecha = dt.Rows[0]["hora"].ToString();
                }

                conn.cerrar();
            }

            return fecha;
        } 

        public CierreCaja BuscaInicioCierre(string fecha, string caja)
        {
            DbConnection conn = new DbConnection(strConnection);

            CierreCaja respuesta = new CierreCaja();
            respuesta.caja = caja;

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("fecha", fecha));
                parametros.Add(new SqlParameter("caja", caja));

                string query = "select * from fnBuscaInicioCierre(@fecha, @caja)";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        respuesta.secuencial = dr["ctrlsec"].ToString();
                        respuesta.hora = dr["ctrlhin"].ToString();
                        respuesta.horaCiere = dr["ctrlhfn"] != null ? (dr["ctrlhfn"].ToString() != "" ? dr["ctrlhfn"].ToString() : null): null;
                        respuesta.fecha = dr["ctrlfchini"].ToString();
                        respuesta.inicio = Utils.Helper.StringToDouble(dr["ctrlvin"].ToString());
                        respuesta.cierre = Utils.Helper.StringToDouble(dr["ctrlvcr"].ToString());
                        respuesta.efectivo = Utils.Helper.StringToDouble(dr["ctrlefc"].ToString());
                        respuesta.tarjeta = Utils.Helper.StringToDouble(dr["ctrltrj"].ToString());
                        respuesta.cheque = Utils.Helper.StringToDouble(dr["ctrlchq"].ToString());
                        respuesta.transfer = Utils.Helper.StringToDouble(dr["ctrltrf"].ToString());
                        respuesta.delivery = Utils.Helper.StringToDouble(dr["ctrldlv"].ToString());
                    }
                }
            }
            return respuesta;
        }

        public CierreCaja ResumenCierreCaja(string fecha, string caja)
        {
            DbConnection conn = new DbConnection(strConnection);
            CierreCaja respuesta = new CierreCaja();

            respuesta.caja = caja;
            respuesta.fecha = fecha;

            List<ResultControl> res = new List<ResultControl>();

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("fecha", fecha));
                parametros.Add(new SqlParameter("caja", caja));

                string query = "select * from fnlstcontrolCompVent(@fecha, @caja,'')";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        res.Add(new ResultControl
                        {
                            origen = dr["origen"].ToString(),
                            hora = dr["hora"].ToString(),
                            secuencial = dr["ventscl"].ToString(),
                            localidad = dr["venttip"].ToString(),
                            valor = Utils.Helper.StringToDouble(dr["venttot"].ToString()),
                            diferencia = Utils.Helper.StringToDouble(dr["ventdif"].ToString()),
                            tipo = dr["fpgotip"].ToString()
                        });
                    }

                    respuesta.secuencial = res.Where(x => x.origen == "CC").Select(o => o.secuencial).FirstOrDefault();
                    respuesta.inicio = res.Where(x => x.origen == "IC").Select(o => o.valor).FirstOrDefault();
                    respuesta.hora = res.Where(x => x.origen == "CC").Select(o => o.hora).FirstOrDefault();
                    respuesta.cierre = res.Where(x => x.origen == "CC").Select(o => o.valor).FirstOrDefault();
                    respuesta.efectivo = res.Where(x => x.origen == "CE").Select(o => o.valor).FirstOrDefault();
                    respuesta.tarjeta = res.Where(x => x.origen == "CT").Select(o => o.valor).FirstOrDefault();
                    respuesta.cheque = res.Where(x => x.origen == "CQ").Select(o => o.valor).FirstOrDefault();
                    respuesta.transfer = res.Where(x => x.origen == "CF").Select(o => o.valor).FirstOrDefault();
                    respuesta.delivery = res.Where(x => x.origen == "DL").Select(o => o.valor).FirstOrDefault();

                    respuesta.eliminado = res.Where(x => x.origen == "EL").Select(o => o.valor).Sum();
                    respuesta.anulado = res.Where(x => x.origen == "AN").Select(o => o.valor).Sum();

                    respuesta.vales = new Vales();
                    respuesta.vales.valorIngreso = res.Where(x => x.origen == "I").Select(o => o.valor).Sum();
                    respuesta.vales.valorEgreso = res.Where(x => x.origen == "E").Select(o => o.valor).Sum();


                    respuesta.compras = new ComprasConsolidada();
                    respuesta.compras.valor = res.Where(x => x.origen == "C").Select(o => o.valor).Sum();

                    respuesta.salarios = res.Where(x => x.origen == "PS").Select(o => o.valor).Sum();

                    respuesta.ventas = new VentasConsolidada();
                    respuesta.ventas.locales = res.Where(x => x.origen == "V" && x.localidad == "LOCAL").Select(o => o.diferencia).Sum();
                    respuesta.ventas.domicilio = res.Where(x => x.origen == "V" && x.localidad == "DOMICILIO").Select(o => o.diferencia).Sum();
                    respuesta.ventas.delivery = res.Where(x => x.origen == "V" && x.localidad == "DELIVERY").Select(o => o.diferencia).Sum();
                    respuesta.ventas.llevar = res.Where(x => x.origen == "V" && x.localidad == "LLEVAR").Select(o => o.diferencia).Sum();
                    respuesta.ventas.retirar = res.Where(x => x.origen == "V" && x.localidad == "RETIRAR").Select(o => o.diferencia).Sum();

                    respuesta.ventas.total = res.Where(x => x.origen == "V").Select(o => o.diferencia).Sum();
                    respuesta.ventas.pago = new List<FormaPago>();

                    respuesta.ventas.pago = res.Where(x => x.origen == "V")
                        .GroupBy(y => y.tipo)
                        .Select(o => new FormaPago { 
                            tipo = o.First().tipo, 
                            valor = o.Sum(c => c.diferencia)
                        })
                        .ToList();

                }
            }

            return respuesta;
        }

        public Respuesta grabar(string accion)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.resultado = false;
            respuesta.mensaje = "El " + (this.tipo.Equals('I') ? "Ingreso" : "Egreso") + " no pudo grabarse correctamente.";

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("tipo", tipo));
                parametros.Add(new SqlParameter("fecha", fecha));
                parametros.Add(new SqlParameter("caja", caja));
                parametros.Add(new SqlParameter("secuencial", secuencial));
                parametros.Add(new SqlParameter("detalle", detalle));
                parametros.Add(new SqlParameter("valor", valor));
                parametros.Add(new SqlParameter("usuario", usuarioCrea));

                String query = "SELECT * FROM fnMovimientoCaja(@accion, @tipo, @caja, @secuencial, @fecha, @detalle, @valor, @usuario)";
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

        public DataTable getMotivos(String tipo)
        {
            String query = "SELECT mtvoScl AS codigo, mtvoNbr AS descripcion FROM tbAdsiMtvos WHERE mtvoTip = @tipo";
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("tipo", tipo));

                dt = conn.ejecutaQuery(query, parametros);

                conn.cerrar();
            }

            return dt;
        }

        public mControl ControlCaja(String fecha, string caja)
        {
            mControl control = new mControl();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select * from fnLstControl(@fecha, @caja)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("fecha", fecha));
                parametros.Add(new SqlParameter("caja", caja));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    control.secuencial = dt.Rows[0]["secuencial"].ToString();   
                    control.ingresos = Utils.Helper.StringToDouble(dt.Rows[0]["ingresos"].ToString());
                    control.egresos = Utils.Helper.StringToDouble(dt.Rows[0]["egresos"].ToString());
                    control.salarios = Utils.Helper.StringToDouble(dt.Rows[0]["salarios"].ToString());
                    control.compras = Utils.Helper.StringToDouble(dt.Rows[0]["compras"].ToString());
                    control.ventas = Utils.Helper.StringToDouble(dt.Rows[0]["ventas"].ToString());
                    control.inicio = Utils.Helper.StringToDouble(dt.Rows[0]["inicio"].ToString());
                    control.cierre = Utils.Helper.StringToDouble(dt.Rows[0]["cierre"].ToString());
                    control.efectivo = Utils.Helper.StringToDouble(dt.Rows[0]["efectivo"].ToString());
                    control.tarjeta = Utils.Helper.StringToDouble(dt.Rows[0]["tarjeta"].ToString());
                    control.cheque = Utils.Helper.StringToDouble(dt.Rows[0]["cheque"].ToString());
                    control.transfer = Utils.Helper.StringToDouble(dt.Rows[0]["transfer"].ToString());
                    control.delivery = Utils.Helper.StringToDouble(dt.Rows[0]["delivery"].ToString());
                }

                conn.cerrar();
            }

            return control;
        }

        public Respuesta InicioCierre(String fecha, string hora, string valor, string sistema, string diferencia,
            string efectivo, string tarjeta, string cheque, string transfer, string delivery)
        {
            Respuesta respuesta = new Respuesta();

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String query = "select * from fnControlCaja(@tipo, @caja, @secuencial, @fecha, @hora, @valor, @sistema, @diferencia, @efectivo, @tarjeta, @cheque, @transfer, @delivery)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("tipo", this.tipo));
                parametros.Add(new SqlParameter("caja", this.caja));
                parametros.Add(new SqlParameter("secuencial", this.secuencial));
                parametros.Add(new SqlParameter("fecha", fecha));
                parametros.Add(new SqlParameter("hora", hora));
                parametros.Add(new SqlParameter("valor", valor));
                parametros.Add(new SqlParameter("sistema", sistema));
                parametros.Add(new SqlParameter("diferencia", diferencia));
                parametros.Add(new SqlParameter("efectivo", efectivo));
                parametros.Add(new SqlParameter("tarjeta", tarjeta));
                parametros.Add(new SqlParameter("cheque", cheque));
                parametros.Add(new SqlParameter("transfer", transfer));
                parametros.Add(new SqlParameter("delivery", delivery));

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

        public List<mControlCaja> reporteCaja(string opcion, int anio, int mes, int dia, Boolean detalle)
        {
            List<mControlCaja> movs = null;
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
               
                String query = "";
             
                if (opcion == "M") 
                    query = "select * from fnRptControlCaja(@anio, @mes, @detalle)";

                if (opcion == "D")
                    query = "select * from fnRptControlCajaDiario(@anio, @mes, @dia)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("anio", anio)); 
                parametros.Add(new SqlParameter("mes", mes));
                if (opcion == "M")
                    parametros.Add(new SqlParameter("detalle", detalle));
                if (opcion == "D")
                    parametros.Add(new SqlParameter("dia", dia));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    movs = new List<mControlCaja>();

                    foreach(DataRow dr in dt.Rows)
                    {
                        if(opcion == "D")
                        {
                            movs.Add(new mControlCaja()
                            {
                                fecha = ((System.DateOnly)dr["fecha"]).ToString("yyyy-MM-dd"),
                                caja = dr["caja"].ToString(),
                                detalle = dr["resumen"].ToString(),
                                inicio = Utils.Helper.StringToDouble(dr["inicio"].ToString()),
                                ingreso = Utils.Helper.StringToDouble(dr["ingreso"].ToString()),
                                egreso = Utils.Helper.StringToDouble(dr["egreso"].ToString()),
                                cierre = Utils.Helper.StringToDouble(dr["cierre"].ToString()),
                                sistema = Utils.Helper.StringToDouble(dr["sistema"].ToString()),
                                diferencia = Utils.Helper.StringToDouble(dr["diferencia"].ToString())
                            });
                        }
                        else
                        {
                            movs.Add(new mControlCaja()
                            {
                                fecha = ((System.DateOnly)dr["fecha"]).ToString("yyyy-MM-dd"),
                                caja = dr["caja"].ToString(),
                                inicio = Utils.Helper.StringToDouble(dr["inicio"].ToString()),
                                ingreso = Utils.Helper.StringToDouble(dr["ingreso"].ToString()),
                                egreso = Utils.Helper.StringToDouble(dr["egreso"].ToString()),
                                cierre = Utils.Helper.StringToDouble(dr["cierre"].ToString()),
                                sistema = Utils.Helper.StringToDouble(dr["sistema"].ToString()),
                                diferencia = Utils.Helper.StringToDouble(dr["diferencia"].ToString())
                            });
                        }
                        
                    }
                }

                conn.cerrar();
            }

            return movs;
        }

        public List<mControlCaja> reporteCajaDiario(int anio, int mes, int dia)
        {
            List<mControlCaja> movs = null;
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                String inicio = anio + "-" + mes.ToString().PadLeft(2, '0') + "-01";
                String fin = anio + "-" + (mes + 1).ToString().PadLeft(2, '0') + "-01";

                String query = "";

                query = "select * from fnRptControlCaja(@anio, @mes, @detalle)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("anio", anio)); 
                parametros.Add(new SqlParameter("mes", mes));
                parametros.Add(new SqlParameter("detalle", detalle));

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    movs = new List<mControlCaja>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        movs.Add(new mControlCaja()
                        {
                            fecha = Convert.ToDateTime(dr["fecha"]).ToString("yyyy-MM-dd"),
                            caja = dr["caja"].ToString(),
                            inicio = Utils.Helper.StringToDouble(dr["inicio"].ToString()),
                            ingreso = Utils.Helper.StringToDouble(dr["ingreso"].ToString()),
                            egreso = Utils.Helper.StringToDouble(dr["egreso"].ToString()),
                            cierre = Utils.Helper.StringToDouble(dr["cierre"].ToString()),
                            sistema = Utils.Helper.StringToDouble(dr["sistema"].ToString()),
                            diferencia = Utils.Helper.StringToDouble(dr["diferencia"].ToString())
                        });
                    }
                }

                conn.cerrar();
            }

            return movs;
        }
    }
}

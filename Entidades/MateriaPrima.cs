using AccesoDatos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Entidades
{
    public class MateriaPrima : EntidadBase
    {
        public string prdCod { get; set; }
        public string prdNbr { get; set; } = "";
        public string prdUnd { get; set; } = "U";
        public string prdGrp { get; set; } = "";
        public Byte[] prdImg { get; set; } = null;
        public bool prdFnl { get; set; } = false;
        public bool prdAtv { get; set; } = true;
        public string prdBcd { get; set; } = "";
        public bool prdGiv { get; set; } = true;

        public MateriaPrima(string strConnection) : base(strConnection)
        {
        }

        public Respuesta grabar(string accion)
        {
            Respuesta res = new Respuesta()
            {
                resultado = false,
                mensaje = "No se pudo realizar la transacción",
                mensajeError = "No se pudo realizar la transacción",
            };

            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("pprdcod", this.prdCod));
                parametros.Add(new SqlParameter("pprdnbr", this.prdNbr));
                parametros.Add(new SqlParameter("pprdund", this.prdUnd));
                parametros.Add(new SqlParameter("pprdfnl", this.prdFnl));
                parametros.Add(new SqlParameter("pprdgrp", this.prdGrp));
                parametros.Add(new SqlParameter("pprdgiv", this.prdGiv));
                parametros.Add(new SqlParameter("pprdatv", this.prdAtv));
                parametros.Add(new SqlParameter("pprdbcd", this.prdBcd));
                

                if (this.prdImg != null)
                {
                    //parametros.Add(new SqlParameter("pprdImg", this.prdImg));
                    parametros.Add(new SqlParameter("pprdImg", SqlDbType.Binary) { Value = this.prdImg });
                }
                else
                {
                    parametros.Add(new SqlParameter("pprdImg", SqlDbType.Binary) { Value = DBNull.Value});
                }


                string query = "SELECT * " +
                               "FROM fnCreaMateriaPrima(" +
                               "@accion, @pprdcod, @pprdnbr, @pprdund, @pprdfnl, @pprdgrp, " +
                               "@pprdgiv, @pprdatv, @pprdbcd, @pprdimg);";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    res.resultado = dr["codigo"].ToString() == "1" ? true : false;
                    res.mensaje = dr["descripcion"].ToString();

                    return res;
                }

                return res;
            }

            return res;
        }

        public DataTable buscar(String nombre, string grupo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstPrd(@prdNbr, @prdUnd) ORDER BY codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("prdNbr", '%' + nombre + '%'));
                parametros.Add(new SqlParameter("prdUnd", grupo));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";
                }
                else
                {
                    dt = new DataTable("Listado");
                }

                conn.cerrar();
            }

            return dt;
        }

        public Boolean getById()
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection);
            if (conn.conectar())
            {
                String query = "SELECT * FROM tbIvtrPrd WHERE prdCod = @prdCod";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("prdCod", this.prdCod));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                dt.TableName = "Listado";
                conn.cerrar();

                if (dt.Rows.Count > 0)
                {
                    this.prdCod = dt.Rows[0]["prdCod"].ToString();
                    this.prdNbr = dt.Rows[0]["prdNbr"].ToString();
                    this.prdUnd = dt.Rows[0]["prdUnd"].ToString();
                    this.prdFnl = (Boolean) dt.Rows[0]["prdFnl"];
                    this.prdGrp = dt.Rows[0]["prdGrp"].ToString();
                    this.prdAtv = (Boolean) dt.Rows[0]["prdAtv"];
                    this.prdBcd = dt.Rows[0]["prdBcd"].ToString();

                    if (dt.Rows[0]["prdImg"].Equals(DBNull.Value))
                    {
                        this.prdImg = null;
                    }else
                    {
                        this.prdImg = (byte[])dt.Rows[0]["prdImg"];
                    }                    

                    res = true;
                }

                conn.cerrar();
            }

            return res;
        }

        public DataTable listarProductosFinales()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                String query = "SELECT prdCod AS codigo, prdNbr as descripcion FROM tbIvtrPrd WHERE prdFnl = true ORDER BY prdNbr";

                List<SqlParameter> parametros = new List<SqlParameter>();

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

                conn.cerrar();
            }

            return dt;
        }

        public DataTable listarProductos()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            if (conn.conectar())
            {
                String query = "SELECT prdCod AS codigo, prdNbr as descripcion FROM tbIvtrPrd ORDER BY prdNbr";

                List<SqlParameter> parametros = new List<SqlParameter>();

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

                conn.cerrar();
            }

            return dt;
        }

        public DataTable listarProductos(String grupo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "";
                if (grupo.Equals(""))
                {
                    query = "SELECT prdCod AS codigo, prdNbr as descripcion FROM tbIvtrPrd ORDER BY prdNbr";
                    dt = conn.ejecutaQuery(query);
                }
                else
                {
                    query = "SELECT prdCod AS codigo, prdNbr as descripcion FROM tbIvtrPrd WHERE prdGrp=@prdGrp ORDER BY prdNbr";
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    parametros.Add(new SqlParameter("prdGrp", grupo));

                    dt = conn.ejecutaQuery(query, parametros);
                }                
                
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

        public Respuesta ajustarInventario(String codProd, String undProd, Double cntProd, String tipo, String motivo)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                DataTable dt = null;
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codProd", codProd));
                parametros.Add(new SqlParameter("undProd", undProd));
                parametros.Add(new SqlParameter("cntProd", cntProd));
                parametros.Add(new SqlParameter("tipoAju", tipo));

                String query = "SELECT * FROM fnAjusInv(@codProd, @undProd, @cntProd, @tipoAju);";
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
    }
}

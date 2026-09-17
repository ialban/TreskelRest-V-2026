using AccesoDatos;
using Entidades.modelos;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    public class Items : EntidadBase
    {
        public String itemCod { get; set; }
        public String grpCod { get; set; }
        public String sgrpCod { get; set; }
        public String itemNbr { get; set; }
        public Boolean itemIva { get; set; } = false;
        public Boolean itemGravaIva { get; set; } = true;
        public Double itemPrc { get; set; } = 0;
        public string itemUnd { get; set; } = "UND";
        public string itemBcde { get; set; } = null; // Barcode
        public Double itemPiv { get; set; } = 0;
        public Byte[] itemImg { get; set; } = null;
        public String prdCod { get; set; } = "";
        public Boolean itemCoc { get; set; } = true;
        public Boolean itemParr { get; set; } = false;
        public Boolean itemBar { get; set; } = false;
        public Boolean itemDelCoc { get; set; } = false;
        public Boolean itemSbg { get; set; } = false;
        public Boolean itemAtv { get; set; } = true;
        public int itemPara { get; set; } = 0;
        public string itemals { get; set; } = "";

        public List<Receta> receta { get; set; } = new List<Receta>();

        public Items(string strConnection) : base(strConnection)
        {
        }

        public Respuesta grabar(List<mItems> itms)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("vitems", JsonConvert.SerializeObject(itms).ToString()));
                parametros.Add(new SqlParameter("vprods", ""));

                string query = "SELECT * FROM fnImportar(@vitems, @vprods)";

                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }
                else
                {
                    res.resultado = false;
                    res.mensaje = "No fue Importar los datos.";
                }

                conn.cerrar();
            }

            return res;
        }

        public Respuesta grabar(String accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                /*
                 obtenemos el siguente secuencial
                */
                if ((itemCod == "0000" || itemCod == "") && accion == "N")
                {
                    string query1 = "SELECT LPAD((COALESCE(MAX(itemcod::int),0) + 1)::TEXT,4,'0') AS codigo FROM tbIvtrItem";

                    DataTable dtc = conn.ejecutaQuery(query1);
                    if (dtc != null)
                    {
                        itemCod = dtc.Rows[0]["codigo"].ToString();
                    }
                }

                conn.cerrar();
            }
            
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("itemCod", this.itemCod));
                parametros.Add(new SqlParameter("sgrpCod", this.sgrpCod));
                parametros.Add(new SqlParameter("itemNbr", this.itemNbr));
                parametros.Add(new SqlParameter("itemGIva", this.itemGravaIva));
                parametros.Add(new SqlParameter("itemIva", this.itemIva));
                parametros.Add(new SqlParameter("itemAtv", this.itemAtv));
                parametros.Add(new SqlParameter("itemPrc", this.itemPrc));
                parametros.Add(new SqlParameter("itemPiv", this.itemPiv));
                if (this.itemImg != null)
                {
                    parametros.Add(new SqlParameter("itemImg", this.itemImg));
                }

                if (!this.prdCod.Equals(""))
                {
                    parametros.Add(new SqlParameter("prdCod", this.prdCod));
                }
                parametros.Add(new SqlParameter("itemCoc", this.itemCoc));
                parametros.Add(new SqlParameter("itemUnd", this.itemUnd));
                parametros.Add(new SqlParameter("itemBcde", this.itemBcde));
                parametros.Add(new SqlParameter("itemParr", this.itemParr));
                parametros.Add(new SqlParameter("itemBar", this.itemBar));
                parametros.Add(new SqlParameter("itemDelCoc", this.itemDelCoc));
                parametros.Add(new SqlParameter("itemSbg", this.itemSbg));
                parametros.Add(new SqlParameter("itemals", this.itemals));
                switch (this.itemPara)
                {
                    case 0:
                        parametros.Add(new SqlParameter("itemPara", "T"));
                        break;
                    case 1:
                        parametros.Add(new SqlParameter("itemPara", "R"));
                        break;
                    case 2:
                        parametros.Add(new SqlParameter("itemPara", "M"));
                        break;
                }

                if (accion.Equals("N"))
                {
                    List<SqlParameter> parametros1 = new List<SqlParameter>();
                    parametros1.Add(new SqlParameter("itemCod", this.itemCod));

                    String query = "SELECT * FROM tbIvtrItem WHERE itemCod = @itemCod ORDER BY itemCod";

                    DataTable dt = conn.ejecutaQuery(query, parametros1);

                    if (dt != null)
                    {
                        res.resultado = false;
                        res.mensaje = "Datos del Item no se pudieron grabar, " + ((char)13) +
                            "El código ingresado ya existe";
                        return res;
                    }

                    conn.cerrar();

                    if (conn.conectar())
                    {
                        query = "INSERT INTO tbIvtrItem (itemCod, sgrpCod, itemNbr, itemGIva, itemIva, itemPrc, itemPiv, itemAtv, itemCoc, itemParr, itemBar, itemSbg, itemDelCoc, barcode, itemund, itempara, itemals";

                        if (this.itemImg != null)
                        {
                            query += ", itemImg";
                        }

                        if (!this.prdCod.Equals(""))
                        {
                            query += ", prdCod";
                        }

                        query += ") VALUES (@itemCod, @sgrpCod, @itemNbr, @itemGIva, @itemIva, @itemPrc, @itemPiv, @itemAtv, @itemCoc, @itemParr, @itemBar, @itemSbg, @itemDelCoc, @itembcde, @itemund, @itempara, @itemals";

                        if (this.itemImg != null)
                        {
                            query += ", @itemImg";
                        }

                        if (!this.prdCod.Equals(""))
                        {
                            query += ", @prdCod";
                        }

                        query += ")";

                        var result = conn.updateQuery(query, parametros);
                        res.resultado = result.resultado;
                        if (res.resultado)
                        {
                            res.mensaje = "Datos del Item grabados correctamente";
                        }
                        else
                        {
                            res.mensaje = "Los datos del Item no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                        }
                    }
                }
                else
                {
                    String query = "UPDATE tbIvtrItem SET itemNbr=@itemNbr, sgrpCod=@sgrpCod, itemGIva = @itemGIva, itemIva=@itemIva, itemPrc=@itemPrc, itemPiv=@itemPiv, itemAtv = @itemAtv, itemCoc = @itemCoc, itemParr = @itemParr, itemBar = @itemBar, itemDelCoc = @itemDelCoc, itemSbg = @itemSbg, barcode = @itembcde, itemund = @itemund, itempara = @itempara, itemals = @itemals";
                    if (this.itemImg != null)
                    {
                        query += ", itemImg = @itemImg";
                    }
                    else
                    {
                        query += ", itemImg = null";
                    }

                    if (!this.prdCod.Equals(""))
                    {
                        query += ", prdCod = @prdCod";
                    }
                    else
                    {
                        query += ", prdCod = null";
                    }

                    query += " WHERE itemCod=@itemCod";

                    var result = conn.updateQuery(query, parametros);
                    res.resultado = result.resultado;
                    if (res.resultado)
                    {
                        res.mensaje = "Datos del Item grabados correctamente";
                    }
                    else
                    {
                        res.mensaje = "Los datos del Item no pudieron ser grabados!, " + ((char)13) + "comuniquese con el administrador!";
                    }
                }

                conn.cerrar();
            }

            return res;
        }

        public DataTable buscar(String nombre, string grupo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstItem(@itemNbr, @grupo) ORDER BY codigo";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("itemNbr", '%' + nombre + '%'));
                parametros.Add(new SqlParameter("grupo", grupo));

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
            }

            return dt;
        }

        public List<mItems> buscarPorNombre(String nombre, string orden)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;

            List<mItems> items = new List<mItems>();

            if (conn.conectar())
            {
                String query = "SELECT * FROM fnLstItem(@itemNbr, '') WHERE estado = 'ACTIVO' ORDER BY @orden";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("itemNbr", '%' + nombre + '%'));
                parametros.Add(new SqlParameter("orden", orden));

                dt = conn.ejecutaQuery(query, parametros);
                conn.cerrar();

                if (dt != null)
                {
                    dt.TableName = "Listado";

                    foreach(DataRow dr in dt.Rows)
                    {
                        items.Add(new mItems
                        {
                            itemcod = dr["codigo"].ToString(),
                            itemnbr = dr["producto"].ToString(),
                            grupo = dr["grupo"].ToString(),
                            itematv = dr["estado"].ToString()== "ACTIVO" ? true: false
                        });
                    }
                }
            }

            return items;
        }

        public DataTable getAllItems()
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT itemCod AS codigo, itemNbr AS descripcion " +
                               "FROM  tbIvtrItem i " +
                               "WHERE i.itemAtv = true";
                
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

        public DataTable getItemsByGroup(string codigo, string para = "R")
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                string query = "SELECT * FROM fnItemsByGroup(@grupo, @tipo)";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("grupo", codigo));
                parametros.Add(new SqlParameter("tipo", para));

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
            }

            return dt;
        }

        public DataTable getById(String codigo, string carrier)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                String query = "SELECT * FROM fnItemsToPeddV2(@itemCod, @carrier); ";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("itemCod", codigo));
                parametros.Add(new SqlParameter("carrier", carrier));

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
            }

            return dt;
        }

        public Boolean getById()
        {
            Boolean res = false;
            DbConnection conn = new DbConnection(strConnection); ;
            if (conn.conectar())
            {
                String query = "SELECT i.itemCod, i.itemNbr, g.grpCod, s.sgrpCod, i.itemPrc, i.itemPiv, i.itemGIva, i.itemIva, " +
                               "i.itemAtv, i.itemImg, i.prdCod, i.itemCoc, i.itemParr, i.itemBar, i.itemSbg, i.itemDelCoc, i.barcode, i.itemund, itempara, itemals " + 
                               "FROM tbIvtrItem i INNER JOIN tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod " +
                               "INNER JOIN tbIvtrGrp g ON g.grpCod = s.grpCod WHERE i.itemCod = @itemCod";

                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("itemCod", this.itemCod));

                DataTable dt = null;

                dt = conn.ejecutaQuery(query, parametros);
                dt.TableName = "Listado";
                conn.cerrar();

                if (dt.Rows.Count > 0)
                {
                    this.itemCod = dt.Rows[0]["itemCod"].ToString();
                    this.itemNbr = dt.Rows[0]["itemNbr"].ToString();
                    this.itemPrc = Utils.Helper.StringToDouble( dt.Rows[0]["itemPrc"].ToString());
                    this.itemGravaIva = (Boolean)dt.Rows[0]["itemGIva"];
                    this.itemIva = (Boolean)dt.Rows[0]["itemIva"];
                    this.itemAtv = (Boolean)dt.Rows[0]["itemAtv"];
                    this.itemCoc = (Boolean)dt.Rows[0]["itemCoc"];
                    this.itemParr = (Boolean)dt.Rows[0]["itemParr"];
                    this.itemBar = (Boolean)dt.Rows[0]["itemBar"];
                    this.itemDelCoc = (Boolean)dt.Rows[0]["itemDelCoc"];
                    this.itemSbg = (Boolean)dt.Rows[0]["itemSbg"];
                    this.grpCod = dt.Rows[0]["grpCod"].ToString();
                    this.sgrpCod = dt.Rows[0]["sgrpCod"].ToString();
                    this.prdCod = dt.Rows[0]["prdCod"].ToString();               
                    this.itemBcde = dt.Rows[0]["barcode"].ToString();
                    this.itemUnd = dt.Rows[0]["itemund"].ToString();
                    this.itemals = dt.Rows[0]["itemals"].ToString();

                    switch (dt.Rows[0]["itempara"].ToString())
                    {
                        case "T":
                            this.itemPara = 0;
                            break;
                        case "R":
                            this.itemPara = 1;
                            break;
                        case "M":
                            this.itemPara = 2;
                            break;
                    }

                    if (dt.Rows[0]["itemImg"].Equals(DBNull.Value))
                    {
                        this.itemImg = null;
                    }
                    else
                    {
                        this.itemImg = (byte[])dt.Rows[0]["itemImg"];
                    }

                    res = true;
                }
            }

            return res;
        }

        public void itemsInventario(String peddScl, List<ProdPedd> productos)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                List<mItemInv> itms = new List<mItemInv>();
                List<mNoIngred> nIngs = new List<mNoIngred>();

                int i = 0;
                foreach(ProdPedd prd in productos)
                {
                    mItemInv itm = new mItemInv
                    {
                        id = i,
                        codigo = prd.codigo,
                        cantidad = prd.cantidad                        
                    };

                    if (prd.noIngredientes.Count > 0)
                    {                        
                        foreach(mIngredientes ing in prd.noIngredientes)
                        {
                            mNoIngred noIng = new mNoIngred()
                            {
                                id = itm.id,
                                codigo = ing.codigo
                            };

                            nIngs.Add(noIng);
                        }
                    }
                    else
                    {
                        mNoIngred noIng = new mNoIngred()
                        {
                            id = itm.id,
                            codigo = ""
                        };

                        nIngs.Add(noIng);
                    }
                    i++;

                    itms.Add(itm);
                }

                parametros.Add(new SqlParameter("peddScl", peddScl));
                parametros.Add(new SqlParameter("data", JsonConvert.SerializeObject(itms)));
                parametros.Add(new SqlParameter("detalle", JsonConvert.SerializeObject(nIngs)));

                String query = "SELECT * FROM fnItemsInvent(@peddScl, @data, @detalle)";

                dt = conn.ejecutaQuery(query, parametros);
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
        }

        public void itemsInventarioReversar(String peddScl, List<ProdPedd> productos)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                List<mItemInv> itms = new List<mItemInv>();
                List<mNoIngred> nIngs = new List<mNoIngred>();

                int i = 0;
                foreach (ProdPedd prd in productos)
                {
                    mItemInv itm = new mItemInv
                    {
                        id = i,
                        codigo = prd.codigo,
                        cantidad = prd.cantidad
                    };

                    if (prd.noIngredientes.Count > 0)
                    {
                        foreach (mIngredientes ing in prd.noIngredientes)
                        {
                            mNoIngred noIng = new mNoIngred()
                            {
                                id = itm.id,
                                codigo = ing.codigo
                            };

                            nIngs.Add(noIng);
                        }
                    }
                    else
                    {
                        mNoIngred noIng = new mNoIngred()
                        {
                            id = itm.id,
                            codigo = ""
                        };

                        nIngs.Add(noIng);
                    }
                    i++;

                    itms.Add(itm);
                }

                parametros.Add(new SqlParameter("peddScl", peddScl));
                parametros.Add(new SqlParameter("data", JsonConvert.SerializeObject(itms)));
                parametros.Add(new SqlParameter("detalle", JsonConvert.SerializeObject(nIngs)));
                 
                String query = "SELECT * FROM fnItemsInventRev(@peddScl, @data, @detalle)";

                dt = conn.ejecutaQuery(query, parametros);
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
        }

        public List<rMovItems> reporteMovimientoInventario(string inicio, string fin)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<rMovItems> movItems = new List<rMovItems>();

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("inicio", inicio));
                parametros.Add(new SqlParameter("fin", fin));

                String query = "SELECT 	p.prdCod AS Codigo, " +
                               "        p.prdNbr AS Producto, " +
                               "        m.dinvdsc AS Descripcion, " +
                               "        CASE m.dInvTip " +
                               "            WHEN 'I' THEN 'Ingreso' " +
                               "            WHEN 'E' THEN 'Egreso' " +
                               "        END AS Movimiento, " +
                               "        m.dInvFch AS Fecha, " +
                               "        SUM(m.dInvCnt) AS Cantidad, " +
                               "        CASE m.dInvUnd " +
                               "            WHEN 'K' THEN 'Kilo' " +
                               "            WHEN 'U' THEN 'Unidad' " +
                               "            WHEN 'L' THEN 'Libra' " +
                               "            WHEN 'Q' THEN 'Quintal' " +
                               "            WHEN 'O' THEN 'Onza' " +
                               "            WHEN 'G' THEN 'Gramo' " +
                               "            WHEN 'P' THEN 'Porción' " +
                               "        END AS Unidad " +
                               "FROM    ( " +
                               "            SELECT " +
                               "                i.prdCod, " +
                               "                i.dInvFch, " +
                               "                trim(substring(i.dInvDsc, 1, position(' ' in i.dInvDsc))) AS dInvDsc, " +
                               "                i.dInvTip, " +
                               "                i.dInvCnt, " +
                               "                i.dInvUnd " +
                               "            FROM tbivtrdinvmt i " +
                               "            WHERE i.dInvFch >= to_date(@inicio,'DD/MM/YYYYY') AND i.dInvFch <= to_date(@fin,'DD/MM/YYYYY') " +
                               "        ) m " +
                               "        JOIN " +
                               "        tbivtrprd p ON p.prdCod = m.prdCod " +
                               "GROUP BY m.dInvFch,p.prdCod,p.prdNbr, m.dInvTip,m.dInvUnd ,m.dinvdsc " +
                               "ORDER BY m.dInvFch, p.prdNbr, m.dInvTip";

                DataTable dt = conn.ejecutaQuery(query, parametros);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        movItems.Add(new rMovItems()
                        {
                            codigo = dr["codigo"].ToString(),
                            fecha = dr["fecha"].ToString(),
                            producto = dr["producto"].ToString(),
                            descripcion = dr["descripcion"].ToString(),
                            movimiento = dr["movimiento"].ToString(),
                            cantidad = Utils.Helper.StringToDouble( dr["cantidad"].ToString()),
                            unidad = dr["unidad"].ToString()
                        });
                    }
                }

                conn.cerrar();
            }

            return movItems;
        }

        public List<object> reporteKardex(int anio, int mes)
        {
            DbConnection conn = new DbConnection(strConnection);
            List<object> movItems = new List<object>();

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("anio", anio));

                String query = "";

                var codMes = new Dictionary<int, string>();

                codMes.Add(1, "en");
                codMes.Add(2, "fe");
                codMes.Add(3, "ma");
                codMes.Add(4, "ab");
                codMes.Add(5, "my");
                codMes.Add(6, "ju");
                codMes.Add(7, "jl");
                codMes.Add(8, "ag");
                codMes.Add(9, "sp");
                codMes.Add(10, "oc");
                codMes.Add(11, "nv");
                codMes.Add(12, "di");

                if (mes > 0)
                {
                    query = "SELECT 	i.prdNbr AS producto, krdxani AS anio,  " +
                            "           COALESCE(krdxb" + codMes[mes] + ",0) AS inicioMes, " +
                            "           COALESCE(krdxi" + codMes[mes] + ",0) AS ingresoMes, " +
                            "           COALESCE(krdxe" + codMes[mes] + ",0) AS egresoMes, " +
                            "           COALESCE(krdxs" + codMes[mes] + ",0) AS saldoMes " +
                            "FROM    tbIvtrKdrxMT k " +
                            "        join tbivtrprd i ON i.prdcod = k.prdcod " +
                            "WHERE krdxani = @anio";
                }
                else
                {   
                    query = "SELECT i.prdNbr, k.krdxani, " +
                            "       COALESCE(k.krdxben, 0) AS krdxben, " +
                            "       COALESCE(k.krdxien, 0) AS krdxien, " +
                            "       COALESCE(k.krdxeen, 0) AS krdxeen, " +
                            "       COALESCE(k.krdxsen, 0) AS krdxsen, " +

                            "       COALESCE(k.krdxbfe, 0) AS krdxbfe, " +
                            "       COALESCE(k.krdxife, 0) AS krdxife, " +
                            "       COALESCE(k.krdxefe, 0) AS krdxefe, " +
                            "       COALESCE(k.krdxsfe, 0) AS krdxsfe, " +

                            "       COALESCE(k.krdxbma, 0) AS krdxbma, " +
                            "       COALESCE(k.krdxima, 0) AS krdxima, " +
                            "       COALESCE(k.krdxema, 0) AS krdxema, " +
                            "       COALESCE(k.krdxsma, 0) AS krdxsma, " +

                            "       COALESCE(k.krdxbab, 0) AS krdxbab, " +
                            "       COALESCE(k.krdxiab, 0) AS krdxiab, " +
                            "       COALESCE(k.krdxeab, 0) AS krdxeab, " +
                            "       COALESCE(k.krdxsab, 0) AS krdxsab, " +

                            "       COALESCE(k.krdxbmy, 0) AS krdxbmy, " +
                            "       COALESCE(k.krdximy, 0) AS krdximy, " +
                            "       COALESCE(k.krdxemy, 0) AS krdxemy, " +
                            "       COALESCE(k.krdxsmy, 0) AS krdxsmy, " +

                            "       COALESCE(k.krdxbju, 0) AS krdxbju, " +
                            "       COALESCE(k.krdxiju, 0) AS krdxiju, " +
                            "       COALESCE(k.krdxeju, 0) AS krdxeju, " +
                            "       COALESCE(k.krdxsju, 0) AS krdxsju, " +

                            "       COALESCE(k.krdxbjl, 0) AS krdxbjl, " +
                            "       COALESCE(k.krdxijl, 0) AS krdxijl, " +
                            "       COALESCE(k.krdxejl, 0) AS krdxejl, " +
                            "       COALESCE(k.krdxsjl, 0) AS krdxsjl, " +
                        
                            "       COALESCE(k.krdxbag, 0) AS krdxbag, " +
                            "       COALESCE(k.krdxiag, 0) AS krdxiag, " +
                            "       COALESCE(k.krdxeag, 0) AS krdxeag, " +
                            "       COALESCE(k.krdxsag, 0) AS krdxsag, " +

                            "       COALESCE(k.krdxbsp, 0) AS krdxbsp, " +
                            "       COALESCE(k.krdxisp, 0) AS krdxisp, " +
                            "       COALESCE(k.krdxesp, 0) AS krdxesp, " +
                            "       COALESCE(k.krdxssp, 0) AS krdxssp, " +

                            "       COALESCE(k.krdxboc, 0) AS krdxboc, " +
                            "       COALESCE(k.krdxioc, 0) AS krdxioc, " +
                            "       COALESCE(k.krdxeoc, 0) AS krdxeoc, " +
                            "       COALESCE(k.krdxsoc, 0) AS krdxsoc, " +

                            "       COALESCE(k.krdxbnv, 0) AS krdxbnv, " +
                            "       COALESCE(k.krdxinv, 0) AS krdxinv, " +
                            "       COALESCE(k.krdxenv, 0) AS krdxenv, " +
                            "       COALESCE(k.krdxsnv, 0) AS krdxsnv, " +

                            "       COALESCE(k.krdxbdi, 0) AS krdxbdi, " +
                            "       COALESCE(k.krdxidi, 0) AS krdxidi, " +
                            "       COALESCE(k.krdxedi, 0) AS krdxedi, " +
                            "       COALESCE(k.krdxsdi, 0) AS krdxsdi " +
                            "FROM   tbIvtrKdrxMT k " +
                            "       JOIN tbivtrprd i ON i.prdcod = k.prdcod " +
                            "WHERE  krdxani = @anio";
                }

                DataTable dt = conn.ejecutaQuery(query, parametros);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (mes > 0)
                        {
                            movItems.Add(new rMovItemsMes
                            {
                                producto = dr["producto"].ToString(),
                                anio = int.Parse(dr["anio"].ToString()),
                                inicioMes = Utils.Helper.StringToDouble(dr["inicioMes"].ToString()),
                                ingresosMes = Utils.Helper.StringToDouble(dr["ingresoMes"].ToString()),
                                egresosMes = Utils.Helper.StringToDouble(dr["egresoMes"].ToString()),
                                saldosMes = Utils.Helper.StringToDouble(dr["saldoMes"].ToString()),
                            });
                        }
                        else
                        {
                            try {
                                movItems.Add(new rMovItemsKrdx
                                {
                                    producto = dr["prdNbr"].ToString(),
                                    anio = int.Parse(dr["krdxani"].ToString()),
                                    inicioEnero = Utils.Helper.StringToDouble(dr["krdxben"].ToString()),
                                    ingresosEnero = Utils.Helper.StringToDouble(dr["krdxien"].ToString()),
                                    egresosEnero = Utils.Helper.StringToDouble(dr["krdxeen"].ToString()),
                                    saldosEnero = Utils.Helper.StringToDouble(dr["krdxsen"].ToString()),

                                    inicioFebrero = Utils.Helper.StringToDouble(dr["krdxbfe"].ToString()),
                                    ingresosFebrero = Utils.Helper.StringToDouble(dr["krdxife"].ToString()),
                                    egresosFebrero = Utils.Helper.StringToDouble(dr["krdxefe"].ToString()),
                                    saldosFebrero = Utils.Helper.StringToDouble(dr["krdxsfe"].ToString()),

                                    inicioMarzo = Utils.Helper.StringToDouble(dr["krdxbma"].ToString()),
                                    ingresosMarzo = Utils.Helper.StringToDouble(dr["krdxima"].ToString()),
                                    egresosMarzo = Utils.Helper.StringToDouble(dr["krdxema"].ToString()),
                                    saldosMarzo = Utils.Helper.StringToDouble(dr["krdxsma"].ToString()),

                                    inicioAbril = Utils.Helper.StringToDouble(dr["krdxbab"].ToString()),
                                    ingresosAbril = Utils.Helper.StringToDouble(dr["krdxiab"].ToString()),
                                    egresosAbril = Utils.Helper.StringToDouble(dr["krdxeab"].ToString()),
                                    saldosAbril = Utils.Helper.StringToDouble(dr["krdxsab"].ToString()),

                                    inicioMayo = Utils.Helper.StringToDouble(dr["krdxbmy"].ToString()),
                                    ingresosMayo = Utils.Helper.StringToDouble(dr["krdximy"].ToString()),
                                    egresosMayo = Utils.Helper.StringToDouble(dr["krdxemy"].ToString()),
                                    saldosMayo = Utils.Helper.StringToDouble(dr["krdxsmy"].ToString()),

                                    inicioJunio = Utils.Helper.StringToDouble(dr["krdxbju"].ToString()),
                                    ingresosJunio = Utils.Helper.StringToDouble(dr["krdxiju"].ToString()),
                                    egresosJunio = Utils.Helper.StringToDouble(dr["krdxeju"].ToString()),
                                    saldosJunio = Utils.Helper.StringToDouble(dr["krdxsju"].ToString()),

                                    inicioJulio = Utils.Helper.StringToDouble(dr["krdxbjl"].ToString()),
                                    ingresosJulio = Utils.Helper.StringToDouble(dr["krdxijl"].ToString()),
                                    egresosJulio = Utils.Helper.StringToDouble(dr["krdxejl"].ToString()),
                                    saldosJulio = Utils.Helper.StringToDouble(dr["krdxsjl"].ToString()),

                                    inicioAgosto = Utils.Helper.StringToDouble(dr["krdxbag"].ToString()),
                                    ingresosAgosto = Utils.Helper.StringToDouble(dr["krdxiag"].ToString()),
                                    egresosAgosto = Utils.Helper.StringToDouble(dr["krdxeag"].ToString()),
                                    saldosAgosto = Utils.Helper.StringToDouble(dr["krdxsag"].ToString()),

                                    inicioSeptiembre = Utils.Helper.StringToDouble(dr["krdxbsp"].ToString()),
                                    ingresosSeptiembre = Utils.Helper.StringToDouble(dr["krdxisp"].ToString()),
                                    egresosSeptiembre = Utils.Helper.StringToDouble(dr["krdxesp"].ToString()),
                                    saldosSeptiembre = Utils.Helper.StringToDouble(dr["krdxssp"].ToString()),

                                    inicioOctubre = Utils.Helper.StringToDouble(dr["krdxboc"].ToString()),
                                    ingresosOctubre = Utils.Helper.StringToDouble(dr["krdxioc"].ToString()),
                                    egresosOctubre = Utils.Helper.StringToDouble(dr["krdxeoc"].ToString()),
                                    saldosOctubre = Utils.Helper.StringToDouble(dr["krdxsoc"].ToString()),

                                    inicioNoviembre = Utils.Helper.StringToDouble(dr["krdxbnv"].ToString()),
                                    ingresosNoviembre = Utils.Helper.StringToDouble(dr["krdxinv"].ToString()),
                                    egresosNoviembre = Utils.Helper.StringToDouble(dr["krdxenv"].ToString()),
                                    saldosNoviembre = Utils.Helper.StringToDouble(dr["krdxsnv"].ToString()),

                                    inicioDiciembre = Utils.Helper.StringToDouble(dr["krdxben"].ToString()),
                                    ingresosDiciembre = Utils.Helper.StringToDouble(dr["krdxidi"].ToString()),
                                    egresosDiciembre = Utils.Helper.StringToDouble(dr["krdxedi"].ToString()),
                                    saldosDiciembre = Utils.Helper.StringToDouble(dr["krdxsdi"].ToString()),
                                });
                            }catch(Exception e)
                            {
                                Console.Write(e.Message);
                            }
                        }
                    }
                }

                conn.cerrar();
            }

            return movItems;
        }

        public DataTable itemsReceta(String nombre, String estado)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("nombre", '%' + nombre + '%'));
                parametros.Add(new SqlParameter("estado", estado));

                String query = "SELECT * FROM fnLstRcta(@nombre, @estado)";

                dt = conn.ejecutaQuery(query, parametros);
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

        public DataTable getRecetaDetalle(String codigo)
        {
            DbConnection conn = new DbConnection(strConnection);
            DataTable dt = null;
            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("codigo", codigo));

                String query = "SELECT r.itemCod, p.prdCod, p.prdNbr, r.rctaCnt, r.rctaUnd, r.rctaInv, r.rctaObv FROM tbIvtrRcta r INNER JOIN tbIvtrPrd p ON p.prdcod = r.prdcod WHERE r.itemCod = @codigo";

                dt = conn.ejecutaQuery(query, parametros);
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

        public Respuesta grabarReceta(String accion)
        {
            Respuesta res = new Respuesta();
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("accion", accion));
                parametros.Add(new SqlParameter("data", this.itemCod));
                parametros.Add(new SqlParameter("detalle", JsonConvert.SerializeObject(this.receta)));

                String query = "SELECT * FROM fnReceta(@accion::varchar, @data::varchar, @detalle::varchar)";
                DataTable dt = conn.ejecutaQuery(query, parametros);

                if (dt != null)
                {
                    res.resultado = int.Parse(dt.Rows[0]["codigo"].ToString()) == 1 ? true : false;
                    res.mensaje = dt.Rows[0]["descripcion"].ToString();
                }else
                {
                    res.resultado = false;
                    res.mensaje = "No fue posible crear la receta";
                }

                conn.cerrar();
            }

            return res;
        }
    }
}

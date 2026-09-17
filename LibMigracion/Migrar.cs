using AccesoDatos;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;

namespace LibMigracion
{
    public class Migrar: IDisposable
    {
        protected String strConnection { get; set; }
        protected String sqlConnection { get; set; }

        private DbConnection conn;
        private DbSqlConnection csql;
        public double progreso = 0;
        private int cntRegTbls = 0;
        private int cntRegTot = 0;
        private string tablaImport = "";

        public delegate void actualizaProgreso(double valor);
        public delegate void actualizaDetalleMigracion(string texto);
        public event actualizaProgreso actualiza;
        public event actualizaDetalleMigracion ActualizaDetalleMigracion;

        public Migrar(String strConnection, String sqlConnection)
        {
            this.strConnection = strConnection;
            this.sqlConnection = sqlConnection;

            conn = new DbConnection(strConnection);
            csql = new DbSqlConnection(sqlConnection);
        }

        public void iniciar()
        {
            Boolean pgcon = conn.conectar();
            Boolean sqcon = csql.conectar();

            this.actualiza(0);

            if (pgcon && sqcon)
            {
                // migrando datos de clientes
                this.tablaImport = "Importando datos de Clientes ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                String sql = "select * from tbadsinid";                
                DataTable dt = csql.ejecutaQuery(sql);
                DataTable dt2 = null;
                DataTable dt3 = null;

                //migrarClientes(dt);
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Caja ";
                this.ActualizaDetalleMigracion(this.tablaImport + " "+ this.cntRegTbls +" total " + this.cntRegTot +" registros");
                sql = "select * from tbcajacaja";
                //dt = csql.ejecutaQuery(sql);
                //migrarCajas(dt);
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Empleados ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select ('000000'+emplcod) as emplcod, emplClv, emplcod as usrcod, " +
                      "      SUBSTRING(ltrim(emplNbr),0,charindex(' ',ltrim(emplNbr))) as emplNbr, " +
                      "      SUBSTRING(ltrim(emplNbr), charindex(' ', ltrim(emplNbr)) + 1, len(ltrim(emplNbr))) as emplApe, " + 
	                  "      case " +
		              "          when emplMsro = 1 and emplCjro = 1 and emplSpvs = 1 then 'A' " +
                      "          when emplMsro = 1 and emplCjro = 1 and emplSpvs = 0 then 'S' " +
                      "          when emplMsro = 1 and emplCjro = 0 and emplSpvs = 0 then 'M' " +
                      "          when emplMsro = 0 and emplCjro = 1 and emplSpvs = 0 then 'C' " +
                      "      end as emplTip " +
                      "from tbPeddEmpl";
                //dt = csql.ejecutaQuery(sql);
                //migrarEmpleados(dt);
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Grupos de Comidas ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select * from tbIvtrGrup";
                //dt = csql.ejecutaQuery(sql);
                ///migrarGrupos(dt);
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Subgrupos de Comidas ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select * from tbIvtrSbgr";
                //dt = csql.ejecutaQuery(sql);
                //migrarSubGrupos(dt);
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Items ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select * from tbIvtrItem";
                //dt = csql.ejecutaQuery(sql);
                //migrarItems(dt);
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Movimientos Entrada de Caja ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select * from tbCajavain";
                //dt = csql.ejecutaQuery(sql);
                if (dt!= null)
                {
                    //migrarValesIngreso(dt);
                }                
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Movimientos Salida de Caja ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select * from tbCajavaeg";
                //dt = csql.ejecutaQuery(sql);
                if(dt != null)
                {
                    //migrarValesEgreso(dt);
                }                
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Pedidos Cabecera ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select distinct ('0000'+p.peddScl) AS peddScl, p.emplCod, p.peddFch, p.peddTip, " +
		              "        case p.peddMsa when '' then '' else '0' + p.peddMsa end as peddMsa, " +
                      "        coalesce(m.peddlta,'A') as peddlta, " +
		              "        p.peddAtv, p.HraIni, p.HraFin " + 
                      "from tbPeddDPedd p " +
                      "     full outer join " +
                      "     tbPeddMsaLtra m on p.peddScl = m.peddScl " +
                      "order by peddscl";
                //dt = csql.ejecutaQuery(sql);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Pedidos Detalle ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select '0000'+b.peddScl as peddScl, '0000'+b.ventscl as ventscl, b.itemcod, b.itemctd, b.itematv, b.itemprc, (itempto - itemprc) as itempiv, b.itempto, b.ventfch " +
                      "from( " +
                      "      select  a.peddScl, a.ventScl, a.itemCod, a.itemctd, a.itematv, " +
                      "              case  " +
                      "                  when a.ventFch < '2016/06/01' then a.itemprc / 1.12 " +
                      "                  when a.ventFch >= '2016/06/01' and a.ventFch < '2017/06/01' then a.itemprc / 1.14 " +
                      "                  when a.ventFch >= '2017/06/01' then a.itemprc / 1.12 " +
                      "              end as itemprc, " +
                      "              a.itemprc as itempto, " +
                      "              a.ventFch " +
                      "      from( " +
                      "          select  p.peddScl, m.itemCtd, c.ventscl, d.itemcod, m.itemPrc, c.ventFch, m.itemAtv " +
                      "          from   tbPeddMVent d  " +
                      "                 inner join " +
                      "                 tbPeddDVent c on c.ventScl = d.ventScl " +
                      "                 inner join " +
                      "                 tbPeddDPedd p on p.peddScl = c.peddScl  " +
                      "                 inner join " +
                      "                 tbPeddMPedd m on m.peddScl = p.peddScl and m.itemCod = d.itemCod " +
                      "      ) a " +
                      ")b";

                //dt2 = csql.ejecutaQuery(sql);
                if(dt != null)
                {
                    //migrarPedidos(dt, dt2);
                }
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Ventas Cabecera ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select '0000'+b.peddScl as peddScl, '0000'+b.ventscl as ventscl, " +
                      "        b.cajacod, b.emplcod, b.ventfch, b.itemcod,  " +
                      "        b.clieNid, b.ventTip, b.ventmsa, b.ventLta, b.ventHra, b.itemcod, b.itemprc,  " +
                      "        b.ventdsc, (itempto - itemprc) as itempiv, b.itempto, b.itemAtv,  " +
                      "        b.ventRcb, b.ventCbo, b.ventatv, b.ventFact  " +
                      "from( " +
                      "    select  a.peddScl, a.ventscl, a.cajacod, a.emplcod, a.ventfch, " +
                      "            a.clieNid, a.ventTip, a.ventmsa, a.ventLta, a.ventHra, a.itemcod, " +
                      "            case   " +
                      "                when a.ventfch < '2016/06/01' then a.itemprc / 1.12 " +
                      "                when a.ventfch >= '2016/06/01' and a.ventfch < '2017/06/01' then a.itemprc / 1.14 " +
                      "                when a.ventfch >= '2017/06/01' then a.itemprc / 1.12 " +
                      "            end as itemprc, a.ventdsc, a.itemprc as itempto, a.itemAtv, " +
                      "            a.ventRcb, a.ventCbo, a.ventatv, a.ventFact " +
                      "    from(" +
                      "        select distinct " + 
                      "                 p.peddScl, c.ventscl, c.cajacod, c.emplcod, c.ventfch, " +
                      "                 c.clieNid, c.ventTip, c.ventmsa,coalesce(ms.peddlta,'A') as ventLta, c.ventHra, d.itemcod, " +
                      "                 (SUM(m.itemCtd * m.itemPrc) - c.ventDsc) as itemprc, m.itemAtv, c.ventdsc, " +
                      "                 c.ventRcb, c.ventCbo, c.ventatv, c.ventFact " +
                      "        from     tbPeddMVent d " +
                      "                 inner join " +
                      "                 tbPeddDVent c on c.ventScl = d.ventScl " +
                      "                 inner join " +
                      "                 tbPeddDPedd p on p.peddScl = c.peddScl " +
                      "                 inner join " +
                      "                 tbPeddMPedd m on m.peddScl = p.peddScl and m.itemCod = d.itemCod " +
                      "                 full outer join " + 
                      "                 tbPeddMsaLtra ms on p.peddScl = ms.peddScl " +
                      "        group by p.peddScl, c.ventscl, c.cajacod, c.emplcod, c.ventfch, " +
                      "                c.clieNid, c.ventTip, c.ventmsa, c.ventHra, d.itemcod, " +
                      "                c.ventFch, m.itemAtv, c.ventRcb, c.ventCbo, c.ventatv, c.ventFact, " +
                      "                c.ventDsc, ms.peddLta " +
                      "    ) a " +
                      ")b " +
                      "where b.peddscl is not null";

                dt = csql.ejecutaQuery(sql);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Ventas Detalle ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select   '0000'+b.ventscl as ventscl, b.itemcod, b.itemctd, b.itematv, b.itemprc, " +
                      "         (itempto - itemprc) as itempiv, b.itempto " +
                      "from( " +
                      "      select  a.ventScl, a.itemcod, a.itemctd, a.itematv, " +
                      "              case " +
                      "                  when a.ventFch < '2016/06/01' then a.itemprc / 1.12 " +
                      "                  when a.ventFch >= '2016/06/01' and a.ventFch < '2017/06/01' then a.itemprc / 1.14 " +
                      "                  when a.ventFch >= '2017/06/01' then a.itemprc / 1.12 " +
                      "              end as itemprc, " +
                      "              a.itemprc as itempto " +
                      "      from( " +
                      "          select  p.peddScl, m.itemCtd, c.ventscl, d.itemcod, m.itemPrc, c.ventFch, m.itemAtv " +
                      "          from    tbPeddMVent d " +
                      "                  inner join " +
                      "                  tbPeddDVent c on c.ventScl = d.ventScl " +
                      "                  inner join " +
                      "                  tbPeddDPedd p on p.peddScl = c.peddScl " +
                      "                  inner join " +
                      "                  tbPeddMPedd m on m.peddScl = p.peddScl and m.itemCod = d.itemCod " +
                      "      ) a " +
                      ")b";
                dt2 = csql.ejecutaQuery(sql);
                if(dt != null)
                {
                    migrarVentas(dt, dt2);
                }
                progreso += 9.09;
                this.actualiza(progreso);

                Thread.Sleep(1000);
                this.tablaImport = "Importando datos de Ventas Forma de Pago ";
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
                sql = "select	'0000'+f.ventscl as ventscl, " +
                      "		    case f.tifpcod " +
                      "                when '0001' then 'E' " +
                      "                when '0002' then 'C' " +
                      "                when '0003' then 'T' " +
                      "                when '0004' then 'T' " +
                      "         end as fpgotip, " +
                      "		    f.tifpval as fpgoval, f.tifpval - (select ventTot from tbPeddDVent v where v.ventScl = f.ventscl) as fpgovto, " +
                      "		    case f.tifpcod " +
                      "                when '0003' then 'M' " +
                      "                when '0004' then 'C' " +
                      "			       else 'N' " +
                      "         end as fpgotjc,	case f.tifpcod when '0003' then 'C' when '0004' then 'C' else 'N' end as fpgotpt,	0 as fpgomss,'' as fpgoche, " +
                      "		    case f.tifpcod "  +
                      "                when '0002' then '' " +
                      "			       else '' " +
                      "         end as fpgobco " +
                      "from     tbPeddDVentFP f";
                dt = csql.ejecutaQuery(sql);
                if (dt != null)
                {
                    migrarFormasPago(dt);
                }
                progreso += 9.09;
                this.actualiza(progreso);
            }
            else
            {

            }
        }

        private void migrarCajas(DataTable dt)
        {
            String pgl = "insert into tbcajacaja (cajacod, cajanbr) values (@cajacod, @cajanbr)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("cajacod", dr["cajacod"]));
                parametros.Add(new SqlParameter("cajanbr", dr["cajanbr"]));

                conn.updateQuery(pgl, parametros);

                this.cntRegTbls++;
                this.cntRegTot ++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }           
        }

        private  void migrarEmpleados(DataTable dt)
        {
            this.tablaImport = "Importando datos de Empleados Cabecera ";
            String pgl = "insert into tbAdsiEmpd " +
                         "(empdnid, empdtid, empdnbr, empdape, empddir, empdtlf, empdcel, empdfna, empdchj, empdatv) " +
                         "values " +
                         "(@empdnid, @empdtid, @empdnbr, @empdape, null, null, null, null, 0, true)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("empdnid", dr["emplcod"]));
                parametros.Add(new SqlParameter("empdtid", "P"));
                parametros.Add(new SqlParameter("empdnbr", dr["emplnbr"]));
                parametros.Add(new SqlParameter("empdape", dr["emplape"]));                

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }

            Thread.Sleep(1000);
            this.tablaImport = "Importando datos de Empleados Detalle ";
            pgl = "insert into tbAdsiUsua (usuacod, usuausr, usuanbr, usuaclv, usuacad, usuatip, empdnid, usuaatv, usuacnm) " +
                  "values " +
                  "(@usuacod, @usuausr, @usuanbr, encryptar(@usuaclv), encryptar(@usuacad), @usuatip, @empdnid, true, encryptar(@usuacnm))";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                string usuario ="";

                if (dr["emplnbr"].ToString().Equals(""))
                {
                    usuario = dr["emplape"].ToString();
                }else
                {
                    usuario = dr["emplnbr"].ToString().Substring(0, 1) + dr["emplape"].ToString();
                }                

                parametros.Add(new SqlParameter("usuacod", dr["usrcod"]));
                parametros.Add(new SqlParameter("usuausr", usuario));
                parametros.Add(new SqlParameter("usuanbr", dr["emplnbr"] + " " + dr["emplape"]));
                parametros.Add(new SqlParameter("usuaclv", dr["emplclv"]));
                parametros.Add(new SqlParameter("usuacad", dr["empltip"].ToString()=="A"? dr["emplclv"] : "Tr3sk3lS0ft"));
                parametros.Add(new SqlParameter("usuatip", dr["empltip"]));
                parametros.Add(new SqlParameter("empdnid", dr["emplcod"]));
                parametros.Add(new SqlParameter("usuacnm", dr["emplclv"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
            
            dt.Dispose();
        }

        private void migrarClientes(DataTable dt)
        {
            String pgl = "insert into tbadsinid (nidnid, nidtip, nidnbr, niddir, nidtlf) values (@nidnid, @nidtip, @nidnbr, @niddir, @nidtlf)";

            this.cntRegTbls = 0;
            foreach(DataRow dr in dt.Rows){
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("nidnid", dr["nidnid"]));
                parametros.Add(new SqlParameter("nidtip", dr["nidtipnid"]));
                parametros.Add(new SqlParameter("nidnbr", dr["nidnbr"]));
                parametros.Add(new SqlParameter("niddir", dr["niddir"]));
                parametros.Add(new SqlParameter("nidtlf", dr["nidtlf"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarGrupos(DataTable dt)
        {
            String pgl = "insert into tbIvtrGrp (grpcod, grpnbr) values (@grpcod, @grpnbr)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("grpcod", dr["grupcod"]));
                parametros.Add(new SqlParameter("grpnbr", dr["grupnbr"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }            
        }

        private void migrarSubGrupos(DataTable dt)
        {
            String pgl = "insert into tbIvtrSGrp (sgrpcod, sgrpnbr, grpcod) values (@sgrpcod, @sgrpnbr, @grpcod)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("sgrpcod", dr["sbgrcod"]));
                parametros.Add(new SqlParameter("sgrpnbr", dr["sbgrnbr"]));
                parametros.Add(new SqlParameter("grpcod", dr["grupcod"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarItems(DataTable dt)
        {
            String pgl = "insert into tbIvtrItem " +
                         "(itemcod, sgrpcod, itemnbr, itemiva, itemprc, itempiv, itemimg, itematv, itemcoc, itemsbg) " +
                         "values " +
                         "(@itemcod, @sgrpcod, @itemnbr, true, @itemprc, @itempiv, null, true, true, false)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("itemcod", dr["itemcod"]));
                parametros.Add(new SqlParameter("sgrpcod", dr["sbgrcod"]));
                parametros.Add(new SqlParameter("itemnbr", dr["itemnbr"]));
                parametros.Add(new SqlParameter("itemprc", Double.Parse(dr["itemprc"].ToString())));
                parametros.Add(new SqlParameter("itempiv", Double.Parse(dr["itemprc"].ToString()) / (1.12)));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarValesIngreso(DataTable dt)
        {
            String pgl = "insert into tbcajavaig (vaigsec, vaigfch, cajacod, vaigdet, vaigval, usuacod, vaigfgr, usuaeli, vaigfel) " +
                         "values (@vaigsec, @vaigfch, @cajacod, @vaigdet, @vaigval, @usuacod, @vaigfgr, @usuaeli, @vaigfel) ";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("vaigsec", dr["vaigsec"]));
                parametros.Add(new SqlParameter("vaigfch", dr["vaigfch"]));
                parametros.Add(new SqlParameter("cajacod", dr["cajacod"]));
                parametros.Add(new SqlParameter("vaigdet", dr["vaigdet"]));
                parametros.Add(new SqlParameter("vaigval", dr["vaigval"]));
                parametros.Add(new SqlParameter("usuacod", dr["usrgbr"]));
                parametros.Add(new SqlParameter("vaigfgr", dr["fchgbr"]));
                parametros.Add(new SqlParameter("usuaeli", dr["usreli"]));
                parametros.Add(new SqlParameter("vaigfel", dr["fcheli"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls ++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarValesEgreso(DataTable dt)
        {
            String pgl = "insert into tbcajavaeg (vaegsec, vaegfch, cajacod, vaegdet, vaegval, usuacod, vaegfgr, usuaeli, vaegfel) " +
                         "values (@vaegsec, @vaegfch, @cajacod, @vaegdet, @vaegval, @usuacod, @vaegfgr, @usuaeli, @vaegfel) ";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("vaegsec", dr["vaegscl"]));
                parametros.Add(new SqlParameter("vaegfch", dr["vaegfch"]));
                parametros.Add(new SqlParameter("cajacod", dr["cajacod"]));
                parametros.Add(new SqlParameter("vaegdet", dr["vaegdet"]));
                parametros.Add(new SqlParameter("vaegval", dr["vaegval"]));
                parametros.Add(new SqlParameter("usuacod", dr["usrgbr"]));
                parametros.Add(new SqlParameter("vaegfgr", dr["fchgbr"]));
                parametros.Add(new SqlParameter("usuaeli", dr["usreli"]));
                parametros.Add(new SqlParameter("vaegfel", dr["fcheli"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarPedidos(DataTable dt, DataTable dt2)
        {
            this.tablaImport = "Importando datos Pedidos Cabecera ";
            String pgl = "insert into tbPeddDPedd " +
                         "(peddscl, peddlta, usuacod, peddfec, peddtip, peddmsa, peddobv, peddhin, peddhfn, peddatv, peddeli) " +
                         "values " +
                         "(@peddscl, @peddlta, @usuacod, @peddfec, @peddtip, @peddmsa, null, @peddhin, @peddhfn, @peddatv, false)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("peddscl", dr["peddScl"]));
                parametros.Add(new SqlParameter("peddlta", dr["peddLta"]));
                parametros.Add(new SqlParameter("usuacod", dr["emplCod"]));
                parametros.Add(new SqlParameter("peddfec", dr["peddFch"]));
                parametros.Add(new SqlParameter("peddtip", dr["peddTip"]));
                parametros.Add(new SqlParameter("peddmsa", dr["peddMsa"]));
                parametros.Add(new SqlParameter("peddhin", dr["HraIni"]));
                parametros.Add(new SqlParameter("peddhfn", dr["HraFin"]));
                parametros.Add(new SqlParameter("peddatv", dr["peddAtv"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }

            Thread.Sleep(1000);
            this.tablaImport = "Importando datos de Pedidos Detalle ";
            pgl = "insert into tbPeddMPedd " +
                  "(peddscl, itemcod, itemcnt, itemiva, itemprc, itempiv, itempto, peddatv) " + 
                  "values " +
                  "(@peddscl, @itemcod, @itemcnt, true, @itemprc, @itempiv, @itempto, @peddatv)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt2.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("peddscl", dr["peddScl"]));
                parametros.Add(new SqlParameter("itemcod", dr["itemcod"]));
                parametros.Add(new SqlParameter("itemcnt", dr["itemctd"]));
                parametros.Add(new SqlParameter("itemprc", dr["itemprc"]));
                parametros.Add(new SqlParameter("itempiv", dr["itempiv"]));
                parametros.Add(new SqlParameter("itempto", dr["itempto"]));
                parametros.Add(new SqlParameter("peddatv", dr["itematv"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls ++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarVentas(DataTable dt, DataTable dt2)
        {
            this.tablaImport = "Importando datos de Ventas Cabecera ";
            String pgl = "insert into tbPeddDVent " +
                         "(ventscl, cajacod, usuacod, ventfch, nidnid, venttip, ventmsa, ventlta, peddscl, venthra, ventsbt, ventdsc, ventiva, venttot, ventrcb, ventvto, ventobv, ventatv, ventfac, ventdpc, ventpgd) " +
                         "values " +
                         "(@ventscl, @cajacod, @usuacod, @ventfch, @nidnid, @venttip, @ventmsa, @ventlta, @peddscl, @venthra, @ventsbt, @ventdsc, @ventiva, @venttot, @ventrcb, @ventvto, '', @ventatv, @ventfac, 0, true)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ventscl", dr["ventscl"]));
                parametros.Add(new SqlParameter("cajacod", dr["cajacod"]));
                parametros.Add(new SqlParameter("usuacod", dr["emplcod"]));
                parametros.Add(new SqlParameter("ventfch", dr["ventfch"]));
                parametros.Add(new SqlParameter("nidnid",  dr["clienid"]));
                parametros.Add(new SqlParameter("venttip", dr["venttip"]));
                parametros.Add(new SqlParameter("ventmsa", dr["ventmsa"]));
                parametros.Add(new SqlParameter("ventlta", dr["ventlta"]));
                parametros.Add(new SqlParameter("peddscl", dr["peddscl"]));
                parametros.Add(new SqlParameter("venthra", dr["venthra"]));
                parametros.Add(new SqlParameter("ventsbt", dr["itemprc"]));
                parametros.Add(new SqlParameter("ventdsc", dr["ventdsc"]));
                parametros.Add(new SqlParameter("ventiva", dr["itempiv"]));
                parametros.Add(new SqlParameter("venttot", dr["itempto"]));
                parametros.Add(new SqlParameter("ventrcb", dr["ventrcb"]));
                parametros.Add(new SqlParameter("ventvto", dr["ventcbo"]));
                parametros.Add(new SqlParameter("ventatv", dr["ventatv"]));
                parametros.Add(new SqlParameter("ventfac", dr["ventfact"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }

            Thread.Sleep(1000);
            this.tablaImport = "Importando datos de Ventas Detalle ";
            pgl = "insert into tbPeddMVent " +
                  "(ventscl, itemcod, itemcnt, itemprc, itemiva, itempto) " +
                  "values " +
                  "(@ventscl, @itemcod, @itemcnt, @itemprc, @itempiv, @itempto)";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt2.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ventscl", dr["ventscl"]));
                parametros.Add(new SqlParameter("itemcod", dr["itemcod"]));
                parametros.Add(new SqlParameter("itemcnt", dr["itemctd"]));
                parametros.Add(new SqlParameter("itemprc", dr["itemprc"]));
                parametros.Add(new SqlParameter("itempiv", dr["itempiv"]));
                parametros.Add(new SqlParameter("itempto", dr["itempto"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        private void migrarFormasPago(DataTable dt)
        {
            String pgl = "insert into tbpedddfpgo (ventscl, fpgotip, fpgoval, fpgovto, fpgotjc, fpgotpt, fpgomss, fpgobco, fpgoche) " +
                         "values (@ventscl, @fpgotip, @fpgoval, @fpgovto, @fpgotjc, @fpgotpt, @fpgomss, @fpgobco, @fpgoche) ";

            this.cntRegTbls = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("ventscl", dr["ventscl"]));
                parametros.Add(new SqlParameter("fpgotip", dr["fpgotip"]));
                parametros.Add(new SqlParameter("fpgoval", dr["fpgoval"]));
                parametros.Add(new SqlParameter("fpgovto", dr["fpgovto"]));
                parametros.Add(new SqlParameter("fpgotjc", dr["fpgotjc"]));
                parametros.Add(new SqlParameter("fpgotpt", dr["fpgotpt"]));
                parametros.Add(new SqlParameter("fpgomss", dr["fpgomss"]));
                parametros.Add(new SqlParameter("fpgobco", dr["fpgobco"]));
                parametros.Add(new SqlParameter("fpgoche", dr["fpgoche"]));

                conn.updateQuery(pgl, parametros);
                this.cntRegTbls++;
                this.cntRegTot++;
                this.ActualizaDetalleMigracion(this.tablaImport + " " + this.cntRegTbls + " total " + this.cntRegTot + " registros");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Migrar() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

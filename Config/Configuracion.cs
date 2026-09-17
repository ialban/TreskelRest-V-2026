using LibConfig.cls;
using LibConfig.datasets;
using LibEncrypt;
using System;
using System.Data;
using System.IO;

namespace LibConfig
{
    public class Configuracion : IDisposable
    {
        public Boolean ExistConfig()
        {
            Boolean existe = false;

            if (File.Exists("config.xml"))
            {
                existe = true;
            }
            return existe;
        }

        public Boolean creaConfig(Config nuevo)
        {
            Boolean res = false;
            try
            {
                dsConfig ds = new dsConfig();

                DataTable dt = ds.Tables["SysConfig"];
                DataRow dr = dt.NewRow();

                CryptUtil crypUtil = new CryptUtil();

                dr["usaTouch"] = nuevo.system.UsaTouch;
                dr["printerFactura"] = nuevo.system.PrinterFactura;
                dr["printerProforma"] = nuevo.system.PrinterProforma;
                dr["printerCocina"] = nuevo.system.PrinterCocina;
                dr["printerParrilla"] = nuevo.system.PrinterParrilla;
                dr["printerBar"] = nuevo.system.PrinterBar;
                dr["caja"] = nuevo.system.Caja;

                dt.Rows.Add(dr);

                DataTable db = ds.Tables["Database"];
                dr = db.NewRow();

                dr["server"] = nuevo.database.Server;
                dr["base"] = nuevo.database.DbName;
                dr["puerto"] = nuevo.database.Puerto;
                dr["usuario"] = crypUtil.Encriptar(nuevo.database.Usuario);
                dr["clave"] = crypUtil.Encriptar(nuevo.database.Clave);

                db.Rows.Add(dr);

                if (File.Exists("config.xml"))
                {
                    File.Delete("config.xml");
                }

                ds.WriteXml("config.xml");

                res = true;
            }
            catch(Exception e)
            {
                res = false;
            }            

            return res;
        }

        public Config readConfig()
        {
            Config config = new Config();

            dsConfig ds = new dsConfig();

            ds.ReadXml("config.xml");

            DataTable dt = ds.Tables["SysConfig"];
            for(int i=0; i<dt.Rows.Count; i++)
            {
                config.system.UsaTouch=(Boolean) dt.Rows[i]["usaTouch"];
                config.system.PrinterFactura= dt.Rows[i]["printerFactura"].ToString() == "Ninguna" ? "" : dt.Rows[i]["printerFactura"].ToString();
                config.system.PrinterProforma = dt.Rows[i]["printerProforma"].ToString() == "Ninguna" ? "" : dt.Rows[i]["printerProforma"].ToString();
                config.system.PrinterCocina = dt.Rows[i]["printerCocina"].ToString() == "Ninguna" ? "" : dt.Rows[i]["printerCocina"].ToString();
                config.system.PrinterParrilla = dt.Rows[i]["printerParrilla"].ToString() == "Ninguna" ? "" : dt.Rows[i]["printerParrilla"].ToString();
                config.system.PrinterBar = dt.Rows[i]["printerBar"].ToString() == "Ninguna" ? "" : dt.Rows[i]["printerBar"].ToString();
                config.system.Caja = dt.Rows[i]["caja"].ToString() == "" ? "0001" : dt.Rows[i]["caja"].ToString();
            }

            dt = ds.Tables["Database"];
            CryptUtil crypUtil = new CryptUtil();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                config.database.Server = (string) dt.Rows[i]["server"];
                config.database.DbName = (string) dt.Rows[i]["base"];
                config.database.Puerto = (long) dt.Rows[i]["puerto"];
                config.database.Usuario= crypUtil.Dencriptar((string) dt.Rows[i]["usuario"]);
                config.database.Clave = crypUtil.Dencriptar((string) dt.Rows[i]["clave"]);
            }

            return config;
        }

        public ConfigDbSql leeConfigSQL(string configFile)
        {
            ConfigDbSql config = new ConfigDbSql();

            dsCfgCnxDB ds = new dsCfgCnxDB();

            ds.ReadXml(configFile);

            DataTable dt = ds.Tables["cfg"];

            clsEncDec crypUtil = new clsEncDec();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                config.SrvSql = crypUtil.desencriptar((string) dt.Rows[i]["SrvSql"]);
                config.InsSql = crypUtil.desencriptar((string) dt.Rows[i]["InsSql"]);
                config.PrtSql = crypUtil.desencriptar((string) dt.Rows[i]["PrtSql"]);
                config.TipAut = crypUtil.desencriptar((string) dt.Rows[i]["TipAut"]);
                config.UsrSql = crypUtil.desencriptar((string) dt.Rows[i]["UsrSql"]);
                config.ClvSql = crypUtil.desencriptar((string) dt.Rows[i]["ClvSql"]);
                config.NbrDB = crypUtil.desencriptar((string)dt.Rows[i]["NbrDB"]);
            }
            crypUtil.Dispose();

            return config;
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
        // ~Configuracion() {
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

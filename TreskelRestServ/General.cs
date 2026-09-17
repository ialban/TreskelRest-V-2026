using Entidades;
using LibConfig.cls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreskelRestServ
{
    public class General
    {
        public static String connectionString = "";

        public static String RUC_EMPRESA = "";
        public static String EMPRESA = "";

        public static Boolean COPIAR_CORREOS = false;

        public static String CORREO_HOST = "";
        public static String CORREO_PORT = "";
        public static String CORREO_USER = "";
        public static String CORREO1 = "";
        public static String CORREO2 = "";
        public static String CORREO3 = "";
        public static String CORREO4 = "";
        public static String CORREO_PASSWORD = "";
        public static String CORREO_AUTH = "";
        public static String CORREO_UTLS = "";
        public static string MODO_VALIDACION_ANULA_ELIMINA = "N";

        public static String HORA1 = "";
        public static String HORA2 = "";
        public static String HORA3 = "";
        public static String CANTIDAD_ARCHIVOS = "";

        public static String ENVIAMAIL = "";
        public static String UPLOAD = "";
        public static String SERVER = "";
        public static String USER = "";
        public static String PASSWORD = "";
        public static String PG_DUMP_LOC = "";
        public static bool EJECUTA_BKP = false;
        public static int PUERTO = 21;
        public static int CONTROLSECUENCIA = 1;


        public static void generaConnectionString(Config config)
        {
            connectionString += "server=" + config.database.Server + ";";
            connectionString += "port=" + config.database.Puerto + ";";
            connectionString += "database=" + config.database.DbName + ";";
            connectionString += "user id=" + config.database.Usuario + ";";
            connectionString += "password=" + config.database.Clave + ";";
        }

        public static bool checkInstalled(string c_name)
        {
            string displayName;

            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(c_name))
                    {
                        return true;
                    }
                }
                key.Close();
            }

            registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(c_name))
                    {
                        return true;
                    }
                }
                key.Close();
            }
            return false;
        }

        public static void loadParametros()
        {
            Parametros parametros = new Parametros(General.connectionString);

            DataTable dt = parametros.listar();

            String numeroCrd = "";
            String validadorCrd = "";

            foreach (DataRow dr in dt.Rows)
            {
                switch ((int)dr["codigo"])
                {
                    case 6:
                        General.CORREO_HOST = dr["valor"].ToString();
                        break;
                    case 7:
                        General.CORREO_PORT = dr["valor"].ToString();
                        break;
                    case 8:
                        General.CORREO_USER = dr["valor"].ToString();
                        break;
                    case 9:
                        General.CORREO_PASSWORD = dr["valor"].ToString();
                        break;
                    case 10:
                        General.CORREO_AUTH = dr["valor"].ToString();
                        break;
                    case 11:
                        General.CORREO_UTLS = dr["valor"].ToString();
                        break;
                    case 14:
                        General.CORREO1 = dr["valor"].ToString();
                        break;
                    case 15:
                        General.COPIAR_CORREOS = dr["valor"].ToString().Equals("1") ? true : false;
                        break;
                    case 16:
                        General.CORREO2 = dr["valor"].ToString();
                        break;
                    case 17:
                        General.CORREO3 = dr["valor"].ToString();
                        break;
                    case 18:
                        General.CORREO4 = dr["valor"].ToString();
                        break;
                    case 29:
                        General.HORA1 = dr["valor"].ToString();
                        break;
                    case 30:
                        General.CANTIDAD_ARCHIVOS = dr["valor"].ToString();
                        break;
                    case 31:
                        General.ENVIAMAIL = dr["valor"].ToString();
                        break;
                    case 33:
                        General.UPLOAD = dr["valor"].ToString();
                        break;
                    case 34:
                        General.SERVER = dr["valor"].ToString();
                        break;
                    case 35:
                        General.USER = dr["valor"].ToString();
                        break;
                    case 36:
                        General.PASSWORD = dr["valor"].ToString();
                        break;
                    case 37:
                        General.HORA2 = dr["valor"].ToString();
                        break;
                    case 38:
                        General.HORA3 = dr["valor"].ToString();
                        break;
                    case 43:
                        General.EJECUTA_BKP = dr["valor"].ToString() == "1" ? true : false;
                        break;
                    //case 44:
                    //    General.PG_DUMP_LOC = dr["valor"].ToString();
                    //    break;
                    case 114:
                        General.PUERTO = !string.IsNullOrEmpty(dr["valor"].ToString()) ? int.Parse(dr["valor"].ToString()) : 21;
                        break;
                    case 115:
                        General.CONTROLSECUENCIA = int.Parse(dr["valor"].ToString());
                        break;
                    case 143:
                        General.MODO_VALIDACION_ANULA_ELIMINA = dr["valor"].ToString();
                        break;
                    case 9996:
                        numeroCrd = dr["valor"].ToString();
                        break;
                    case 9997:
                        validadorCrd = dr["valor"].ToString();
                        break;
                    case 9998:
                        General.RUC_EMPRESA = dr["valor"].ToString();
                        break;
                    case 9999:
                        General.EMPRESA = dr["valor"].ToString();
                        break;
                }
            }

            dt = parametros.PgDumpLocation();
            foreach(DataRow dr in dt.Rows)
            {
                General.PG_DUMP_LOC = dr["descripcion"].ToString();
            }
        }

        public static void loadParametrosBackup()
        {
            Parametros parametros = new Parametros(General.connectionString);

            DataTable dt = parametros.ParametrosBackup();

            //string numeroCrd = "";
            //string validadorCrd = "";

            foreach (DataRow dr in dt.Rows)
            {
                switch ((int)dr["codigo"])
                {
                    case 29:
                        General.HORA1 = dr["valor"].ToString();
                        break;
                    case 30:
                        General.CANTIDAD_ARCHIVOS = dr["valor"].ToString();
                        break;
                    case 31:
                        General.ENVIAMAIL = dr["valor"].ToString();
                        break;
                    case 33:
                        General.UPLOAD = dr["valor"].ToString();
                        break;
                    case 34:
                        General.SERVER = dr["valor"].ToString();
                        break;
                    case 35:
                        General.USER = dr["valor"].ToString();
                        break;
                    case 36:
                        General.PASSWORD = dr["valor"].ToString();
                        break;
                    case 37:
                        General.HORA2 = dr["valor"].ToString();
                        break;
                    case 38:
                        General.HORA3 = dr["valor"].ToString();
                        break;
                    case 43:
                        General.EJECUTA_BKP = dr["valor"].ToString() == "1" ? true : false;
                        break;
                    //case 44:
                    //    General.PG_DUMP_LOC = dr["valor"].ToString();
                    //    break;
                    case 143:
                        General.MODO_VALIDACION_ANULA_ELIMINA = dr["valor"].ToString();
                        break;
                }
            }
        }

        public static void FinEjecutaBackup()
        {
            Parametros parametros = new Parametros(General.connectionString);

            parametros.SetEjecutaBackup(false);
        }
    }
}

using System;

namespace LibConfig.cls
{
    public class Database
    {
        private String server = "";
        public String Server {
            get {
                return server;
            }
            set {
                server = value;
            }
        }

        private String dbname = "";
        public String DbName
        {
            get
            {
                return dbname;
            }
            set
            {
                dbname = value;
            }
        }

        private long puerto = 0;
        public long Puerto
        {
            get
            {
                return puerto;
            }
            set
            {
                puerto = value;
            }
        }

        private String usuario = "";
        public String Usuario
        {
            get
            {
                return usuario;
            }
            set
            {
                usuario = value;
            }
        }

        private String clave = "";
        public String Clave
        {
            get
            {
                return clave;
            }
            set
            {
                clave = value;
            }
        }
    }
}

using AccesoDatos;
using System;

namespace Entidades
{
    public class TestDbConnection : EntidadBase
    {
        public TestDbConnection(string strConnection) : base(strConnection)
        {

        }

        public Boolean Test()
        {
            DbConnection conn = new DbConnection(strConnection);

            if (conn.conectar())
            {
                conn.cerrar();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

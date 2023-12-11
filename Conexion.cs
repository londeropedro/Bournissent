using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Bournissent
{
    class Conexion
    {


        private SqlConnection ConexionDB = new SqlConnection(@"Server=DESKTOP-348851V; DataBase=Bournissent; Integrated Security=true");


        public SqlConnection AbrirConexion()
        {

            try
            {
                if (ConexionDB.State == ConnectionState.Closed)
                    ConexionDB.Open();
                return ConexionDB;
            }

            catch (Exception e)

            {
                throw;
            }
        }

        public SqlConnection CerrarConexion()
        {

            try
            {
                if (ConexionDB.State == ConnectionState.Open)
                    ConexionDB.Close();
                return ConexionDB;
            }

            catch (Exception e)
            {
                throw;
            }


        }


    }


}

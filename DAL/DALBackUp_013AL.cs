using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALBackUp_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public void RealizarBackup_013AL(string backupPath)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string nombreArchivo = $"MiSistema.bak";
                    string rutaCompleta = System.IO.Path.Combine(backupPath, nombreArchivo);
                    string comandoBackup = $"BACKUP DATABASE Dietética TO DISK='{rutaCompleta}'";
                    
                    SqlCommand cmd = new SqlCommand(comandoBackup, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception("Error al realizar backup", ex); }
        }

        public void RealizarRestore_013AL(string backupFilePath)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();

                    using (SqlCommand setMaster = new SqlCommand("USE master;", con))
                    {
                        setMaster.ExecuteNonQuery();
                    }

                    using (SqlCommand setSingleUser = new SqlCommand("ALTER DATABASE Dietética SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", con))
                    {
                        setSingleUser.ExecuteNonQuery();
                    }

                    string query = $"RESTORE DATABASE Dietética FROM DISK = '{backupFilePath}' WITH REPLACE;";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand setMultiUser = new SqlCommand("ALTER DATABASE Dietética SET MULTI_USER;", con))
                    {
                        setMultiUser.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al realizar restore", ex);
            }
        }
    }
}

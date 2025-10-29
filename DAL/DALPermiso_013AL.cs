using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALPermiso_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public string ObtenerNombrePermiso_013AL(int idPermiso)
        {
            string nombrePermiso = string.Empty;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT [NombrePermiso-013AL] FROM [Permisos-013AL] WHERE [CodPermiso-013AL] = @idPermiso");

                    SqlCommand com = new SqlCommand(query.ToString(), con);
                    com.Parameters.AddWithValue("@idPermiso", idPermiso);
                    com.CommandType = CommandType.Text;
                    con.Open();

                    object result = com.ExecuteScalar();
                    if (result != null)
                    {
                        nombrePermiso = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                nombrePermiso = string.Empty;
                throw new Exception("Error al obtener el nombre del permiso", ex);
                
            }
            return nombrePermiso;
        }

        public DataTable TraerListaPermisos_013AL()
        {
            SqlDataReader resultado;
            DataTable tabla = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("SELECT [CodPermiso-013AL], [NombrePermiso-013AL], [Tipo-013AL] FROM [Permisos-013AL]", con);
                    com.CommandType = CommandType.Text;
                    con.Open();
                    resultado = com.ExecuteReader();
                    tabla.Load(resultado);
                }
            }
            catch (Exception ex) { throw new Exception("Error al obtener al listar permisos", ex); }
            return tabla;
        }

    }
}

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
    public class DALProductoC_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public DataTable ConsultaProductosC_013AL(int idProducto, DateTime? fechaInicio, DateTime? fechaFin)
        {
            DataTable dt = new DataTable();
            SqlDataReader resultado;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("[consultacambios-013AL]", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@idp", SqlDbType.Int).Value = idProducto;
                    cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                    cmd.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = fechaInicio ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@fechaFin", SqlDbType.Date).Value = fechaFin ?? (object)DBNull.Value;

                    con.Open();
                    resultado = cmd.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar los cambios del producto", ex);
            }
            return dt;
        }

        public DataTable ListarProductosC_013AL(int? idProducto = null)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT * FROM [ProductoC-013AL]";

                    if (idProducto.HasValue)
                        query += " WHERE [CodProducto-013AL] = @idProducto";

                    SqlCommand com = new SqlCommand(query, con);

                    if (idProducto.HasValue)
                        com.Parameters.AddWithValue("@idProducto", idProducto.Value);

                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar el historial de productos", ex);
            }
            return dt;
        }

        public void RestaurarVersionProducto_013AL(int codProductoC)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("RestaurarVersionProducto_013AL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodProductoC", codProductoC);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restaurar la versión del producto en la base de datos: ", ex);
            }
        }
    }
}

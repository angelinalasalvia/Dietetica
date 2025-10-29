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

        public DataTable ConsultaProductosC_013AL(int? idp, string nombre, DateTime? fechaInicio, DateTime? fechaFin)
        {
            DataTable dt = new DataTable();
            SqlDataReader resultado;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("[consultacambios-013AL]", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    if (idp == null)
                        cmd.Parameters.Add("@idp", SqlDbType.Int).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@idp", SqlDbType.Int).Value = idp;


                    if (string.IsNullOrEmpty(nombre))
                        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 50).Value = nombre;


                    if (fechaInicio == null)
                        cmd.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = fechaInicio;


                    if (fechaFin == null)
                        cmd.Parameters.Add("@fechaFin", SqlDbType.Date).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@fechaFin", SqlDbType.Date).Value = fechaFin;

                    con.Open();
                    resultado = cmd.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar los cambios de productos", ex);
            }
            return dt;
        }

        public DataTable ListarProductosC_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("SELECT * FROM [ProductoC-013AL]", con);
                    //com.CommandType = CommandType.StoredProcedure;
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

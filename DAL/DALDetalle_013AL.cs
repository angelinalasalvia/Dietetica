using BE_013AL;
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
    public class DALDetalle_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public List<Detalle_013AL> ListarDetalle_013AL()
        {
            List<Detalle_013AL> Lista = new List<Detalle_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "Select [CodCompra-013AL], [CodProducto-013AL], [Cantidad-013AL], [PrecioUnitario-013AL] from [Detalle-013AL]";
                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Detalle_013AL()
                            {
                                CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]),
                                CodProducto_013AL = Convert.ToInt32(dr["CodProducto-013AL"]),
                                Cantidad_013AL = Convert.ToInt32(dr["Cantidad-013AL"]),
                                PrecioUnitario_013AL = Convert.ToInt32(dr["PrecioUnitario-013AL"])

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar detalle", ex);
            }
            return Lista;
        }

        public string ActualizarCantidadDetalle_013AL(int codCompra, int codProducto, int nuevaCantidad)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "UPDATE [Detalle-013AL] SET [Cantidad-013AL] = @nuevaCantidad WHERE [CodCompra-013AL] = @codCompra AND [CodProducto-013AL] = @codProducto";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nuevaCantidad", nuevaCantidad);
                    cmd.Parameters.AddWithValue("@codCompra", codCompra);
                    cmd.Parameters.AddWithValue("@codProducto", codProducto);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0 ? "Actualizado" : "Error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la cantidad del producto en la compra", ex);
            }
        }

        public string EliminarDetalle_013AL(int id)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("EliminarDetalle-013AL", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el producto del detalle", ex);
            }
            return respuesta;
        }

        public string AgregarDetalle_013AL(Detalle_013AL cp)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("AgregarCompraProducto-013AL", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@idc", SqlDbType.Int).Value = cp.CodCompra_013AL;
                    com.Parameters.Add("@idp", SqlDbType.Int).Value = cp.CodProducto_013AL;
                    com.Parameters.Add("@cant", SqlDbType.Int).Value = cp.Cantidad_013AL;
                    com.Parameters.Add("@pu", SqlDbType.Int).Value = cp.PrecioUnitario_013AL;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar el producto al detalle", ex);
            }
            return respuesta;
        }

        public int ObtenerUltimoIdCompra_013AL()
        {
            int ultimoId = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT ISNULL(MAX([CodCompra-013AL]), 0) AS UltimoId FROM [Detalle-013AL]";

                    SqlCommand command = new SqlCommand(query, con);
                    con.Open();

                    object resultado = command.ExecuteScalar();
                    if (resultado != null)
                    {
                        ultimoId = Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al obtener ultimo id compra ", ex); }
            return ultimoId;
        }

    }
}

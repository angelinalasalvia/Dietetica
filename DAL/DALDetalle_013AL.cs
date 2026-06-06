using BE_013AL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

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
        public Detalle_013AL ObtenerDetalle_013AL(int codCompra, int codProducto)
        {
            Detalle_013AL detalle = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("ObtenerDetalle-013AL", con);
                    com.CommandType = CommandType.StoredProcedure;

                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra; 
                    com.Parameters.Add("@CodProducto", SqlDbType.Int).Value = codProducto;

                    con.Open();

                    SqlDataReader dr = com.ExecuteReader();

                    if (dr.Read())
                    {
                        detalle = new Detalle_013AL
                        {
                            CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]),
                            CodProducto_013AL = Convert.ToInt32(dr["CodProducto-013AL"]),
                            Cantidad_013AL = Convert.ToInt32(dr["Cantidad-013AL"]),
                            PrecioUnitario_013AL = Convert.ToInt32(dr["PrecioUnitario-013AL"])
                        };
                    }
                
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar detalle: " + ex.Message);
                //throw new Exception("Error al listar detalle", ex);
            }
            return detalle;
        }

        public string ActualizarCantidadDetalle_013AL(int codCompra, int codProducto, int cantidad)
        {
            string respuesta = "";

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("ActualizarCantidadDetalle-013AL", con);

                    com.CommandType = CommandType.StoredProcedure;

                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra;
                    com.Parameters.Add("@CodProducto", SqlDbType.Int).Value = codProducto;
                    com.Parameters.Add("@Cantidad", SqlDbType.Int).Value = cantidad;

                    con.Open();

                    respuesta = com.ExecuteNonQuery() >= 1 ? "OK" : "error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar cantidad del detalle", ex);
            }

            return respuesta;
        }

        public string EliminarDetalle_013AL(int codCompra, int codProducto) 
        { 
            string respuesta = ""; 
            try 
            { 
                using (SqlConnection con = conexion.ObtenerConexion()) 
                { 
                    SqlCommand com = new SqlCommand("EliminarDetalle-013AL", con); 
                    com.CommandType = CommandType.StoredProcedure; 
                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra; 
                    com.Parameters.Add("@CodProducto", SqlDbType.Int).Value = codProducto; 
                    con.Open(); 
                    respuesta = com.ExecuteNonQuery() >= 1 ? "OK" : "ERROR"; 
                } 
            } 
            catch (Exception ex) 
            { 
                throw new Exception("Error al eliminar detalle: " + ex.Message); 
            } 
            return respuesta; 
        }
        public string AgregarDetalle_013AL(Detalle_013AL detalle)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"
                    INSERT INTO [Detalle-013AL]
                    (
                        [CodCompra-013AL],
                        [CodProducto-013AL],
                        [Cantidad-013AL],
                        [PrecioUnitario-013AL]
                    )
                    VALUES
                    (
                        @idc,
                        @idp,
                        @cant,
                        @precio
                    )";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue(
                        "@idc",
                        detalle.CodCompra_013AL
                    );

                    cmd.Parameters.AddWithValue(
                        "@idp",
                        detalle.CodProducto_013AL
                    );

                    cmd.Parameters.AddWithValue(
                        "@cant",
                        detalle.Cantidad_013AL
                    );

                    cmd.Parameters.AddWithValue(
                        "@precio",
                        detalle.PrecioUnitario_013AL
                    );

                    con.Open();

                    cmd.ExecuteNonQuery();
                }

                return "Detalle agregado correctamente";
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error al agregar producto a detalle: " +
                    ex.Message
                );
            }
        }

        public int CalcularTotalPedido_013AL(int codCompra)
        {
            int total = 0;

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("CalcularTotalPedido-013AL", con);

                    com.CommandType = CommandType.StoredProcedure;

                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra;

                    con.Open();

                    object resultado = com.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        total = Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al calcular total del pedido", ex);
            }

            return total;
        }
        public string ActualizarTotalPedido_013AL(int codCompra, int total)
        {
            string respuesta = "";

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("ActualizarTotalPedido-013AL", con);

                    com.CommandType = CommandType.StoredProcedure;

                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra;
                    com.Parameters.Add("@Total", SqlDbType.Int).Value = total;

                    con.Open();

                    respuesta = com.ExecuteNonQuery() >= 1 ? "OK" : "error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar total del pedido", ex);
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
        public List<Detalle_013AL> ListarDetallePorCompra_013AL(int codCompra)
        {
            List<Detalle_013AL> lista = new List<Detalle_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @" SELECT [CodCompra-013AL], [CodProducto-013AL], [Cantidad-013AL], [PrecioUnitario-013AL] FROM [Detalle-013AL] WHERE [CodCompra-013AL] = @CodCompra";
                    SqlCommand com = new SqlCommand(query, con);
                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra;
                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Detalle_013AL()
                        {
                            CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]),
                            CodProducto_013AL = Convert.ToInt32(dr["CodProducto-013AL"]),
                            Cantidad_013AL = Convert.ToInt32(dr["Cantidad-013AL"]),
                            PrecioUnitario_013AL = Convert.ToInt32(dr["PrecioUnitario-013AL"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar detalle: " + ex.Message);
            }
            return lista;
        }

    }
}

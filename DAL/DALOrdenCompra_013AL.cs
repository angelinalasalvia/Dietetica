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
    public class DALOrdenCompra_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public int GuardarOrdenCompra_013AL(OrdenCompra_013AL nuevaOrden)
        {
            int codOrdenCompra = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("[GuardarOrdenCompra-013AL]", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idsol", nuevaOrden.CodSolicitud_013AL);
                    cmd.Parameters.AddWithValue("@cuit", nuevaOrden.CUITProveedor_013AL);
                    cmd.Parameters.AddWithValue("@total", nuevaOrden.Total_013AL);

                    SqlParameter outputParam = new SqlParameter("@codOrdenCompra", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    codOrdenCompra = (int)outputParam.Value;
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar orden de compra", ex); }
            return codOrdenCompra;
        }


        public List<OrdenCompra_013AL> ListarOrdenCompra_013AL()
        {
            List<OrdenCompra_013AL> Lista = new List<OrdenCompra_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT * FROM [OrdenCompra-013AL]";
                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    con.Open();


                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            Lista.Add(new OrdenCompra_013AL()
                            {
                                CodOrdenCompra_013AL = Convert.ToInt32(dr["CodOrdenCompra-013AL"].ToString()),
                                CodSolicitud_013AL = Convert.ToInt32(dr["CodSolicitud-013AL"].ToString()),
                                CUITProveedor_013AL = Convert.ToInt32(dr["CUITProveedor-013AL"].ToString()),
                                Fecha_013AL = Convert.ToDateTime(dr["Fecha-013AL"].ToString()),
                                Total_013AL = Convert.ToInt32(dr["Total-013AL"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar ordenes de compra", ex);
            }
            return Lista;
        }
        public int ObtenerCodOrdenCompra_013AL(int idSolicitud, int cuitProveedor, DateTime fecha)
        {
            int codOrdenCompra = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("[ObtenerCodOrdenCompra-013AL]", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                    cmd.Parameters.AddWithValue("@CUITProveedor", cuitProveedor);
                    cmd.Parameters.AddWithValue("@Fecha", fecha);

                    con.Open();
                    codOrdenCompra = (int)cmd.ExecuteScalar(); 
                }
            }
            catch (Exception ex) { throw new Exception("Error al obtener el codigo de la orden de compra", ex); }
            return codOrdenCompra;
        }

        public string RegistrarCompra_013AL(Producto_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[RegistrarCompra-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = obj.CodProducto_013AL;
                    com.Parameters.Add("@stock", SqlDbType.Int).Value = obj.Stock_013AL;

                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
                }
            }
            catch (Exception ex) { throw new Exception("Error al registrar compra", ex); }
            return resultado;
        }
        public DataTable ListarOrdenesCompra_013AL()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                using (SqlCommand com = new SqlCommand("SELECT * FROM [OrdenCompra-013AL]", con))
                {
                    com.CommandType = CommandType.Text; // O CommandType.StoredProcedure

                    SqlDataAdapter da = new SqlDataAdapter(com);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        public DataTable ObtenerProductosPorOrden_013AL(int codOrdenCompra)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = @"
                    SELECT 
                    [Producto-013AL].[CodProducto-013AL], 
                    [Producto-013AL].[Nombre-013AL], 
                    [Producto-013AL].[Stock-013AL], 
                    [Producto-013AL].[Precio-013AL], 
                    [Producto-013AL].[Imagen-013AL], 
                    [Producto-013AL].[Descripcion-013AL], 
                    [Producto-013AL].[Bit_Lo_Bo-013AL], 
                    [DetalleSolicitudC-013AL].[Cantidad-013AL],
                    [OrdenCompra-013AL].[Completo-013AL]
                    FROM [OrdenCompra-013AL]
                    JOIN [SolicitudCotizacion-013AL] 
                    ON [OrdenCompra-013AL].[CodSolicitud-013AL] = [SolicitudCotizacion-013AL].[CodSCotizacion-013AL]
                    JOIN [DetalleSolicitudC-013AL] 
                    ON [SolicitudCotizacion-013AL].[CodSCotizacion-013AL] = [DetalleSolicitudC-013AL].[CodSCotizacion-013AL]
                    JOIN [Producto-013AL] 
                    ON [DetalleSolicitudC-013AL].[CodProducto-013AL] = [Producto-013AL].[CodProducto-013AL]
                    WHERE [OrdenCompra-013AL].[CodOrdenCompra-013AL] = @CodOrdenCompra;";
                    cmd.Parameters.AddWithValue("@CodOrdenCompra", codOrdenCompra);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public string ActualizarEstadoCompleto_013AL(int codOrdenCompra, bool completo)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                com.Parameters.AddWithValue("@Completo", completo);
                com.Parameters.AddWithValue("@CodOrdenCompra", codOrdenCompra);

                try
                {
                    con.Open();
                    com.ExecuteNonQuery();
                    return "OK";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public Proveedor_013AL TraerDatosProveedor_013AL(int codOrdenCompra)
        {
            Proveedor_013AL proveedor = null;

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[TraerDatosProveedor-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@oc", SqlDbType.Int).Value = codOrdenCompra;

                    con.Open();

                    SqlDataReader reader = com.ExecuteReader();
                    if (reader.Read())
                    {
                        proveedor = new Proveedor_013AL
                        {
                            CUIT_013AL = Convert.ToInt32(reader["CUIT-013AL"]),
                            NombreProveedor_013AL = reader["NombreProveedor-013AL"].ToString(),
                            ApellidoProveedor_013AL = reader["ApellidoProveedor-013AL"].ToString(),
                            Domicilio_013AL = reader["Domicilio-013AL"].ToString(),
                            Mail_013AL = reader["Mail-013AL"].ToString(),
                            RazonSocial_013AL = reader["RazonSocial-013AL"].ToString(),
                            Telefono_013AL = Convert.ToInt32(reader["Telefono-013AL"].ToString())
                        };
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error al traer datos del proveedor: " + ex.Message);
            }
            return proveedor;
        }
    }
}

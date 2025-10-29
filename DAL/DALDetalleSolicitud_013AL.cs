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
    public class DALDetalleSolicitud_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public string AgregarDetalleSC_013AL(DetalleSolicitudC_013AL obj)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[AgregarDetalleSC-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@sc", SqlDbType.Int).Value = obj.CodSCotizacion_013AL;
                    com.Parameters.Add("@prod", SqlDbType.Int).Value = obj.CodProducto_013AL;
                    com.Parameters.Add("@cant", SqlDbType.Int).Value = obj.Cantidad_013AL;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo AgregarDetalleSC";
                }
            }
            catch (Exception ex) { throw new Exception("Error al agregar el detalle de la solicitud de cotizacion", ex); }
            return respuesta;
        }
        public string EliminarDetalleSC_013AL(DetalleSolicitudC_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[EliminarDetalleSC-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@idp", SqlDbType.Int).Value = obj.CodProducto_013AL;
                    com.Parameters.Add("@codsc", SqlDbType.Int).Value = obj.CodSCotizacion_013AL;
                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
                }
            }
            catch (Exception ex) { throw new Exception("Error al eliminar el detalle de la solicitud de cotizacion", ex); }
            return resultado;
        }
        public DetalleSolicitudC_013AL ListarProductosOC_013AL(int codsc)
        {
            DetalleSolicitudC_013AL detalleProducto = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand command = new SqlCommand("[ListarProductosOC-013AL]", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@codsc", codsc);

                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            detalleProducto = new DetalleSolicitudC_013AL
                            {
                                CodProducto_013AL = (int)reader["CodProducto-013AL"],
                                Cantidad_013AL = (int)reader["Cantidad-013AL"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos", ex);
            }
            return detalleProducto;
        }
    }
}

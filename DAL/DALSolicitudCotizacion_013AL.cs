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
    public class DALSolicitudCotizacion_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public int AgregarSCotizacion_013AL(SolicitudCotizacion_013AL obj)
        {
            int idSolicitud = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[AgregarSCotizacion-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@valor", SqlDbType.Int).Value = obj.CUITProveedor_013AL;

                    com.Parameters.Add("@IdSolicitudCotizacion", SqlDbType.Int).Direction = ParameterDirection.Output;

                    con.Open();
                    com.ExecuteNonQuery();


                    idSolicitud = Convert.ToInt32(com.Parameters["@IdSolicitudCotizacion"].Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar la solicitud de cotización", ex);
            }
            return idSolicitud;
        }
        public List<SolicitudCotizacion_013AL> ListarSCotizacion_013AL()
        {
            List<SolicitudCotizacion_013AL> Lista = new List<SolicitudCotizacion_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT [CodSCotizacion-013AL], [CUITProveedor-013AL] FROM [SolicitudCotizacion-013AL]";
                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    con.Open();


                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            Lista.Add(new SolicitudCotizacion_013AL()
                            {
                                CodSCotizacion_013AL = Convert.ToInt32(dr["CodSCotizacion-013AL"].ToString()),
                                CUITProveedor_013AL = Convert.ToInt32(dr["CUITProveedor-013AL"].ToString())



                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar solicitud de cotizacion", ex);
            }
            return Lista;
        }
    }
}

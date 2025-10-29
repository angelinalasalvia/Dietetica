using DAL_013AL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALEvento_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public string AgregarEvento_013AL(Eventos evento)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("[AgregarEvento-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@login", SqlDbType.NVarChar).Value = evento.Login;
                    com.Parameters.Add("@modulo", SqlDbType.NVarChar).Value = evento.Modulo;
                    com.Parameters.Add("@evento", SqlDbType.NVarChar).Value = evento.Evento;
                    com.Parameters.Add("@criticidad", SqlDbType.Int).Value = evento.Criticidad;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo agregar el evento";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar evento", ex);
            }
            return respuesta;
        }

        public DataTable ListarEventos_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[ListarEventos-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar eventos", ex);
            }
            return dt;
        }

        public DataTable ConsultasEventos_013AL(string login, DateTime? fechaInicio, DateTime? fechaFin, string modulo, string evento, int? criticidad)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[consultaseventos-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;


                    if (string.IsNullOrEmpty(login))
                        com.Parameters.Add("@login", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                    else
                        com.Parameters.Add("@login", SqlDbType.NVarChar, 50).Value = login;


                    if (fechaInicio == null)
                        com.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = DBNull.Value;
                    else
                        com.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = fechaInicio;


                    if (fechaFin == null)
                        com.Parameters.Add("@fechaFin", SqlDbType.Date).Value = DBNull.Value;
                    else
                        com.Parameters.Add("@fechaFin", SqlDbType.Date).Value = fechaFin;


                    if (string.IsNullOrEmpty(modulo))
                        com.Parameters.Add("@modulo", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                    else
                        com.Parameters.Add("@modulo", SqlDbType.NVarChar, 50).Value = modulo;


                    if (string.IsNullOrEmpty(evento))
                        com.Parameters.Add("@evento", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                    else
                        com.Parameters.Add("@evento", SqlDbType.NVarChar, 50).Value = evento;


                    if (criticidad == null)
                        com.Parameters.Add("@criticidad", SqlDbType.Int).Value = DBNull.Value;
                    else
                        com.Parameters.Add("@criticidad", SqlDbType.Int).Value = criticidad;

                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar eventos", ex);
            }
            return dt;
        }

    }
}

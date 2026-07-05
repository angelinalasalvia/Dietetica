using BE;
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
    public class DALLote_013AL
    {
            private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();

            public DataTable ListaLotes_013AL()
            {
                DataTable dt = new DataTable();

                try
                {
                    using (SqlConnection con = conexion.ObtenerConexion())
                    {
                        using (SqlCommand cmd = new SqlCommand("ListaLotes-013AL", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            con.Open();

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al listar los lotes.", ex);
                }

                return dt;
            }

        public string ModificarLote_013AL(Lote_013AL lote)
        {
            string resultado = "";

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("ModificarLote-013AL", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IdLote", SqlDbType.Int).Value = lote.CodLote_013AL;
                    cmd.Parameters.Add("@FechaVencimiento", SqlDbType.Date).Value = lote.FechaVencimiento_013AL;
                    cmd.Parameters.Add("@Estado", SqlDbType.NVarChar).Value = lote.Estado_013AL;

                    con.Open();

                    resultado = cmd.ExecuteNonQuery() == 1 ? "OK" : "Error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar lote.", ex);
            }

            return resultado;
        }
        public void AgregarLote_013AL(Lote_013AL lote)
        {
            SqlConnection cn = conexion.ObtenerConexion();

            try
            {
                SqlCommand cmd = new SqlCommand("InsertarLote-013AL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodProducto", lote.CodProducto_013AL);
                cmd.Parameters.AddWithValue("@FechaVencimiento", lote.FechaVencimiento_013AL);
                cmd.Parameters.AddWithValue("@Cantidad", lote.CantidadInicial_013AL);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();

                throw;
            }
        }
        public void ActualizarEstadosLotes_013AL()
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("ActualizarEstadosLotes_013AL", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public int ContarLotesProximosAVencer_013AL()
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("ContarLotesProximosAVencer_013AL", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
    
}

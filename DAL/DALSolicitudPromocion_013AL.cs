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
    public class DALSolicitudPromocion_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        public void AgregarSolicitudPromocion_013AL(SolicitudPromocion_013AL solicitud)
        {
            SqlConnection cn = conexion.ObtenerConexion();

            try
            {
                SqlCommand cmd = new SqlCommand("InsertarSolicitudPromocion_013AL", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodLote", solicitud.CodLote_013AL);
                cmd.Parameters.AddWithValue("@FechaSolicitud", solicitud.FechaSolicitud_013AL);
                cmd.Parameters.AddWithValue("@Estado", solicitud.Estado_013AL);
                cmd.Parameters.AddWithValue("@Observaciones", solicitud.Observaciones_013AL);
                cmd.Parameters.AddWithValue("@UsuarioSolicitante", solicitud.UsuarioSolicitante_013AL);

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
        public int ContarSolicitudesPendientes_013AL()
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("ContarSolicitudesPendientes_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public DataTable ListarSolicitudesPendientes_013AL()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("ListarSolicitudesPendientes_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                dt.Load(dr);
            }

            return dt;
        }
        public DataTable TraerSolicitudPorId_013AL(int codSolicitud)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("TraerSolicitudPorId_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodSolicitud", codSolicitud);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                dt.Load(dr);
            }

            return dt;
        }
        public bool ExistePromocionActiva_013AL(int codLote)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("ExistePromocionActiva_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodLote", codLote);

                con.Open();

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }
        public bool TuvoPromocionAnterior_013AL(int codLote)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("TuvoPromocionAnterior_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodLote", codLote);

                con.Open();

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }
        public bool ExisteSolicitudPendiente_013AL(int codLote)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("ExisteSolicitudPendiente_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodLote", codLote);

                con.Open();

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }
        public void RechazarSolicitud_013AL(int codSolicitud)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd =
                    new SqlCommand("RechazarSolicitudPromocion_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodSolicitud", codSolicitud);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
        public void AprobarSolicitud_013AL(Promocion_013AL promo, SolicitudPromocion_013AL solicitud)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("AprobarSolicitudPromocion_013AL", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodSolicitud", promo.CodSolicitud_013AL);

                cmd.Parameters.AddWithValue("@Tipo", promo.Tipo_013AL);

                cmd.Parameters.AddWithValue("@Valor", promo.Valor_013AL.HasValue ? (object)promo.Valor_013AL.Value : DBNull.Value);

                cmd.Parameters.AddWithValue("@FechaInicio", promo.FechaInicio_013AL);

                cmd.Parameters.AddWithValue("@FechaFin", promo.FechaFin_013AL);

                cmd.Parameters.AddWithValue("@UsuarioSupervisor", solicitud.UsuarioSupervisor_013AL);

                cmd.Parameters.AddWithValue("@FechaResolucion", solicitud.FechaResolucion_013AL);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}

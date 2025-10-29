using BE_013AL.Composite;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAL
{
    public class DALFamilia_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public string InsertarFamiliaRol_013AL(int rol, int permiso)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[AgregarFamiliaRol-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@rol", SqlDbType.Int).Value = rol;
                    com.Parameters.Add("@familia", SqlDbType.Int).Value = permiso;


                    con.Open();
                    com.ExecuteNonQuery();
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar familia en rol", ex);
            }
        }
        public bool RegistrarHijoEnFamilia_013AL(int idPadre, int idHijo)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("[RegistrarHijosFamilia-013AL]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PermisoPadre", idPadre);
                        cmd.Parameters.AddWithValue("@PermisoHijo", idHijo);

                        SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.Int);
                        resultadoParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(resultadoParam);

                        con.Open();
                        cmd.ExecuteNonQuery();

                        int resultado = (int)cmd.Parameters["@Resultado"].Value;
                        return resultado == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la relación en la base de datos.", ex);
            }
        }
        public bool VerificarPermisoEnFamilia_013AL(int permisoId, int familiaId)
        {

            int count = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT COUNT(*) FROM [Permisos_Componente-013AL] WHERE [cod_permiso_padre-013AL] = @familiaId AND [cod_permiso_hijo-013AL] = @permisoId";


                    SqlCommand command = new SqlCommand(query, con);
                    command.Parameters.AddWithValue("@familiaId", familiaId);
                    command.Parameters.AddWithValue("@permisoId", permisoId);

                    con.Open();
                    count = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex) { throw new Exception("Error al verificar la relación en la base de datos.", ex); }
            return count > 0;

        }
        public void EliminarPermisoDeFamilia_013AL(int idPermiso, int idFamilia)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM [Permisos_Componente-013AL] WHERE [cod_permiso_hijo-013AL] = @idPermiso AND [cod_permiso_padre-013AL] = @idFamilia", con))
                    {
                        cmd.Parameters.AddWithValue("@idPermiso", idPermiso);
                        cmd.Parameters.AddWithValue("@idFamilia", idFamilia);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            MessageBox.Show($"No se encontró la relación entre la familia {idFamilia} y el permiso {idPermiso}.",
                                            "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Permiso {idPermiso} eliminado correctamente de la familia {idFamilia}.",
                                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el permiso", ex);
            }
        }
        public DataTable TraerListaFamilias_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("Select * from [Permisos-013AL]", con);
                    com.CommandType = CommandType.Text;
                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex) { throw new Exception("Error al listar familias", ex); }
            return dt;
        }
        public int CrearFamilia_013AL(string nombreFamilia)
        {
            int nuevoId = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand comando = new SqlCommand("[CrearFamilia-013AL]", con);
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@NombreFamilia", nombreFamilia);
                    con.Open();
                    object resultado = comando.ExecuteScalar();
                    if (resultado != null)
                    {
                        nuevoId = Convert.ToInt32(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la familia", ex);
            }
            return nuevoId;
        }
        public string EliminarFamilia_013AL(int id)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("[EliminarFamilia-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo eliminar";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar familia", ex);
            }
            return respuesta;
        }
        public string ModificarFamilia_013AL(Familia_013AL fam)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand comando = new SqlCommand("[ModificarFamilia-013AL]", con);
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add("@CodPermiso", SqlDbType.Int).Value = fam.Cod_013AL;
                    comando.Parameters.Add("@NombreFamilia", SqlDbType.NVarChar, 50).Value = fam.Nombre_013AL;
                    con.Open();
                    resultado = comando.ExecuteNonQuery() == 1 ? "OK" : "No se pudo modificar familia";
                }
            }
            catch (Exception ex) { throw new Exception("Error al modificar familia", ex); }
            return resultado;
        }
        public bool FamiliaTieneRelaciones_013AL(int idFamilia)
        {
            int count = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"SELECT COUNT(*) FROM [Permisos_Componente-013AL] 
                     WHERE [cod_permiso_padre-013AL] = @idFamilia OR [cod_permiso_hijo-013AL] = @idFamilia";
                    SqlCommand comando = new SqlCommand(query, con);

                    comando.Parameters.AddWithValue("@idFamilia", idFamilia);
                    con.Open();
                    count = (int)comando.ExecuteScalar();
                }
            }
            catch (Exception ex) { throw new Exception("Error", ex); }
            return count > 0;
        }
    }
}

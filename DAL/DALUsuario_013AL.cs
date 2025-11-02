using BE_013AL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_013AL.Composite;

namespace DAL
{
    public class DALUsuario_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;
                
        public Usuarios_013AL Listar_013AL(string login)
        {
            Usuarios_013AL usuario = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"SELECT [Login-013AL], [Contraseña-013AL], [DNI-013AL], [Bloqueo-013AL], [CodRol-013AL], [Activo-013AL], [Eliminado-013AL] 
                         FROM [Usuario-013AL]
                         WHERE [Login-013AL] = @login";

                    using (SqlCommand com = new SqlCommand(query, con))
                    {
                        com.Parameters.AddWithValue("@login", login);
                        con.Open();

                        using (SqlDataReader dr = com.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                usuario = new Usuarios_013AL()
                                {
                                    Login_013AL = dr["Login-013AL"].ToString(),
                                    Contraseña_013AL = dr["Contraseña-013AL"].ToString(),
                                    DNI_013AL = dr["DNI-013AL"].ToString(),
                                    Bloqueo_013AL = (bool)dr["Bloqueo-013AL"],
                                    CodRol_013AL = (int)dr["CodRol-013AL"],
                                    Activo_013AL = (bool)dr["Activo-013AL"],
                                    Eliminado_013AL = (bool)dr["Eliminado-013AL"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar usuarios", ex);
            }

            return usuario;
        }

        //int idRol = Convert.ToInt32(dr["CodRol-013AL"]);
        //Rol_013AL rol = new Rol_013AL { Cod_013AL = idRol };

        public string ObtenerContraseñaActual_013AL(string email)
        {
            string contraseña = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"SELECT [Contraseña-013AL] FROM [Usuario-013AL] WHERE [Login-013AL] = @Login";

                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Login", email);
                    con.Open();
                    var resultado = com.ExecuteScalar();
                    if (resultado != null)
                    {
                        contraseña = resultado.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo obtener la contraseña actual", ex);
            }
            return contraseña;
        }



        public string CambiarContraseña_013AL(Usuarios_013AL usuarios)
        {
            string resultado;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"Update [Usuario-013AL]
                                SET [Contraseña-013AL] = @Contraseña
                                WHERE [Login-013AL] = @Login";

                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Login", usuarios.Login_013AL);
                    com.Parameters.AddWithValue("@Contraseña", usuarios.Contraseña_013AL);
                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo cambiar la contraseña";
                }

            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo cambiar la contraseña", ex);
            }
            return resultado;
        }

        public string BloquearUsuario_013AL(Usuarios_013AL usuario)
        {
            string resultado;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"UPDATE [Usuario-013AL]
                                SET [Bloqueo-013AL] = 1
                                WHERE [Login-013AL] = @NombreUsuario;";

                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@NombreUsuario", usuario.Login_013AL);
                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo bloquear el usuario";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo bloquear al usuario", ex);
            }
            return resultado;
        }


        public string AgregarUsuario_013AL(Usuarios_013AL usuario)
        {
            string respuesta = "";

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string verificarDNI = "SELECT COUNT(*) FROM [Usuario-013AL] WHERE [DNI-013AL] = @dni";
                    string insertarUsuario = @"
                    INSERT INTO [Usuario-013AL] 
                    ([Mail-013AL], [Contraseña-013AL], [Nombres-013AL], [Apellidos-013AL], 
                    [CodRol-013AL], [DNI-013AL], [Login-013AL], [Bloqueo-013AL], [Activo-013AL], [Eliminado-013AL])
                    VALUES (@mail, @contraseña, @nombres, @apellidos, @rol, @dni, @login, @bloqueo, @activo, 0)";

                    using (SqlCommand com = new SqlCommand(verificarDNI, con))
                    {
                        com.Parameters.AddWithValue("@dni", usuario.DNI_013AL);
                        con.Open();

                        int count = (int)com.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            return "El DNI ya está registrado";
                        }
                    }

                    using (SqlCommand com = new SqlCommand(insertarUsuario, con))
                    {
                        com.Parameters.AddWithValue("@mail", usuario.Email_013AL);
                        com.Parameters.AddWithValue("@contraseña", usuario.Contraseña_013AL);
                        com.Parameters.AddWithValue("@nombres", usuario.Nombres_013AL);
                        com.Parameters.AddWithValue("@apellidos", usuario.Apellidos_013AL);
                        com.Parameters.AddWithValue("@rol", usuario.CodRol_013AL);
                        com.Parameters.AddWithValue("@dni", usuario.DNI_013AL);
                        com.Parameters.AddWithValue("@login", usuario.Login_013AL);
                        com.Parameters.AddWithValue("@bloqueo", usuario.Bloqueo_013AL);
                        com.Parameters.AddWithValue("@activo", usuario.Activo_013AL);

                        con.Open();
                        int filas = com.ExecuteNonQuery();
                        respuesta = filas == 1 ? "Usuario agregado correctamente" : "No se pudo agregar el usuario";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo agregar el nuevo usuario", ex);
            }

            return respuesta;
        }



        public DataTable ListarUsuarios_013AL()
        {
            
            SqlDataReader resultado;
            DataTable dt = new DataTable();

            try {
                using (SqlConnection con = conexion.ObtenerConexion())
                {

                    string query = @"Select * from [Usuario-013AL] order by [DNI-013AL] asc";

                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);

                } 
            }catch (Exception ex)
            {
                throw new Exception("Error al listar usuarios", ex);
            }

            return dt;
        }

        public DataSet ListarUsuariosActivos_013AL()
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT * FROM [Usuario-013AL] WHERE [Activo-013AL] = 1 AND [Eliminado-013AL] = 0";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds, "Usuario");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar usuarios activos", ex);
            }
            return ds;
        }

        public string DesbloquearUsuario_013AL(Usuarios_013AL usuario)
        {
            string resultado;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {

                    string query = @"UPDATE [Usuario-013AL]
                                SET [Bloqueo-013AL] = 0
                                WHERE [Login-013AL] = @Login;";

                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;

                    com.Parameters.AddWithValue("@Login", usuario.Login_013AL);
                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo desbloquear el usuario";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al desbloquear el usuario", ex);
            }
            return resultado;
        }


        public DataSet ObtenerUsuarios_013AL()
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "Select * from [Usuario-013AL] order by [DNI-013AL] asc";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;


                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds, "Usuario");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios", ex);
            }
            return ds;
        }

        public void GuardarUsuarios_013AL(string NombreTabla, DataSet Dset)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();

                    foreach (DataRow row in Dset.Tables["Usuario"].Rows)
                    {
                        if (row.RowState == DataRowState.Added)
                        {                            
                            string verificarQuery = "SELECT COUNT(*) FROM [Usuario-013AL] WHERE [DNI-013AL] = @dni";
                            using (SqlCommand verificarCmd = new SqlCommand(verificarQuery, con))
                            {
                                verificarCmd.Parameters.AddWithValue("@dni", row["DNI-013AL"]);
                                int existe = (int)verificarCmd.ExecuteScalar();

                                if (existe > 0)
                                {
                                    throw new Exception($"El DNI {row["DNI-013AL"]} ya está registrado.");
                                }
                            }

                            // Insertar nuevo usuario
                            string insertQuery = @"
                        INSERT INTO [Usuario-013AL] 
                        ([Mail-013AL], [Contraseña-013AL], [Nombres-013AL], [Apellidos-013AL], 
                         [CodRol-013AL], [DNI-013AL], [Login-013AL], [Bloqueo-013AL], [Activo-013AL], [Eliminado-013AL])
                        VALUES 
                        (@email, @contraseña, @nombres, @apellidos, @rol, @dni, @nomusuario, @bloqueo, @activo, 0)";

                            using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@email", row["Mail-013AL"]);
                                cmd.Parameters.AddWithValue("@contraseña", row["Contraseña-013AL"]);
                                cmd.Parameters.AddWithValue("@nombres", row["Nombres-013AL"]);
                                cmd.Parameters.AddWithValue("@apellidos", row["Apellidos-013AL"]);
                                cmd.Parameters.AddWithValue("@rol", row["CodRol-013AL"]);
                                cmd.Parameters.AddWithValue("@dni", row["DNI-013AL"]);
                                cmd.Parameters.AddWithValue("@nomusuario", row["Login-013AL"]);
                                cmd.Parameters.AddWithValue("@bloqueo", row["Bloqueo-013AL"]);
                                cmd.Parameters.AddWithValue("@activo", row["Activo-013AL"]);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (row.RowState == DataRowState.Modified)
                        {
                            // Modificar usuario
                            string updateQuery = @"
                        UPDATE [Usuario-013AL]
                        SET [Mail-013AL] = @email,
                            [Nombres-013AL] = @nombres,
                            [Apellidos-013AL] = @apellidos,
                            [CodRol-013AL] = @rol,
                            [Login-013AL] = @nomusuario,
                            [Bloqueo-013AL] = @bloqueo,
                            [Activo-013AL] = @activo,
                            [Eliminado-013AL] = @eliminado
                        WHERE [DNI-013AL] = @dni";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@email", row["Mail-013AL"]);
                                cmd.Parameters.AddWithValue("@nombres", row["Nombres-013AL"]);
                                cmd.Parameters.AddWithValue("@apellidos", row["Apellidos-013AL"]);
                                cmd.Parameters.AddWithValue("@rol", row["CodRol-013AL"]);
                                cmd.Parameters.AddWithValue("@dni", row["DNI-013AL"]);
                                cmd.Parameters.AddWithValue("@nomusuario", row["Login-013AL"]);
                                cmd.Parameters.AddWithValue("@bloqueo", row["Bloqueo-013AL"]);
                                cmd.Parameters.AddWithValue("@activo", row["Activo-013AL"]);
                                cmd.Parameters.AddWithValue("@eliminado", row["Eliminado-013AL"]);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (row.RowState == DataRowState.Deleted)
                        {
                            // Eliminar usuario
                            string deleteQuery = "DELETE FROM [Usuario-013AL] WHERE [DNI-013AL] = @dni";

                            using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                            {
                                string dni = row["DNI-013AL", DataRowVersion.Original].ToString();
                                cmd.Parameters.AddWithValue("@dni", dni);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                Dset.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar los usuarios.", ex);
            }
        }
        public DataTable ObtenerNombreApellidoPorLogin_013AL(string login)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("SELECT [Nombres-013AL], [Apellidos-013AL] FROM [Usuario-013AL] WHERE [Login-013AL] = @login", con);
                    com.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;

                    con.Open();
                    SqlDataReader resultado = com.ExecuteReader();
                    dt.Load(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener nombre y apellido", ex);
            }
            return dt;
        }
    }
}

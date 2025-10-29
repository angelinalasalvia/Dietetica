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
    public class DALRol_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public List<Rol_013AL> ListarPermisos_013AL(string dni)
        {
            List<Rol_013AL> permisos = new List<Rol_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"
                    SELECT p.[CodPermiso-013AL], p.[NombrePermiso-013AL], p.[Tipo-013AL]
                    FROM [Usuario-013AL] u
                    INNER JOIN [Roles-013AL] r ON u.[CodRol-013AL] = r.[CodRol-013AL]
                    INNER JOIN [Rol_Permiso-013AL] rp ON r.[CodRol-013AL] = rp.[CodRol-013AL]
                    INNER JOIN [Permisos-013AL] p ON rp.[CodPermiso-013AL] = p.[CodPermiso-013AL]
                    WHERE u.[DNI-013AL] = @dni";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@dni", dni);
                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                int tipo = Convert.ToInt32(dr["Tipo-013AL"]);
                                int cod = Convert.ToInt32(dr["CodPermiso-013AL"]);
                                string nombre = dr["NombrePermiso-013AL"].ToString();

                                Rol_013AL comp;

                                if (tipo == 1) 
                                {
                                    Familia_013AL familia = new Familia_013AL()
                                    {
                                        Cod_013AL = cod,
                                        Nombre_013AL = nombre,
                                        Tipo_013AL = "1"
                                    };

                                    var hijos = ObtenerHijos_013AL(cod);
                                    foreach (var hijo in hijos)
                                    {
                                        familia.AgregarHijo_013AL(hijo);
                                    }

                                    comp = familia;
                                }
                                else 
                                {
                                    comp = new Permiso_013AL()
                                    {
                                        Cod_013AL = cod,
                                        Nombre_013AL = nombre,
                                        Tipo_013AL = "0"
                                    };
                                }

                                permisos.Add(comp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                permisos = new List<Rol_013AL>();
                throw new Exception("Error al listar permisos", ex); 
            }
            return permisos;
        }
        private List<Rol_013AL> ObtenerHijos_013AL(int idPadre)
        {
            List<Rol_013AL> hijos = new List<Rol_013AL>();

            string query = @"
            SELECT p.[CodPermiso-013AL], p.[NombrePermiso-013AL], p.[Tipo-013AL]
            FROM [Permisos-013AL] p
            INNER JOIN [Permisos_Componente-013AL] pc 
                ON pc.[cod_permiso_hijo-013AL] = p.[CodPermiso-013AL]
            WHERE pc.[cod_permiso_padre-013AL] = @idPadre";

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idPadre", idPadre);
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                int tipo = Convert.ToInt32(dr["Tipo-013AL"]);
                                int cod = Convert.ToInt32(dr["CodPermiso-013AL"]);
                                string nombre = dr["NombrePermiso-013AL"].ToString();

                                Rol_013AL comp;

                                if (tipo == 1) // Familia
                                {
                                    Familia_013AL familia = new Familia_013AL()
                                    {
                                        Cod_013AL = cod,
                                        Nombre_013AL = nombre,
                                        Tipo_013AL = "1"
                                    };

                                    var hijosDeEstaFamilia = ObtenerHijos_013AL(cod);
                                    foreach (var hijo in hijosDeEstaFamilia)
                                    {
                                        familia.AgregarHijo_013AL(hijo);
                                    }

                                    comp = familia;
                                }
                                else // Permiso
                                {
                                    comp = new Permiso_013AL()
                                    {
                                        Cod_013AL = cod,
                                        Nombre_013AL = nombre,
                                        Tipo_013AL = "0"
                                    };
                                }

                                hijos.Add(comp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener hijos", ex);
            }

            return hijos;
        }

        public List<Rol_013AL> TraerListaHijos_013AL(int idPadre)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            List<Rol_013AL> lista = new List<Rol_013AL>();

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[TraerListaHijos-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@codPermisoPadre", idPadre);

                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Rol_013AL permiso;

                        int tipo = Convert.ToInt32(dr[3]);

                        if (tipo == 1)  // 1 es Familia
                        {
                            permiso = new Familia_013AL()
                            {
                                Cod_013AL = Convert.ToInt32(dr[1]),
                                Nombre_013AL = dr[2].ToString(),
                                Tipo_013AL = "Familia"
                            };
                        }
                        else if (tipo == 0)  // 0 es Permiso
                        {
                            permiso = new Permiso_013AL()
                            {
                                Cod_013AL = Convert.ToInt32(dr[1]),
                                Nombre_013AL = dr[2].ToString(),
                                Tipo_013AL = "Simple"
                            };
                        }
                        else
                        {
                            throw new Exception("Valor inesperado en la columna 'Tipo'");
                        }

                        lista.Add(permiso);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en TraerListaHijos: " + ex.Message);
            }
            return lista;
        }
        
        public List<Rol_013AL> TraerListaPermisosRol_013AL(int idRol)
        {
            List<Rol_013AL> lista = new List<Rol_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[TraerListaPermisosRol-013AL]", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodRol", idRol);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Rol_013AL permiso;
                            int tipo = Convert.ToInt32(reader["Tipo-013AL"]);

                            if (tipo == 0) // 0 = Permiso simple
                            {
                                permiso = new Permiso_013AL()
                                {
                                    Cod_013AL = Convert.ToInt32(reader["CodPermiso-013AL"]),
                                    Nombre_013AL = reader["NombrePermiso-013AL"].ToString(),
                                    Tipo_013AL = "Simple"
                                };
                            }
                            else // 1 = Familia
                            {
                                permiso = new Familia_013AL()
                                {
                                    Cod_013AL = Convert.ToInt32(reader["CodPermiso-013AL"]),
                                    Nombre_013AL = reader["NombrePermiso-013AL"].ToString(),
                                    Tipo_013AL = "Familia"
                                };
                            }
                            lista.Add(permiso);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de permisos del rol.", ex);
            }
            return lista;
        }
        
        public bool EliminarPermisoRol_013AL(int idRol, int idPermiso)
        {
            int filasAfectadas = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("[EliminarPermisoRol-013AL]", con);
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idRol", idRol);
                    cmd.Parameters.AddWithValue("@idPermiso", idPermiso);

                    filasAfectadas = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Error al eliminar el permiso del rol.", ex);
                
            }
            return filasAfectadas > 0;
        }
        public int CrearRol_013AL(string nombreRol)
        {
            int nuevoId = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand comando = new SqlCommand("[AgregarRol-013AL]", con);
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@NombreRol", nombreRol);
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
                throw new Exception("Error al crear el rol", ex);
            }
            return nuevoId;
        }
        public string EliminarRol_013AL(int id)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("[EliminarRol-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo eliminar";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el rol", ex);
            }
            return respuesta;
        }
        public string ModificarRol_013AL(int id, string nombre)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[ModificarRol-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = nombre;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo modificar";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el rol", ex);
            }
            return respuesta;
        }
        public bool VerificarRelacionesRol_013AL(int idRol)
        {
            int count = 0;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM [Rol_Permiso-013AL] WHERE [CodRol-013AL] = @IdRol";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@IdRol", idRol);
                    count = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex) { throw new Exception("Error al verificar relaciones del rol", ex); }
            return count > 0;

        }

        public Dictionary<int, string> ListarRoles_013AL()
        {
            Dictionary<int, string> roles = new Dictionary<int, string>();

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT [CodRol-013AL], [NombreRol-013AL] FROM [Roles-013AL]";
                    using (SqlCommand com = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader dr = com.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                int codRol = Convert.ToInt32(dr["CodRol-013AL"]);
                                string nombreRol = dr["NombreRol-013AL"].ToString();
                                roles.Add(codRol, nombreRol);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar roles", ex);
            }

            return roles;
        }

        public DataTable TraerListaRoles_013AL()
        {
            SqlDataReader resultado;
            DataTable tabla = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("Select * from [Roles-013AL]", con);
                    com.CommandType = CommandType.Text;
                    con.Open();
                    resultado = com.ExecuteReader();
                    tabla.Load(resultado);
                }
            }
            catch (Exception ex) { throw new Exception("Error al listar roles", ex); }
            return tabla;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using BE_013AL;
using Servicios;
using BE_013AL.Composite;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;


namespace DAL_013AL
{
    public class DALConexiones_013AL
    {
        /*SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-QM84P0N\SQLEXPRESS;Initial Catalog=Dietética;Integrated Security=True");
        SqlCommand com;*/

        private readonly string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Dietética;Integrated Security=True";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }

        /*
        private List<Componente_013AL> ObtenerHijos_013AL(int idPadre)
        {
            List<Componente_013AL> hijos = new List<Componente_013AL>();

            string query = @"
        SELECT p.[CodPermiso-013AL], p.[NombrePermiso-013AL], p.[Tipo-013AL]
        FROM [Permisos-013AL] p
        INNER JOIN [Permisos_Componente-013AL] pc 
            ON pc.[cod_permiso_hijo-013AL] = p.[CodPermiso-013AL]
        WHERE pc.[cod_permiso_padre-013AL] = @idPadre";

            try
            {
                using (SqlConnection nuevaConexion = new SqlConnection(con.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, nuevaConexion))
                {
                    cmd.Parameters.AddWithValue("@idPadre", idPadre);
                    nuevaConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int tipo = Convert.ToInt32(dr["Tipo-013AL"]);
                            int cod = Convert.ToInt32(dr["CodPermiso-013AL"]);
                            string nombre = dr["NombrePermiso-013AL"].ToString();

                            Componente_013AL comp;

                            if (tipo == 1) 
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
                            else 
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener hijos del permiso {idPadre}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return hijos;
        }


        




        public int BuscarId_013AL(string nombre)
        {

            int id = 0;
            try
            {
                com = new SqlCommand("[BuscarId-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@nombre", nombre);

                con.Open();
                var result = com.ExecuteScalar();
                if (result != null)
                {
                    id = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ", ex);
            }
            finally
            {
                con.Close();
            }
            return id;
        }

        public int BuscarIdRol_013AL(string nombre)
        {

            int id = 0;
            try
            {
                com = new SqlCommand("[BuscarIdRol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@nombre", nombre);

                con.Open();
                var result = com.ExecuteScalar();
                if (result != null)
                {
                    id = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar rol", ex);
            }
            finally
            {
                con.Close();
            }
            return id;
        }



        public string InsertarFamiliaPatente_013AL(Familia_013AL obj, Permiso_013AL per)
        {
            try
            {
                SqlCommand com = new SqlCommand("[AgregarPermisosFamilia-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@familia", SqlDbType.Int).Value = obj.Cod_013AL;
                com.Parameters.Add("@patente", SqlDbType.Int).Value = per.Cod_013AL;

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

        

        public List<Familia_013AL> ListarFamilias_013AL()
        {
            List<Familia_013AL> Lista = new List<Familia_013AL>();
            try
            {

                SqlCommand com = new SqlCommand("[ListarFamilias-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Lista.Add(new Familia_013AL()
                        {
                            Cod_013AL = Convert.ToInt32(dr["CodPermiso-013AL"]),
                            Nombre_013AL = dr["NombrePermiso-013AL"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar familias", ex);
            }
            finally
            {
                con.Close();
            }
            return Lista;
        }
        public List<Permiso_013AL> ListarPermisos_013AL()
        {
            List<Permiso_013AL> Lista = new List<Permiso_013AL>();
            try
            {

                SqlCommand com = new SqlCommand("[ListarPatentes-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Lista.Add(new Permiso_013AL()
                        {
                            Cod_013AL = Convert.ToInt32(dr["CodPermiso-013AL"]),
                            Nombre_013AL = dr["NombrePermiso-013AL"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar patentes", ex);
            }
            finally
            {
                con.Close();
            }
            return Lista;
        }

        

        public string AgregarFamiliaPadre_013AL(Familia_013AL permiso)
        {
            string respuesta = "";
            try
            {

                SqlCommand com = new SqlCommand("[AgregarFamiliaPadre-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@nombremenu", SqlDbType.NVarChar).Value = permiso.Nombre_013AL;

                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo agregar la familia";

            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }
        
        public string AgregarRol_013AL(Rol_013AL rol)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[AgregarRol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = rol.Nombre_013AL;
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo agregar el rol";
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        

        

        public string ModificarPermisos_013AL(Familia_013AL permiso)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[ModificarPermisos-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = permiso.Cod_013AL;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = permiso.Nombre_013AL;
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo eliminar";
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        

        public string Eliminarpermisosfamilia_013AL(int padre, int hijo)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[Eliminarpermisosfamilia-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@padre", SqlDbType.Int).Value = padre;
                com.Parameters.Add("@hijo", SqlDbType.Int).Value = hijo;

                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo eliminar";
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        public string Eliminarfamiliarol_013AL(int rol, int fam)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[Eliminarfamiliarol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@rol", SqlDbType.Int).Value = rol;
                com.Parameters.Add("@fam", SqlDbType.Int).Value = fam;

                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo eliminar";
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }


        public bool VerificarRolEnRolPermisos_013AL(int idRol)
        {
            bool existe = false;

            try
            {
                SqlCommand com = new SqlCommand("[VerificarRolEnRolPermisos-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idRol", SqlDbType.Int).Value = idRol;
                SqlParameter existeParam = new SqlParameter("@existe", SqlDbType.Bit);
                existeParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(existeParam);

                con.Open();
                com.ExecuteNonQuery();
                existe = Convert.ToBoolean(existeParam.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return existe;
        }

        public bool VerificarPermisosEnRolPermisos_013AL(int idper)
        {
            bool existe = false;

            try
            {
                SqlCommand com = new SqlCommand("[VerificarPermisosEnRolPermisos-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idPermiso", SqlDbType.Int).Value = idper;
                SqlParameter existeParam = new SqlParameter("@existe", SqlDbType.Bit);
                existeParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(existeParam);

                con.Open();
                com.ExecuteNonQuery();
                existe = Convert.ToBoolean(existeParam.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return existe;
        }

        public bool VerificarRolEnUsuario_013AL(int idRol)
        {
            bool existe = false;

            try
            {
                SqlCommand com = new SqlCommand("[VerificarRolEnUsuario-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idRol", SqlDbType.Int).Value = idRol;
                SqlParameter existeParam = new SqlParameter("@existe", SqlDbType.Bit);
                existeParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(existeParam);

                con.Open();
                com.ExecuteNonQuery();
                existe = Convert.ToBoolean(existeParam.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return existe;
        }
        public bool VerificarFamiliaEnPermisosComponente_013AL(int idRol)
        {
            bool existe = false;

            try
            {
                SqlCommand com = new SqlCommand("[VerificarFamiliaEnPermisosComponente-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idfam", SqlDbType.Int).Value = idRol;
                SqlParameter existeParam = new SqlParameter("@existe", SqlDbType.Bit);
                existeParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(existeParam);

                con.Open();
                com.ExecuteNonQuery();
                existe = Convert.ToBoolean(existeParam.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return existe;
        }

        public bool VerificarPermisosFamilia_013AL(int idfam, int idpat)
        {
            bool existe = false;

            try
            {
                SqlCommand com = new SqlCommand("[VerificarPermisosFamilia-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idfam", SqlDbType.Int).Value = idfam;
                com.Parameters.Add("@idpat", SqlDbType.Int).Value = idpat;

                SqlParameter existeParam = new SqlParameter("@existe", SqlDbType.Bit);
                existeParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(existeParam);

                con.Open();
                com.ExecuteNonQuery();
                existe = Convert.ToBoolean(existeParam.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return existe;
        }

        public bool VerificarPermisosRol_013AL(int idrol, int idfam)
        {
            bool existe = false;

            try
            {
                SqlCommand com = new SqlCommand("[VerificarPermisosRol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.Add("@idrol", SqlDbType.Int).Value = idrol;
                com.Parameters.Add("@idfam", SqlDbType.Int).Value = idfam;
                SqlParameter existeParam = new SqlParameter("@existe", SqlDbType.Bit);
                existeParam.Direction = ParameterDirection.Output;
                com.Parameters.Add(existeParam);

                con.Open();
                com.ExecuteNonQuery();
                existe = Convert.ToBoolean(existeParam.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return existe;
        }




        List<Rol_013AL> ListarPermisos_013AL(string dni)
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




        public void RegistrarHijos_013AL(int permisoPadre, int permisoHijo)
        {

            SqlCommand comando = new SqlCommand("[RegistrarHijosFamilia-013AL]", con);
                
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@PermisoPadre", permisoPadre);
                    comando.Parameters.AddWithValue("@PermisoHijo", permisoHijo);

                    try
                    {
                        con.Open();
                        comando.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al registrar la relación entre familia y permiso", ex);
                    }
                    finally {con.Close();}
                
        }

        


        public string EliminarHijos_013AL(int id)
        {
            string respuesta = "";
            try
            {
                SqlCommand com = new SqlCommand("[EliminarHijos-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@PermisoPadre", SqlDbType.Int).Value = id;
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo eliminar hijos";
            }
            catch (Exception ex) { }
            finally {  con.Close(); }
            return respuesta;

        }

        

        public Componente_013AL VerificarSiEstaEnFamilia_013AL(int idHijo)
        {
            Componente_013AL componente = null;
            try
            {
                SqlCommand command = new SqlCommand("[VerificarSiEstaEnFamilia-013AL]", con);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idHijo", idHijo);

                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) 
                    {
                        componente = new Familia_013AL()
                        {
                            Cod_013AL = reader.GetInt32(0),
                            Nombre_013AL = reader.GetString(2),
                            Tipo_013AL = reader.GetInt32(3).ToString()
                        };
                    }
                }
            }catch(Exception ex) { }
            finally { con.Close(); }
            
            return componente;
            
        }

        

        public void EliminarPermisosRol_013AL(int idRol)
        {
            
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[EliminarPermisosRol-013AL]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdRol", idRol);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar los permisos del rol.", ex);
                }
            finally { con.Close(); }
            
        }

        

        public List<Componente_013AL> TraerListaPermisosRolSegunPermiso_013AL(int idPermiso)
        {
            List<Componente_013AL> lista = new List<Componente_013AL>();

            
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[TraerListaPermisosRolSegunPermiso-013AL]", con);
                    
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodPermiso", idPermiso);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Componente_013AL rol = new Familia_013AL()
                                {
                                    Cod_013AL = Convert.ToInt32(reader["CodRol-013AL"])
                                };
                                lista.Add(rol);
                            }
                        }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de roles según el permiso.", ex);
                }
            finally{ con.Close(); }

            return lista;
        }
        public DataTable TraerListaPermisosHijo_013AL()
        {
            SqlDataReader resultado;
            DataTable tabla = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("SELECT * FROM [Permisos-013AL]", con);
                com.CommandType = CommandType.Text;
                con.Open();
                resultado = com.ExecuteReader();
                tabla.Load(resultado);
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return tabla;
        }

       

        

        

        

        
        
        
        
        */


        


    }
}

  




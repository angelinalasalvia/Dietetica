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
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-QM84P0N\SQLEXPRESS;Initial Catalog=Dietética;Integrated Security=True");
        SqlCommand com;

       

        public List<Usuarios_013AL> Listar_013AL()
        {
            List<Usuarios_013AL> Lista = new List<Usuarios_013AL>();
            try
            {
                string query = @"SELECT [Login-013AL], [Contraseña-013AL], [DNI-013AL], 
                                [Bloqueo-013AL], [CodRol-013AL], [Activo-013AL], [Eliminado-013AL] 
                         FROM [Usuario-013AL]";

                com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;

                con.Open();


                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int idRol = Convert.ToInt32(dr["CodRol-013AL"]);
                        Rol_013AL rol = new Rol_013AL { CodRol_013AL = idRol };
                        Lista.Add(new Usuarios_013AL()
                        {
                            Login_013AL = dr["Login-013AL"].ToString(),
                            Contraseña_013AL = dr["Contraseña-013AL"].ToString(),
                            //ID = Convert.ToInt32(dr["Id"]),
                            DNI_013AL = dr["DNI-013AL"].ToString(),
                            Bloqueo_013AL = (bool)dr["Bloqueo-013AL"],
                            Rol_013AL = rol,
                            Activo_013AL = (bool)dr["Activo-013AL"],
                            Eliminado_013AL = (bool)dr["Eliminado-013AL"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar usuarios", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }

        public string ObtenerContraseñaActual_013AL(string email)
        {
            string contraseña = null;
            try {

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
            catch (Exception ex)
            {
                throw ex;
            }
            finally { con.Close(); }
            return contraseña;
        }

       

        public string CambiarContraseña_013AL(Usuarios_013AL usuarios)
        {
            string resultado;
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
            return resultado;
        }
        
        public string BloquearUsuario_013AL(Usuarios_013AL usuario)
        {
            string resultado;
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return resultado;
        }


        public string AgregarUsuario_013AL(Usuarios_013AL usuario)
        {
            string respuesta = "";

            try
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
                    com.Parameters.AddWithValue("@rol", usuario.Rol_013AL);
                    com.Parameters.AddWithValue("@dni", usuario.DNI_013AL);
                    com.Parameters.AddWithValue("@login", usuario.Login_013AL);
                    com.Parameters.AddWithValue("@bloqueo", usuario.Bloqueo_013AL);
                    com.Parameters.AddWithValue("@activo", usuario.Activo_013AL);

                    con.Open();
                    int filas = com.ExecuteNonQuery();
                    respuesta = filas == 1 ? "Usuario agregado correctamente" : "No se pudo agregar el usuario";
                }
            }
            catch (Exception ex)
            {
                respuesta = "Error: " + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return respuesta;
        }



        public DataTable ListarUsuarios_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            string query = @"Select * from [Usuario-013AL] order by [DNI-013AL] asc";

            com = new SqlCommand(query, con);
            com.CommandType = CommandType.Text;
            con.Open();
            resultado = com.ExecuteReader();
            dt.Load(resultado);
            return dt;
        }

        public DataSet ListarUsuariosActivos_013AL()
        {
            DataSet ds = new DataSet();

            string query = "SELECT * FROM [Usuario-013AL] WHERE [Activo-013AL] = 1 AND [Eliminado-013AL] = 0";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds, "Usuario");
                }
            }

            return ds;
        }



        public string DesbloquearUsuario_013AL(Usuarios_013AL usuario)
        {
            string resultado;
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return resultado;
        }

        
        public DataSet ObtenerUsuarios_013AL()
        {
            DataSet ds = new DataSet();

            string query = "Select * from [Usuario-013AL] order by [DNI-013AL] asc";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;


                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds, "Usuario");
                }
            }

            return ds;
        }


        public DataSet ObtenerClientes_013AL()
        {

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [Cliente-013AL]", con);
            DataSet ds = new DataSet();
            da.Fill(ds, "Cliente");

            return ds;

        }


        public void GuardarUsuarios_013AL(string NombreTabla, DataSet Dset)
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
                            
                            throw new Exception("El DNI ya está registrado.");
                        }
                    }

                    
                    string insertQuery = @"
                INSERT INTO [Usuario-013AL] 
                ([Mail-013AL], [Contraseña-013AL], [Nombres-013AL], [Apellidos-013AL], [CodRol-013AL], [DNI-013AL], [Login-013AL], [Bloqueo-013AL], [Activo-013AL], [Eliminado-013AL])
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
                    string deleteQuery = "DELETE FROM [Usuario-013AL] WHERE [DNI-013AL] = @dni";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        string dni = row["DNI-013AL", DataRowVersion.Original].ToString();
                        cmd.Parameters.AddWithValue("@dni", dni);

                       
                        cmd.ExecuteNonQuery();
                       
                    }
                }
            }
            con.Close();

            Dset.AcceptChanges();
        }


        public void GuardarClientes_013AL(string NombreTabla, DataSet Dset)
        {
            SqlDataAdapter da = new SqlDataAdapter(("SELECT * FROM [Cliente-013AL]" /*+ NombreTabla*/), con);
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            da.UpdateCommand = cb.GetUpdateCommand();
            da.DeleteCommand = cb.GetDeleteCommand();
            da.InsertCommand = cb.GetInsertCommand();
            da.ContinueUpdateOnError = true;
            da.Fill(Dset);
            da.Update(Dset.Tables[0]);
        }


        //NEGOCIO

        public List<Factura_013AL> ListarFactura_013AL()
        {
            List<Factura_013AL> Lista = new List<Factura_013AL>();
            try
            {
                string query = "Select [CodCompra-013AL], [CUILCliente-013AL], [Fecha-013AL], [IVA-013AL], [MetodoPago-013AL] from [Factura-013AL]";
                com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Lista.Add(new Factura_013AL()
                        {
                            CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]),
                            CUIL_013AL = Convert.ToInt32(dr["CUILCliente-013AL"]),
                            Fecha_013AL = Convert.ToDateTime(dr["Fecha-013AL"]),
                            IVA_013AL = Convert.ToInt32(dr["IVA-013AL"]),
                            MetPago_013AL = Convert.ToString(dr["MetodoPago-013AL"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }
       
        

            public string ObtenerNombreProducto_013AL(int idProducto)
            {
                string nombre = null;
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT [Nombre-013AL] FROM [Producto-013AL] WHERE [CodProducto-013AL] = @id", con);
                cmd.Parameters.AddWithValue("@id", idProducto);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    nombre = reader["Nombre-013AL"].ToString();
                }

                reader.Close();


            }
            catch (Exception ex) { throw new Exception("Error al realizar la operación: ", ex); }
            finally
            {
                con.Close();
            }
                return nombre;
            }
        
        public string AgregarCompraCliente_013AL(Factura_013AL cc)
        {
            string respuesta = "";
            try{
            com = new SqlCommand("AgregarCompraCliente-013AL", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@idc", SqlDbType.Int).Value = cc.CodCompra_013AL;
            com.Parameters.Add("@cuil", SqlDbType.Int).Value = cc.CUIL_013AL;
            com.Parameters.Add("@metpago", SqlDbType.NVarChar).Value = cc.MetPago_013AL;
            con.Open();
            respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "error";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al realizar la operación: ", ex);
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        public List<Detalle_013AL> ListarCompraProducto_013AL()
        {
            List<Detalle_013AL> Lista = new List<Detalle_013AL>();
            try
            {
                string query = "Select [CodCompra-013AL], [CodProducto-013AL], [Cantidad-013AL], [PrecioUnitario-013AL] from [Detalle-013AL]";
                com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Lista.Add(new Detalle_013AL()
                        {
                            CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]),
                            CodProducto_013AL = Convert.ToInt32(dr["CodProducto-013AL"]),
                            Cantidad_013AL = Convert.ToInt32(dr["Cantidad-013AL"]),
                            PrecioUnitario_013AL = Convert.ToInt32(dr["PrecioUnitario-013AL"])

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }
        
        public string ActualizarCantidadCompraProducto_013AL(int codCompra, int codProducto, int nuevaCantidad)
        {
            try {
                
                    string query = "UPDATE [Detalle-013AL] SET [Cantidad-013AL] = @nuevaCantidad WHERE [CodCompra-013AL] = @codCompra AND [CodProducto-013AL] = @codProducto";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nuevaCantidad", nuevaCantidad);
                    cmd.Parameters.AddWithValue("@codCompra", codCompra);
                    cmd.Parameters.AddWithValue("@codProducto", codProducto);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0 ? "Actualizado" : "Error";
               
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la cantidad del producto en la compra", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void ActualizarStockProducto_013AL(int idProducto, int nuevoStock)
        {
            try
            {
                string query = "UPDATE [Producto-013AL] SET [Stock-013AL] = @valor WHERE [CodProducto-013AL] = @id";

                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-QM84P0N\SQLEXPRESS;Initial Catalog=Dietética;Integrated Security=True"))
                {
                    using (SqlCommand com = new SqlCommand(query, con))
                    {
                        
                        com.CommandType = CommandType.Text;
                        com.Parameters.AddWithValue("@id", idProducto);
                        com.Parameters.AddWithValue("@valor", nuevoStock);

                        con.Open();
                        com.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el stock del producto en la base de datos", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public string EliminarDetalle_013AL(int id)
        {
            string respuesta = "";
            SqlCommand com = new SqlCommand("EliminarDetalle-013AL", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@id", SqlDbType.Int).Value = id;

            try
            {
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "error";
            }
            catch (Exception ex)
            {
                respuesta = "Error: " + ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return respuesta;
        }
        public string AgregarCompraProducto_013AL(Detalle_013AL cp)
        {
            string respuesta = "";
            try { 
            com = new SqlCommand("AgregarCompraProducto-013AL", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@idc", SqlDbType.Int).Value = cp.CodCompra_013AL;
            com.Parameters.Add("@idp", SqlDbType.Int).Value = cp.CodProducto_013AL;
            com.Parameters.Add("@cant", SqlDbType.Int).Value = cp.Cantidad_013AL;
            com.Parameters.Add("@pu", SqlDbType.Int).Value = cp.PrecioUnitario_013AL;
            con.Open();
            respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "error";
            }catch (Exception ex)
            {
                throw new Exception("Error al realizar la operación: ", ex);
            }
            finally
            {
                    con.Close();
            }
            return respuesta;
        }

        public string RegistrarVentaCompleta_013AL(Factura_013AL factura, List<Detalle_013AL> detalles)
        {
            
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    
                    SqlCommand cmdFactura = new SqlCommand("AgregarCompraCliente-013AL", con, transaction);
                    cmdFactura.CommandType = CommandType.StoredProcedure;
                    cmdFactura.Parameters.AddWithValue("@idc", factura.CodCompra_013AL);
                    cmdFactura.Parameters.AddWithValue("@cuil", factura.CUIL_013AL);
                    cmdFactura.Parameters.AddWithValue("@metpago", factura.MetPago_013AL);
                    cmdFactura.ExecuteNonQuery();

                    
                    foreach (var detalle in detalles)
                    {
                        SqlCommand cmdDetalle = new SqlCommand("AgregarCompraProducto-013AL", con, transaction);
                        cmdDetalle.CommandType = CommandType.StoredProcedure;
                        cmdDetalle.Parameters.AddWithValue("@idc", detalle.CodCompra_013AL);
                        cmdDetalle.Parameters.AddWithValue("@idp", detalle.CodProducto_013AL);
                        cmdDetalle.Parameters.AddWithValue("@cant", detalle.Cantidad_013AL);
                        cmdDetalle.Parameters.AddWithValue("@pu", detalle.PrecioUnitario_013AL);
                        cmdDetalle.ExecuteNonQuery();

                        
                        SqlCommand cmdStock = new SqlCommand(
                            "UPDATE [Producto-013AL] SET [Stock-013AL] = [Stock-013AL] - @cant WHERE [CodProducto-013AL] = @idp",
                            con, transaction);
                        cmdStock.Parameters.AddWithValue("@cant", detalle.Cantidad_013AL);
                        cmdStock.Parameters.AddWithValue("@idp", detalle.CodProducto_013AL);
                        cmdStock.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al registrar la venta completa: ", ex);
                }
          
        }
        public void DescontarStock_013AL(int idProducto, int cantidad)
        {
            string query = "UPDATE [Producto-013AL] SET [Stock-013AL] = [Stock-013AL] - @cant WHERE [CodProducto-013AL] = @id";
            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-QM84P0N\SQLEXPRESS;Initial Catalog=Dietética;Integrated Security=True"))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@cant", cantidad);
                        cmd.Parameters.AddWithValue("@id", idProducto);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al descontar stock: ", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public void RevertirStock_013AL(int idProducto, int cantidad)
        {
            string query = "UPDATE [Producto-013AL] SET [Stock-013AL] = [Stock-013AL] + @cant WHERE [CodProducto-013AL] = @id";
            try {
            using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-QM84P0N\SQLEXPRESS;Initial Catalog=Dietética;Integrated Security=True"))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@cant", cantidad);
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            } 
            }
            catch (Exception ex)
            {
                throw new Exception("Error al revertir stock: ", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

        }


        public string AgregarCliente_013AL(Cliente_013AL obj)
        {
            string respuesta = "";
            try{ 
            SqlCommand com = new SqlCommand("AgregarCliente-013AL", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = obj.Nombre_013AL;
            com.Parameters.Add("@apellido", SqlDbType.NVarChar).Value = obj.Apellido_013AL;
            com.Parameters.Add("@cuil", SqlDbType.Int).Value = obj.CUIL_013AL;
            com.Parameters.Add("@domicilio", SqlDbType.NVarChar).Value = obj.Domicilio_013AL;
            com.Parameters.Add("@mail", SqlDbType.NVarChar).Value = obj.Mail_013AL;
            com.Parameters.Add("@tel", SqlDbType.Int).Value = obj.Telefono_013AL;
            con.Open();
            respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo registrar el cliente";
            }catch(Exception ex){ throw new Exception("Error al realizar la operación: ", ex); }
            finally { con.Close(); }
            return respuesta;
        }


        public List<Cliente_013AL> BuscarCliente_013AL()
        {
            List<Cliente_013AL> Lista = new List<Cliente_013AL>();
            try
            {

                string query = "SELECT [Nombre-013AL], [Apellido-013AL], [CUIL-013AL], [Domicilio-013AL], [Mail-013AL], [Telefono-013AL] FROM [Cliente-013AL]";
                com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Lista.Add(new Cliente_013AL()
                        {
                            
                            Nombre_013AL = dr["Nombre-013AL"].ToString(),
                            Apellido_013AL = dr["Apellido-013AL"].ToString(),
                            CUIL_013AL = Convert.ToInt32(dr["CUIL-013AL"]),
                            Domicilio_013AL = dr["Domicilio-013AL"].ToString(),
                            Mail_013AL = dr["Mail-013AL"].ToString(),
                            Telefono_013AL = Convert.ToInt32(dr["Telefono-013AL"])

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar clientes", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }

        public Cliente_013AL BuscarClientePorCUIL_013AL(int cuil)
        {
            Cliente_013AL cliente = null;

            try
            {
                string query = "SELECT [Nombre-013AL], [Apellido-013AL], [CUILCliente-013AL], [Domicilio-013AL], [Mail-013AL], [Telefono-013AL] FROM [Cliente-013AL] WHERE [CUILCliente-013AL] = @CUIL";
                com = new SqlCommand(query, con);
                com.Parameters.AddWithValue("@CUIL", cuil);
                com.CommandType = CommandType.Text;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente = new Cliente_013AL()
                        {
                            Nombre_013AL = dr["Nombre-013AL"].ToString(),
                            Apellido_013AL = dr["Apellido-013AL"].ToString(),
                            CUIL_013AL = Convert.ToInt32(dr["CUILCliente-013AL"]),
                            Domicilio_013AL = dr["Domicilio-013AL"].ToString(),
                            Mail_013AL = dr["Mail-013AL"].ToString(),
                            Telefono_013AL = Convert.ToInt32(dr["Telefono-013AL"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar cliente por CUIL", ex);
            }
            finally
            {
                con.Close();
            }

            return cliente;
        }

        public DataTable BuscarProductoPorId_013AL(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("BuscarProductoPorId-013AL", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idl", SqlDbType.Int).Value = id;

                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                dt.Load(reader);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al realizar la operación: ", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable ListaProductos_013AL()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conNueva = new SqlConnection(con.ConnectionString))
                {
                    using (SqlCommand com = new SqlCommand("ListaProductos-013AL", conNueva))
                    {
                        com.CommandType = CommandType.StoredProcedure;
                        conNueva.Open();
                        using (SqlDataReader reader = com.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al realizar la operación: ", ex);
            }

            return dt;
        }



        public int ObtenerUltimoIdCompra_013AL()
        {
            int ultimoId = 0; 
            try
            {
                string query = "SELECT ISNULL(MAX([CodCompra-013AL]), 0) AS UltimoId FROM [Detalle-013AL]";

                SqlCommand command = new SqlCommand(query, con);
                con.Open();

                object resultado = command.ExecuteScalar();
                if (resultado != null)
                {
                    ultimoId = Convert.ToInt32(resultado);
                }
            }
            catch (Exception ex) { throw new Exception("Error al realizar la operación: ", ex); }
            finally { con.Close(); }
            return ultimoId;
        }
       

        public DataTable BuscarProductoxNombre_013AL(string valor)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("BuscarProductoxNombre-013AL", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@valor", SqlDbType.NVarChar).Value = valor;
                con.Open();
                resultado = com.ExecuteReader();
                dt.Load(resultado);
                
            }
            catch (Exception ex) { throw new Exception("Error al realizar la operación: ", ex); }
            finally { con.Close(); }
            return dt;
        }


        /*public string ModificarStock(Producto obj)
        {
            string resultado = "";
            try
            {
                com = new SqlCommand("ModificarStock", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdProducto;
                com.Parameters.Add("@valor", SqlDbType.Int).Value = obj.Stock;
                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "ERROR";
            }
            catch (Exception ex)
            {
                // Manejar excepción
            }
            finally
            {
                con.Close();
            }

            return resultado;
        }*/

        //COMPOSITE
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

                            if (tipo == 1) // Familia
                            {
                                Familia_013AL familia = new Familia_013AL()
                                {
                                    Cod_013AL = cod,
                                    Nombre_013AL = nombre,
                                    Tipo_013AL = "1"
                                };

                                // ⚠ Importante: Llamada recursiva segura porque usa NUEVA conexión
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener hijos del permiso {idPadre}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return hijos;
        }


        public List<Componente_013AL> ListarPermisos_013AL(string dni)
        {
            List<Componente_013AL> permisos = new List<Componente_013AL>();
            try
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

                            Componente_013AL comp;

                            if (tipo == 1) // Familia
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
                            else // Permiso
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
            catch (Exception ex)
            {
                //permisos = new List<Componente_013AL>();
                MessageBox.Show($"Error al listar permisos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                permisos = new List<Componente_013AL>();
            }
            finally { con.Close(); }

            return permisos;
        }



        public string ObtenerNombrePermiso_013AL(int idPermiso)
        {
            string nombrePermiso = string.Empty;
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT [NombrePermiso-013AL] FROM [Permisos-013AL] WHERE [CodPermiso-013AL] = @idPermiso");

                SqlCommand com = new SqlCommand(query.ToString(), con);
                com.Parameters.AddWithValue("@idPermiso", idPermiso);
                com.CommandType = CommandType.Text;
                con.Open();

                object result = com.ExecuteScalar();
                if (result != null)
                {
                    nombrePermiso = result.ToString();
                }
            }
            catch (Exception ex)
            {
                nombrePermiso = string.Empty;
            }
            finally
            {
                con.Close();
            }

            return nombrePermiso;
        }



        //PERMISOS

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

        public string InsertarFamiliaRol_013AL(int rol, int permiso)
        {
            try
            {
                SqlCommand com = new SqlCommand("[AgregarFamiliaRol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@rol", SqlDbType.Int).Value = rol;
                com.Parameters.Add("@familia", SqlDbType.Int).Value = permiso;


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

        public List<Rol_013AL> ListarRoles_013AL()
        {
            List<Rol_013AL> Lista = new List<Rol_013AL>();
            try
            {
                string query = "SELECT [CodRol-013AL], [NombreRol-013AL] FROM [Roles-013AL]";
                com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Lista.Add(new Rol_013AL()
                        {
                            CodRol_013AL = Convert.ToInt32(dr["CodRol-013AL"]),
                            Nombre_013AL = dr["NombreRol-013AL"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar roles", ex);
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
        /*
         public List<Permiso> ListarHijos(int idFamilia)
         {
             List<Permiso> listaHijos = new List<Permiso>();
             try
             {
                 SqlCommand com = new SqlCommand("ListarHijos", con);
                 com.CommandType = CommandType.StoredProcedure;
                 com.Parameters.Add("@idFamilia", SqlDbType.Int).Value = idFamilia;
                 con.Open();

                 using (SqlDataReader dr = com.ExecuteReader())
                 {
                     while (dr.Read())
                     {
                         TipoPermiso tipo = (TipoPermiso)Convert.ToInt32(dr["TipoPermiso"]);
                         switch (tipo)
                         {
                             case TipoPermiso.Familia:
                                 listaHijos.Add(new Familia
                                 {
                                     Id = Convert.ToInt32(dr["Id"]),
                                     Nombre = dr["NombreMenu"].ToString()
                                 });
                                 break;
                             default:
                                 listaHijos.Add((Permiso)new Componente
                                 {
                                     Id = Convert.ToInt32(dr["Id"]),
                                     Nombre = dr["NombreMenu"].ToString()
                                 });
                                 break;
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 throw new Exception("Error", ex);
             }
             finally
             {
                 con.Close();
             }
             return listaHijos;
         }
         */
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

        

        public string EliminarRol_013AL(int id)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[EliminarRol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public string ModificarRol_013AL(Rol_013AL rol)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[ModificarRol-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = rol.CodRol_013AL;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = rol.Nombre_013AL;
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

        /*public string BuscarCliente(int valor)
        {
            string respuesta = "";
            SqlCommand com = new SqlCommand("BuscarCliente", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@valor", SqlDbType.Int).Value = valor;
            respuesta = com.ExecuteNonQuery()== 1 ? "OK" : "No se encontró el cliente";
            return respuesta;
        }*/


        /*public Array GetAllPermission()
        {
            return Enum.GetValues(typeof(TipoPermiso));
        }
        public Familia GuardarComponente(Familia p, bool esfamilia)
        {
            try
            {
                con.Open();
                var cmd = new SqlCommand();
                cmd.Connection = con;

                var sql = $@"insert into permiso (nombre,permiso) values (@nombre,@permiso);  SELECT ID AS LastID FROM permiso WHERE ID = @@Identity;       ";

                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("nombre", p.Nombre));


                if (esfamilia)
                    cmd.Parameters.Add(new SqlParameter("permiso", DBNull.Value));

                else
                    cmd.Parameters.Add(new SqlParameter("permiso", p.Familia.ToString()));

                var id = cmd.ExecuteScalar();
                p.Id = (int)id;
                return p;
            }
            catch (Exception e)
            {
                throw e;
            }
        }*/
        // Bitácora

        public string AgregarEvento_013AL(Eventos evento)
        {
            string respuesta = "";
            try
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
        public DataTable ListarEventos_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {

                SqlCommand com = new SqlCommand("[ListarEventos-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                con.Open();
                resultado = com.ExecuteReader();
                dt.Load(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable ConsultasEventos_013AL(string login, DateTime? fechaInicio, DateTime? fechaFin, string modulo, string evento, int? criticidad)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable ConsultaProductosC_013AL(int? idp, string nombre, DateTime? fechaInicio, DateTime? fechaFin)
        {
            DataTable dt = new DataTable();
            SqlDataReader resultado;
            try
            {
                SqlCommand cmd = new SqlCommand("[consultacambios-013AL]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                
                if (idp == null)
                    cmd.Parameters.Add("@idp", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@idp", SqlDbType.Int).Value = idp;

                
                if (string.IsNullOrEmpty(nombre))
                    cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 50).Value = nombre;

                
                if (fechaInicio == null)
                    cmd.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = fechaInicio;

                
                if (fechaFin == null)
                    cmd.Parameters.Add("@fechaFin", SqlDbType.Date).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@fechaFin", SqlDbType.Date).Value = fechaFin;

                con.Open();
                resultado = cmd.ExecuteReader();
                dt.Load(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar los cambios de productos", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable ObtenerNombreApellidoPorLogin_013AL(string login)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("SELECT [Nombres-013AL], [Apellidos-013AL] FROM [Usuario-013AL] WHERE [Login-013AL] = @login", con);
                com.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;

                con.Open();
                SqlDataReader resultado = com.ExecuteReader();
                dt.Load(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener nombre y apellido", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable ListarProductosC_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("SELECT * FROM [ProductoC-013AL]", con);
                //com.CommandType = CommandType.StoredProcedure;
                con.Open();
                resultado = com.ExecuteReader();
                dt.Load(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public void ActivarProductoC_013AL(int idProductoC)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("[ActivarProductoC-013AL]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@IdProductoC", SqlDbType.Int).Value = idProductoC;

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al activar el producto", ex);
            }
            finally
            {
                con.Close();
            }
        }

        /*public DataTable ConsultasEventos(string login, DateTime fecha, string modulo, string evento, int criticidad)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
            SqlCommand com = new SqlCommand("consultaseventos", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@login", SqlDbType.NVarChar, 50).Value = login;
            com.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha;
            com.Parameters.Add("@modulo", SqlDbType.NVarChar, 50).Value = modulo;
            com.Parameters.Add("@evento", SqlDbType.NVarChar, 50).Value = evento;
            com.Parameters.Add("@criticidad", SqlDbType.Int).Value = criticidad;
            con.Open();
            resultado = com.ExecuteReader();
            dt.Load(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }*/
        public void RealizarBackup_013AL(string backupPath)
        {
            try
            {
                string nombreArchivo = $"MiSistema.bak";
                string rutaCompleta = System.IO.Path.Combine(backupPath, nombreArchivo);
                string comandoBackup = $"BACKUP DATABASE Dietética TO DISK='{rutaCompleta}'";
                //.BCK_  .BCK.bak

                SqlCommand cmd = new SqlCommand(comandoBackup, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { }
            finally { con.Close(); }
        }

        public void RealizarRestore_013AL(string backupFilePath)
        {
            try
            {

                con.Open();

                using (SqlCommand setMaster = new SqlCommand("USE master;", con))
                {
                    setMaster.ExecuteNonQuery();
                }

                using (SqlCommand setSingleUser = new SqlCommand("ALTER DATABASE Dietética SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", con))
                {
                    setSingleUser.ExecuteNonQuery();
                }

                string query = $"RESTORE DATABASE Dietética FROM DISK = '{backupFilePath}' WITH REPLACE;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand setMultiUser = new SqlCommand("ALTER DATABASE Dietética SET MULTI_USER;", con))
                {
                    setMultiUser.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {

            }
            finally { con.Close(); }
        }

        /*public void SerializarUsuarios(List<Usuarios> usuarios, string rutaArchivo)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Usuarios>));
            using (StreamWriter writer = new StreamWriter(rutaArchivo))
            {
                serializer.Serialize(writer, usuarios);
            }
        }

        public List<Usuarios> DeserializarUsuarios(string rutaArchivo)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Usuarios>));
            using (StreamReader reader = new StreamReader(rutaArchivo))
            {
                return (List<Usuarios>)serializer.Deserialize(reader);
            }
        }*/

        //COMPRA

        public List<Producto_013AL> ListarProductosPocoStock_013AL()
        {
            List<Producto_013AL> Lista = new List<Producto_013AL>();
            try
            {

                com = new SqlCommand("[ListarProductosPocoStock-013AL]", con);
                com.CommandType = CommandType.Text;
                con.Open();


                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        Lista.Add(new Producto_013AL()
                        {
                            Nombre_013AL = dr["Nombre-013AL"].ToString(),
                            Stock_013AL = Convert.ToInt32(dr["Stock-013AL"].ToString()),
                            CodProducto_013AL = Convert.ToInt32(dr["CodProducto-013AL"].ToString())

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }

        public List<Proveedor_013AL> ListarProveedores_013AL()
        {
            List<Proveedor_013AL> Lista = new List<Proveedor_013AL>();
            try
            {
                string query = "SELECT [NombreProveedor-013AL], [ApellidoProveedor-013AL], [RazonSocial-013AL], [CUIT-013AL] FROM [Proveedor-013AL]";
                com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;
                con.Open();


                using (SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        Lista.Add(new Proveedor_013AL()
                        {
                            NombreProveedor_013AL = dr["NombreProveedor-013AL"].ToString(),
                            ApellidoProveedor_013AL = dr["ApellidoProveedor-013AL"].ToString(),
                            RazonSocial_013AL = dr["RazonSocial-013AL"].ToString(),
                            CUIT_013AL = Convert.ToInt32(dr["CUIT-013AL"].ToString())


                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar proveedores", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }

        public bool VerificarCuit_013AL(int cuit)
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM [Proveedores-013AL] WHERE [CUIT-013AL] = @Cuit";

            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-QM84P0N\SQLEXPRESS;Initial Catalog=Dietética;Integrated Security=True"))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Cuit", cuit);
                        con.Open();
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                //return "Error al verificar el CUIT: " + ex.Message;
            }
            finally { con.Close(); }

            return count > 0;
        }



        public string PreregistrarProveedor_013AL(Proveedor_013AL proveedor)
        {
            string respuesta = "";
            try
            {
                SqlCommand com = new SqlCommand("[PreregistrarProveedor-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = proveedor.NombreProveedor_013AL;
                com.Parameters.Add("@cuit", SqlDbType.Int).Value = proveedor.CUIT_013AL;
                com.Parameters.Add("@razonsocial", SqlDbType.NVarChar).Value = proveedor.RazonSocial_013AL;
                con.Open();
                int resultado = com.ExecuteNonQuery();
                if (resultado > 0)
                    respuesta = "Proveedor preregistrado correctamente.";
                else
                    respuesta = "No se pudo preregistrar al proveedor.";
            }
            catch (SqlException ex) 
            {
                if (ex.Number == 2627 || ex.Number == 2601) // 2627 = Clave duplicada, 2601 = Índice único violado
                {
                    respuesta = "El CUIT ingresado ya está registrado.";
                }
                else
                {
                    respuesta = "Error al preregistrar proveedor: " + ex.Message;
                }

            }
            finally { con.Close(); }
            return respuesta;
        }

        /*public string AgregarSCotizacion(SolicitudCotizacion obj)
        {
            string respuesta = "";
            try
            {
                SqlCommand com = new SqlCommand("AgregarSCotizacion", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@valor", SqlDbType.Int).Value = obj.CUITProveedor;
                
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo AgregarSCotizacion";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return respuesta;
        }*/

        public int AgregarSCotizacion_013AL(SolicitudCotizacion_013AL obj)
        {
            int idSolicitud = 0;
            try
            {
                SqlCommand com = new SqlCommand("[AgregarSCotizacion-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@valor", SqlDbType.Int).Value = obj.CUITProveedor_013AL;

                com.Parameters.Add("@IdSolicitudCotizacion", SqlDbType.Int).Direction = ParameterDirection.Output;

                con.Open();
                com.ExecuteNonQuery();


                idSolicitud = Convert.ToInt32(com.Parameters["@IdSolicitudCotizacion"].Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar la solicitud de cotización", ex);
            }
            finally
            {
                con.Close();
            }
            return idSolicitud;
        }

        public string AgregarDetalleSC_013AL(DetalleSolicitudC_013AL obj)
        {
            string respuesta = "";
            try
            {
                SqlCommand com = new SqlCommand("[AgregarDetalleSC-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@sc", SqlDbType.Int).Value = obj.CodSCotizacion_013AL;
                com.Parameters.Add("@prod", SqlDbType.Int).Value = obj.CodProducto_013AL;
                com.Parameters.Add("@cant", SqlDbType.Int).Value = obj.Cantidad_013AL;
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "No se pudo AgregarDetalleSC";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return respuesta;
        }

        public Producto_013AL ObtenerProductoPorId_013AL(int idProducto)
        {
            Producto_013AL producto = null;
            try
            {
                string query = "SELECT [CodProducto-013AL], [Nombre-013AL], [Stock-013AL], [Precio-013AL] FROM [Producto-013AL] WHERE [CodProducto-013AL] = @IdProducto";

                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@IdProducto", idProducto);

                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        producto = new Producto_013AL
                        {
                            CodProducto_013AL = (int)reader["CodProducto-013AL"],
                            Nombre_013AL = reader["Nombre-013AL"].ToString(),
                            Stock_013AL = (int)reader["Stock-013AL"],
                            Precio_013AL = (int)reader["Precio-013AL"]
                        };
                    }
                }
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return producto;
        }

        public DetalleSolicitudC_013AL ListarProductosOC_013AL(int codsc)
        {
            DetalleSolicitudC_013AL detalleProducto = null;
            try
            {
                SqlCommand command = new SqlCommand("[ListarProductosOC-013AL]", con);
                command.CommandType = CommandType.StoredProcedure; // Asegúrate de que sea un procedimiento almacenado
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
            catch (Exception ex)
            {
                // Manejo de errores
            }
            finally
            {
                con.Close();
            }
            return detalleProducto;
        }

        public string AgregarProducto_013AL(Producto_013AL obj)
        {
            string respuesta = "";
            try
            {
                SqlCommand com = new SqlCommand("[AgregarProducto-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = obj.Nombre_013AL;
                com.Parameters.Add("@stock", SqlDbType.Int).Value = obj.Stock_013AL;
                com.Parameters.Add("@precio", SqlDbType.Int).Value = obj.Precio_013AL;
                com.Parameters.Add("@imagen", SqlDbType.Image).Value = obj.Imagen_013AL;
                com.Parameters.Add("@desc", SqlDbType.NVarChar).Value = obj.Descripcion_013AL;
                con.Open();
                respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return respuesta;
        }

        public DataTable ObtenerProductosConImagen_013AL()
        {
            string query = "SELECT [CodProducto-013AL], [Nombre-013AL], [Stock-013AL], [Precio-013AL], [Imagen-013AL], [Descripcion-013AL] FROM [Producto-013AL] WHERE [Bit_Lo_Bo-013AL] = 1";
            using (SqlCommand command = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public string ModificarProducto_013AL(Producto_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[ModificarProducto_013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = obj.CodProducto_013AL;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = obj.Nombre_013AL;
                com.Parameters.Add("@stock", SqlDbType.Int).Value = obj.Stock_013AL;
                com.Parameters.Add("@precio", SqlDbType.Int).Value = obj.Precio_013AL;
                com.Parameters.Add("@imagen", SqlDbType.Image).Value = obj.Imagen_013AL;
                com.Parameters.Add("@desc", SqlDbType.NVarChar).Value = obj.Descripcion_013AL;
                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }

        public string EliminarProducto_013AL(Producto_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[EliminarProducto-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = obj.CodProducto_013AL;

                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }
        public string EliminarDetalleSC_013AL(DetalleSolicitudC_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[EliminarDetalleSC-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@idp", SqlDbType.Int).Value = obj.CodProducto_013AL;
                com.Parameters.Add("@codsc", SqlDbType.Int).Value = obj.CodSCotizacion_013AL;
                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }
        public string ModificarProveedor_013AL(Proveedor_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[ModificarProveedor-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@cuit", SqlDbType.Int).Value = obj.CUIT_013AL;
                com.Parameters.Add("@apellido", SqlDbType.NVarChar).Value = obj.ApellidoProveedor_013AL;
                com.Parameters.Add("@domicilio", SqlDbType.NVarChar).Value = obj.Domicilio_013AL;
                com.Parameters.Add("@mail", SqlDbType.NVarChar).Value = obj.Mail_013AL;
                com.Parameters.Add("@tel", SqlDbType.Int).Value = obj.Telefono_013AL;
                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }
        public DataTable ListarProveedoresDGV_013AL()
        {
            DataTable dataTable = new DataTable();
            try
            {
                string query = "SELECT * FROM [Proveedor-013AL]";
                SqlCommand command = new SqlCommand(query, con);
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                adapter.Fill(dataTable);
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return dataTable;

        }
        public string ModificarProveedor2_013AL(Proveedor_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[ModificarProveedor2-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@cuit", SqlDbType.Int).Value = obj.CUIT_013AL;
                com.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = obj.NombreProveedor_013AL;
                com.Parameters.Add("@apellido", SqlDbType.NVarChar).Value = obj.ApellidoProveedor_013AL;
                com.Parameters.Add("@domicilio", SqlDbType.NVarChar).Value = obj.Domicilio_013AL;
                com.Parameters.Add("@mail", SqlDbType.NVarChar).Value = obj.Mail_013AL;
                com.Parameters.Add("@rs", SqlDbType.NVarChar).Value = obj.RazonSocial_013AL;
                com.Parameters.Add("@tel", SqlDbType.Int).Value = obj.Telefono_013AL;
                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }
        public string EliminarProveedor_013AL(Proveedor_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[EliminarProveedor-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@cuit", SqlDbType.Int).Value = obj.CUIT_013AL;
                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }
        //select CodSCotizacion from SolicitudCotizacion

        public List<SolicitudCotizacion_013AL> ListarSCotizacion_013AL()
        {
            List<SolicitudCotizacion_013AL> Lista = new List<SolicitudCotizacion_013AL>();
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error al listar proveedores", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }
        public int GuardarOrdenCompra_013AL(OrdenCompra_013AL nuevaOrden)
        {
            int codOrdenCompra = 0;
            try
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
            catch (Exception ex) { }
            finally { con.Close(); }
            return codOrdenCompra;
        }

        public Proveedor_013AL TraerDatosProveedor_013AL(int codOrdenCompra)
        {
            Proveedor_013AL proveedor = null;

            try
            {
                // Crea el comando para ejecutar el procedimiento almacenado
                SqlCommand com = new SqlCommand("[TraerDatosProveedor-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@oc", SqlDbType.Int).Value = codOrdenCompra;

                // Abre la conexión
                con.Open();

                // Ejecuta el comando y lee los resultados
                SqlDataReader reader = com.ExecuteReader();
                if (reader.Read())
                {
                    // Mapea los resultados al objeto Proveedor
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
            catch (Exception ex)
            {

                throw new Exception("Error al traer datos del proveedor: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return proveedor;
        }
        public List<OrdenCompra_013AL> ListarOrdenCompra_013AL()
        {
            List<OrdenCompra_013AL> Lista = new List<OrdenCompra_013AL>();
            try
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
                            CodSolicitud_013AL = Convert.ToInt32(dr["IdSolicitud-013AL"].ToString()),
                            CUITProveedor_013AL = Convert.ToInt32(dr["CUITProveedor-013AL"].ToString()),
                            Fecha_013AL = Convert.ToDateTime(dr["Fecha-013AL"].ToString()),
                            Total_013AL = Convert.ToInt32(dr["Total-013AL"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar ordenes de compra", ex);
            }
            finally
            {
                con.Close();
            }

            return Lista;
        }
        public int ObtenerCodOrdenCompra_013AL(int idSolicitud, int cuitProveedor, DateTime fecha)
        {
            int codOrdenCompra = 0;
            try
            {
                SqlCommand cmd = new SqlCommand("[ObtenerCodOrdenCompra-013AL]", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                cmd.Parameters.AddWithValue("@CUITProveedor", cuitProveedor);
                cmd.Parameters.AddWithValue("@Fecha", fecha);

                con.Open();
                codOrdenCompra = (int)cmd.ExecuteScalar(); // Se espera un único valor

            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return codOrdenCompra;
        }

        public string RegistrarCompra_013AL(Producto_013AL obj)
        {
            string resultado = "";
            try
            {
                SqlCommand com = new SqlCommand("[RegistrarCompra-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = obj.CodProducto_013AL;
                com.Parameters.Add("@stock", SqlDbType.Int).Value = obj.Stock_013AL;

                con.Open();
                resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return resultado;
        }
        public DataTable ListarOrdenesCompra_013AL()
        {
            DataTable dt = new DataTable();
            using (SqlCommand com = new SqlCommand("SELECT * FROM [OrdenCompra-013AL]", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(com);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable ObtenerProductosPorOrden_013AL(int codOrdenCompra)
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
        public string ActualizarEstadoCompleto_013AL(int codOrdenCompra, bool completo)
        {
            using (SqlCommand com = new SqlCommand("UPDATE [OrdenCompra-013AL] SET [Completo-013AL] = @Completo WHERE [CodOrdenCompra-013AL] = @CodOrdenCompra", con))
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











        //COMPOSITE NUEVO

       
        public DataTable TraerListaFamilias_013AL()
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("Select * from [Permisos-013AL]", con);
                com.CommandType = CommandType.Text;
                con.Open();
                resultado = com.ExecuteReader();
                dt.Load(resultado);
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return dt;
        }
      
        public int CrearFamilia_013AL(string nombreFamilia)
        {
            int nuevoId = 0;
            SqlCommand comando = new SqlCommand("[CrearFamilia-013AL]", con);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@NombreFamilia", nombreFamilia);

                    try
                    {
                        con.Open();
                        object resultado = comando.ExecuteScalar();
                        if (resultado != null)
                        {
                            nuevoId = Convert.ToInt32(resultado);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al crear la familia", ex);
                    }
                    finally { con.Close(); }
                
            
            return nuevoId;
        }

        public string ModificarFamilia_013AL(Componente_013AL fam)
        {
            string resultado = "";
            try
            {
                SqlCommand comando = new SqlCommand("[ModificarFamilia-013AL]", con);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add("@CodPermiso", SqlDbType.Int).Value = fam.Cod_013AL;
                comando.Parameters.Add("@NombreFamilia", SqlDbType.NVarChar, 50).Value = fam.Nombre_013AL;
                con.Open();
                resultado = comando.ExecuteNonQuery() == 1 ? "OK" : "No se pudo modificar familia";
            }
            catch(Exception ex) { }
            finally { con.Close(); }
            return resultado;
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

        public List<Componente_013AL> TraerListaHijos_013AL(int idPadre)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            List<Componente_013AL> lista = new List<Componente_013AL>();

            try
            {
                SqlCommand com = new SqlCommand("[TraerListaHijos-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@codPermisoPadre", idPadre);

                con.Open();
                resultado = com.ExecuteReader();
                dt.Load(resultado);

                foreach (DataRow dr in dt.Rows)
                {
                    Componente_013AL permiso;

                    // Convertir el campo Tipo (INT) a Familia o Permiso
                    int tipo = Convert.ToInt32(dr[3]);  // Asumiendo que la columna "Tipo" está en la posición 3

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
            catch (Exception ex)
            {
                MessageBox.Show("Error en TraerListaHijos: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return lista;
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

        public string EliminarFamilia_013AL(int id)
        {
            string respuesta = "";
            try
            {
                com = new SqlCommand("[EliminarFamilia-013AL]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@id", SqlDbType.Int).Value = id;
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
                    if (reader.Read()) // Solo toma la primera fila
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
            /*List<Permiso> permisos = new List<Permiso>();


            SqlCommand command = new SqlCommand("VerificarSiEstaEnFamilia", con);
                
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idHijo", idHijo);

                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Permiso permiso = new Permiso
                            {
                                Id = Convert.ToInt32(reader["id_permiso_padre"]),
                                Nombre = reader["NombrePermiso"].ToString(),
                                Tipo = reader["Tipo"].ToString()
                            };
                            permisos.Add(permiso);
                        }
                    }
                
            

            return permisos;*/
        }

        public int CrearRol_013AL(string nombreRol)
        {
            int nuevoId = 0;
            SqlCommand comando = new SqlCommand("[AgregarRol-013AL]", con);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@NombreRol", nombreRol);

            try
            {
                con.Open();
                object resultado = comando.ExecuteScalar();
                if (resultado != null)
                {
                    nuevoId = Convert.ToInt32(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el rol", ex);
            }
            finally { con.Close(); }


            return nuevoId;
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

        //eliminar rol esta mas arriba

        //RegistrarRolPermiso = AgregarFamiliaRol(+ arriba)

        public List<Componente_013AL> TraerListaPermisosRol_013AL(int idRol)
        {
            List<Componente_013AL> lista = new List<Componente_013AL>();

           
                try
                {
                con.Open();
                SqlCommand cmd = new SqlCommand("[TraerListaPermisosRol-013AL]", con);
                    
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodRol", idRol);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Componente_013AL permiso;
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
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener la lista de permisos del rol.", ex);
                }
            finally { con.Close(); }
            

            return lista;
        }

        //ListarRoles

        //ModificarRol

        //listar usuarios

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

        public DataTable TraerListaPermisos_013AL()
        {
            SqlDataReader resultado;
            DataTable tabla = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("SELECT [CodPermiso-013AL], [NombrePermiso-013AL], [Tipo-013AL] FROM [Permisos-013AL]", con);
                com.CommandType = CommandType.Text;
                con.Open();
                resultado = com.ExecuteReader();
                tabla.Load(resultado);
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return tabla;
        }

        public bool RegistrarHijoEnFamilia_013AL(int idPadre, int idHijo)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("[RegistrarHijosFamilia-013AL]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PermisoPadre", idPadre);
                    cmd.Parameters.AddWithValue("@PermisoHijo", idHijo);

                    // Parámetro de salida para recibir el resultado
                    SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.Int);
                    resultadoParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(resultadoParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    // Leer el resultado
                    int resultado = (int)cmd.Parameters["@Resultado"].Value;
                    return resultado == 1; // Devuelve true si se insertó, false si ya existía
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la relación en la base de datos.", ex);
            }
            finally
            {
                con.Close();
            }
        }

        public void EliminarPermisoDeFamilia_013AL(int idPermiso, int idFamilia)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el permiso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }


        public bool VerificarPermisoEnFamilia_013AL(int permisoId, int familiaId)
        {
                    
            int count = 0;
            try
            {
                string query = "SELECT COUNT(*) FROM [Permisos_Componente-013AL] WHERE [cod_permiso_padre-013AL] = @familiaId AND [cod_permiso_hijo-013AL] = @permisoId";


                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@familiaId", familiaId);
                command.Parameters.AddWithValue("@permisoId", permisoId);

                con.Open();
                count = (int)command.ExecuteScalar();
            }catch (Exception ex) { }
            finally { con.Close(); }

            return count > 0;
            
        }

        public DataTable TraerListaRoles_013AL()
        {
            SqlDataReader resultado;
            DataTable tabla = new DataTable();
            try
            {
                SqlCommand com = new SqlCommand("Select * from [Roles-013AL]", con);
                com.CommandType = CommandType.Text;
                con.Open();
                resultado = com.ExecuteReader();
                tabla.Load(resultado);
            }
            catch (Exception ex) { }
            finally { con.Close(); }
            return tabla;
        }
        public bool EliminarPermisoRol_013AL(int idRol, int idPermiso)
        {
            int filasAfectadas = 0;
            try {  
            SqlCommand cmd = new SqlCommand("[EliminarPermisoRol-013AL]", con);
            con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idRol", idRol);
                    cmd.Parameters.AddWithValue("@idPermiso", idPermiso);

                filasAfectadas = cmd.ExecuteNonQuery();
            } catch(Exception ex) {
                Console.WriteLine($"Error al eliminar permiso de rol: {ex.Message}");
                return false;
            }
            finally { con.Close(); }
            return filasAfectadas > 0;
                
        }  
        public bool VerificarRelacionesRol_013AL(int idRol)
        {
            int count = 0;
            try
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM [Rol_Permiso-013AL] WHERE [CodRol-013AL] = @IdRol";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@IdRol", idRol);
                count = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return false; }
            finally { con.Close(); }
            return count > 0;
                
        }
        public bool FamiliaTieneRelaciones_013AL(int idFamilia)
        {
            int count = 0;
            try
            {

                string query = @"SELECT COUNT(*) FROM [Permisos_Componente-013AL] 
                     WHERE [cod_permiso_padre-013AL] = @idFamilia OR [cod_permiso_hijo-013AL] = @idFamilia";
                SqlCommand comando = new SqlCommand(query, con);

                comando.Parameters.AddWithValue("@idFamilia", idFamilia);
                con.Open();
                count = (int)comando.ExecuteScalar();
            }catch(Exception ex) { Console.WriteLine( ex.ToString()); return false; }
            finally { con.Close(); }
                return count > 0; // Retorna true si la familia tiene relaciones
            
        }



        //DIGITO VERIFICADOR

        /*public  List<DigitoVerificador> ObtenerRegistros(string tabla)
        {
            List<DigitoVerificador> registros = new List<DigitoVerificador>();
            
                con.Open();
                string query = $"SELECT * FROM {tabla}";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    registros.Add(new DigitoVerificador
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Campo1 = reader["Campo1"].ToString(),
                        Campo2 = reader["Campo2"].ToString(),
                        DVH = Convert.ToInt32(reader["DVH"])
                    });
                }
            
            return registros;
        }

        // Actualizar el DVH de un registro
        public  void ActualizarDVH(int id, string tabla, int nuevoDVH)
        {
            
                con.Open();
                string query = $"UPDATE {tabla} SET DVH = @DVH WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DVH", nuevoDVH);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            
        }

        // Obtener el DVV de una tabla
        public  int ObtenerDVV(string tabla)
        {
            
                con.Open();
                string query = "SELECT DVV FROM DV WHERE Tabla = @Tabla";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Tabla", tabla);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            
        }
        */


    }
}

  




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
    public class DALProducto_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public string ObtenerNombreProducto_013AL(int idProducto)
        {
            string nombre = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex) { throw new Exception("Error al obtener nombre del producto", ex); }
            return nombre;
        }


        public void DescontarStock_013AL(int idProducto, int cantidad)
        {
            string query = "UPDATE [Producto-013AL] SET [Stock-013AL] = [Stock-013AL] - @cant WHERE [CodProducto-013AL] = @id";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
                throw new Exception("Error al descontar stock.", ex);
            }
        }
        public void RevertirStock_013AL(int idProducto, int cantidad)
        {
            string query = "UPDATE [Producto-013AL] SET [Stock-013AL] = [Stock-013AL] + @cant WHERE [CodProducto-013AL] = @id";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
                throw new Exception("Error al revertir stock.", ex);
            }
        }

        public DataTable BuscarProductoPorId_013AL(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("BuscarProductoPorId-013AL", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@idl", SqlDbType.Int).Value = id;

                    con.Open();
                    SqlDataReader reader = com.ExecuteReader();
                    dt.Load(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar producto por id", ex);
            }
            return dt;
        }

        public DataTable ListaProductos_013AL()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand cmd = new SqlCommand("ListaProductos-013AL", con))
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
                throw new Exception("Error general al listar los productos.", ex);
            }
            return dt;
        }

        public DataTable BuscarProductoxNombre_013AL(string valor)
        {
            SqlDataReader resultado;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("BuscarProductoxNombre-013AL", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@valor", SqlDbType.NVarChar).Value = valor;
                    con.Open();
                    resultado = com.ExecuteReader();
                    dt.Load(resultado);
                }

            }
            catch (Exception ex) { throw new Exception("Error al buscar producto por nombre", ex); }
            return dt;
        }

        public List<Producto_013AL> ListarProductosPocoStock_013AL()
        {
            List<Producto_013AL> Lista = new List<Producto_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos", ex);
            }
            return Lista;
        }

        public Producto_013AL ObtenerProductoPorId_013AL(int idProducto)
        {
            Producto_013AL producto = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex) { throw new Exception("Error al buscar producto por id", ex); }
            return producto;
        }

        

        public string AgregarProducto_013AL(Producto_013AL obj)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex) { throw new Exception("Error al agregar producto", ex); }
            return respuesta;
        }

        public DataTable ObtenerProductosConImagen_013AL()
        {
            string query = "SELECT [CodProducto-013AL], [Nombre-013AL], [Stock-013AL], [Precio-013AL], [Imagen-013AL], [Descripcion-013AL] FROM [Producto-013AL] WHERE [Bit_Lo_Bo-013AL] = 1";
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public string ModificarProducto_013AL(Producto_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[ModificarProducto-013AL]", con);
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
            }
            catch (Exception ex) { throw new Exception("Error al modificar producto", ex); }
            return resultado;
        }

        public string EliminarProducto_013AL(Producto_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[EliminarProducto-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = obj.CodProducto_013AL;

                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
                }
            }
            catch (Exception ex) { throw new Exception("Error al eliminar producto", ex); }
            return resultado;
        }
    }
}

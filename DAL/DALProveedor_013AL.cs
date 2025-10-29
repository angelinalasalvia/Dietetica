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
    public class DALProveedor_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public List<Proveedor_013AL> ListarProveedores_013AL()
        {
            List<Proveedor_013AL> Lista = new List<Proveedor_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar proveedores", ex);
            }
            return Lista;
        }

        public bool VerificarCuit_013AL(int cuit)
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM [Proveedores-013AL] WHERE [CUIT-013AL] = @Cuit";

            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
                throw new Exception("Error verificar CUIT", ex);
            }
            return count > 0;
        }

        public string PreregistrarProveedor_013AL(Proveedor_013AL proveedor)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            return respuesta;
        }

        public string RegistrarProveedor_013AL(Proveedor_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[RegistrarProveedor-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@cuit", SqlDbType.Int).Value = obj.CUIT_013AL;
                    com.Parameters.Add("@apellido", SqlDbType.NVarChar).Value = obj.ApellidoProveedor_013AL;
                    com.Parameters.Add("@domicilio", SqlDbType.NVarChar).Value = obj.Domicilio_013AL;
                    com.Parameters.Add("@mail", SqlDbType.NVarChar).Value = obj.Mail_013AL;
                    com.Parameters.Add("@tel", SqlDbType.Int).Value = obj.Telefono_013AL;
                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
                }
            }
            catch (Exception ex) { throw new Exception("Error registrar proveedor", ex); }
            return resultado;
        }
        public DataTable ListarProveedoresDGV_013AL()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "SELECT * FROM [Proveedor-013AL]";
                    SqlCommand command = new SqlCommand(query, con);
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex) { throw new Exception("Error al listar proveedores", ex); }
            return dataTable;

        }
        public string ModificarProveedor_013AL(Proveedor_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[ModificarProveedor-013AL]", con);
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
            }
            catch (Exception ex) { throw new Exception("Error al modificar proveedor", ex); }
            return resultado;
        }
        public string EliminarProveedor_013AL(Proveedor_013AL obj)
        {
            string resultado = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand com = new SqlCommand("[EliminarProveedor-013AL]", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@cuit", SqlDbType.Int).Value = obj.CUIT_013AL;
                    con.Open();
                    resultado = com.ExecuteNonQuery() == 1 ? "OK" : "Error";
                }
            }
            catch (Exception ex) { throw new Exception("Error al eliminar proveedor", ex); }
            return resultado;
        }
        
    }
}

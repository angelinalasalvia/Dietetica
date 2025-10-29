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
    public class DALCliente_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public DataSet ObtenerClientes_013AL()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [Cliente-013AL]", con);
                    da.Fill(ds, "Cliente");                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes: ", ex);
            }
            return ds;
        }
        public void GuardarClientes_013AL(string NombreTabla, DataSet Dset)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlDataAdapter da = new SqlDataAdapter(("SELECT * FROM [Cliente-013AL]"), con);
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.UpdateCommand = cb.GetUpdateCommand();
                da.DeleteCommand = cb.GetDeleteCommand();
                da.InsertCommand = cb.GetInsertCommand();
                da.ContinueUpdateOnError = true;
                da.Fill(Dset);
                da.Update(Dset.Tables[0]);
            }
        }
        public string AgregarCliente_013AL(Cliente_013AL obj)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
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
                }
            }
            catch (Exception ex) { throw new Exception("No se pudo registrar el cliente.", ex); }
            return respuesta;
        }
        public List<Cliente_013AL> BuscarCliente_013AL()
        {
            List<Cliente_013AL> Lista = new List<Cliente_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar clientes", ex);
            }
            return Lista;
        }

        public Cliente_013AL BuscarClientePorCUIL_013AL(int cuil)
        {
            Cliente_013AL cliente = null;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar cliente por CUIL", ex);
            }
            return cliente;
        }

    }
}

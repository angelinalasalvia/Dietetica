using BE;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALIdioma_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        public int Agregar(Idioma_013AL idioma)
        {
            int nuevoId;
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    var cmd = new SqlCommand("INSERT INTO [Idioma-013AL] ([Nombre-013AL]) VALUES (@Nombre); SELECT SCOPE_IDENTITY();", con);
                    cmd.Parameters.AddWithValue("@Nombre", idioma.Nombre_013AL);
                    nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar idioma", ex);
            }
            return nuevoId;
        }

        public List<Idioma_013AL> ListarIdiomas_013AL()
        {
            List<Idioma_013AL> lista = new List<Idioma_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("SELECT [IdIdioma-013AL], [Nombre-013AL] FROM [Idioma-013AL]", con);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Idioma_013AL idioma = new Idioma_013AL
                        {
                            IdIdioma_013AL = reader.GetInt32(reader.GetOrdinal("IdIdioma-013AL")),
                            Nombre_013AL = reader.GetString(reader.GetOrdinal("Nombre-013AL"))
                        };
                        lista.Add(idioma);
                    }
                }
            } 
            catch (Exception ex) 
            {
                throw new Exception("Error al listar idiomas", ex);
            }
            return lista;
        }
    }


}


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
    public class DALTraduccion_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;


        public List<Traduccion_013AL> ObtenerPorIdioma(int idIdioma)
        {
            List<Traduccion_013AL> lista = new List<Traduccion_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT t.[IdTraduccion-013AL], t.[IdIdioma-013AL], t.[IdEtiqueta-013AL], t.[Texto-013AL], e.[Nombre-013AL] " +
                        "FROM [Traduccion-013AL] t INNER JOIN [Etiqueta-013AL] e ON t.[IdEtiqueta-013AL] = e.[IdEtiqueta-013AL] " +
                        "WHERE t.[IdIdioma-013AL] = @IdIdioma", con);
                    cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Traduccion_013AL
                        {
                            IdTraduccion_013AL = Convert.ToInt32(dr["IdTraduccion-013AL"]),
                            IdIdioma_013AL = Convert.ToInt32(dr["IdIdioma-013AL"]),
                            IdEtiqueta_013AL = Convert.ToInt32(dr["IdEtiqueta-013AL"]),
                            Texto_013AL = dr["Texto-013AL"].ToString(),
                            Etiqueta_013AL = new Etiqueta_013AL() 
                            {
                                IdEtiqueta_013AL = Convert.ToInt32(dr["IdEtiqueta-013AL"]),
                                Nombre_013AL = dr["Nombre-013AL"].ToString()
                            }
                        });
                    }
                    dr.Close();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener traducciones por idioma", ex);
            }
            return lista;
        
    }
        public void ClonarTraduccionesDesdeEspanol(int idNuevoIdioma, int idEspanol)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    var cmd = new SqlCommand(@"
                INSERT INTO [Traduccion-013AL] ([IdIdioma-013AL], [IdEtiqueta-013AL], [Texto-013AL])
                SELECT @IdNuevoIdioma, [IdEtiqueta-013AL], '[' + [Texto-013AL] + ']'
                FROM [Traduccion-013AL]
                WHERE [IdIdioma-013AL] = @IdEspanol", con);

                    cmd.Parameters.AddWithValue("@IdNuevoIdioma", idNuevoIdioma);
                    cmd.Parameters.AddWithValue("@IdEspanol", idEspanol);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al clonar traducciones desde español", ex);
            }
        }


        public void ActualizarTexto(int idTraduccion, string nuevoTexto)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();
                    var cmd = new SqlCommand("UPDATE [Traduccion-013AL] SET [Texto-013AL] = @Texto WHERE [IdTraduccion-013AL] = @Id", con);
                    cmd.Parameters.AddWithValue("@Texto", nuevoTexto);
                    cmd.Parameters.AddWithValue("@Id", idTraduccion);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar texto de traducción", ex);
            }
        }

    }
}

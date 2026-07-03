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
using System.Text.Json.Nodes;


namespace DAL_013AL
{
    public class DALConexiones_013AL
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Dietetica;Integrated Security=True";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }

        //PARA INSTALADOR
        /*private string ObtenerStringConexion()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDir, "config.json");

            

            if (!File.Exists(path))
                throw new FileNotFoundException("No se encontró config.json en: " + path, path);

            var json = File.ReadAllText(path);
            JsonObject nodo = JsonNode.Parse(json)?.AsObject();
            string cs = nodo?["ConnectionString"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(cs))
                throw new Exception("No se encontró la cadena de conexión.");
            return cs;
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(ObtenerStringConexion());
        }*/

    }
}

  




using System;
using System.Data.SqlClient;
using System.IO;

namespace DatabaseSetup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Leer la cadena de conexión desde un archivo de configuración
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

            if (!File.Exists(configPath))
            {
                Console.WriteLine("Error: No se encontró el archivo de configuración.");
                return;
            }

            string[] configLines = File.ReadAllLines(configPath);
            string dataSource = configLines[0].Split('=')[1].Trim();
            string database = configLines[1].Split('=')[1].Trim();

            string connectionString = $@"Data Source={dataSource};Initial Catalog={database};Integrated Security=True";

            // Obtener la ruta del script SQL
            string scriptFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BaseDatos511.sql");

            if (!File.Exists(scriptFilePath))
            {
                Console.WriteLine("Error: No se encontró el archivo de la base de datos.");
                return;
            }

            try
            {
                // Leer el contenido del script SQL
                string script = File.ReadAllText(scriptFilePath);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ejecutar cada comando individualmente
                    string[] commands = script.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var commandText in commands)
                    {
                        using (SqlCommand command = new SqlCommand(commandText, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("Base de datos configurada correctamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al configurar la base de datos: " + ex.Message);
            }
        }
    }
}

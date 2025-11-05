using BE;
using BE_013AL;
using DAL_013AL;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALFactura_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        //en factura se agrega lo del digito verificador

        public List<Factura_013AL> ListarFactura_013AL()
        {
            List<Factura_013AL> Lista = new List<Factura_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = "Select [CodCompra-013AL], [CUILCliente-013AL], [Fecha-013AL], [Total-013AL], [IVA-013AL], [MetodoPago-013AL] from [Factura-013AL]";
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
                                Total_013AL = Convert.ToInt32(dr["Total-013AL"]),
                                IVA_013AL = Convert.ToInt32(dr["IVA-013AL"]),
                                MetPago_013AL = Convert.ToString(dr["MetodoPago-013AL"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar", ex);
            }
            return Lista;
        }

        /*public string AgregarFactura_013AL(Factura_013AL cc)
        {
            string respuesta = "";
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    com = new SqlCommand("AgregarCompraCliente-013AL", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add("@idc", SqlDbType.Int).Value = cc.CodCompra_013AL;
                    com.Parameters.Add("@cuil", SqlDbType.Int).Value = cc.CUIL_013AL;
                    com.Parameters.Add("@metpago", SqlDbType.NVarChar).Value = cc.MetPago_013AL;
                    con.Open();
                    respuesta = com.ExecuteNonQuery() == 1 ? "OK" : "error";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar la factura.", ex);
            }
            return respuesta;
        }*/

        /*public string RegistrarVentaCompleta_013AL(Factura_013AL factura, List<Detalle_013AL> detalles)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmdFactura = new SqlCommand("AgregarCompraCliente-013AL", con, transaction))
                            {
                                cmdFactura.CommandType = CommandType.StoredProcedure;
                                cmdFactura.Parameters.AddWithValue("@idc", factura.CodCompra_013AL);
                                cmdFactura.Parameters.AddWithValue("@cuil", factura.CUIL_013AL);
                                cmdFactura.Parameters.AddWithValue("@tot", factura.Total_013AL);
                                cmdFactura.Parameters.AddWithValue("@metpago", factura.MetPago_013AL);
                                cmdFactura.ExecuteNonQuery();
                            }

                            foreach (var detalle in detalles)
                            {
                                using (SqlCommand cmdDetalle = new SqlCommand("AgregarCompraProducto-013AL", con, transaction))
                                {
                                    cmdDetalle.CommandType = CommandType.StoredProcedure;
                                    cmdDetalle.Parameters.AddWithValue("@idc", detalle.CodCompra_013AL);
                                    cmdDetalle.Parameters.AddWithValue("@idp", detalle.CodProducto_013AL);
                                    cmdDetalle.Parameters.AddWithValue("@cant", detalle.Cantidad_013AL);
                                    cmdDetalle.Parameters.AddWithValue("@pu", detalle.PrecioUnitario_013AL);
                                    cmdDetalle.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return "OK";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error al registrar los detalles de la venta: ", ex);
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                throw new Exception("Error general al registrar la venta completa.", ex);
            }
        }*/


        public string RegistrarVentaCompleta_013AL(Factura_013AL factura, List<Detalle_013AL> detalles)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // 1) Inserto la factura (tu SP)
                            using (SqlCommand cmdFactura = new SqlCommand("AgregarCompraCliente-013AL", con, transaction))
                            {
                                cmdFactura.CommandType = CommandType.StoredProcedure;
                                cmdFactura.Parameters.AddWithValue("@idc", factura.CodCompra_013AL);
                                cmdFactura.Parameters.AddWithValue("@cuil", factura.CUIL_013AL);
                                cmdFactura.Parameters.AddWithValue("@tot", factura.Total_013AL);
                                cmdFactura.Parameters.AddWithValue("@metpago", factura.MetPago_013AL);
                                cmdFactura.ExecuteNonQuery();
                            }

                            // 2) Inserto cada detalle
                            foreach (var detalle in detalles)
                            {
                                using (SqlCommand cmdDetalle = new SqlCommand("AgregarCompraProducto-013AL", con, transaction))
                                {
                                    cmdDetalle.CommandType = CommandType.StoredProcedure;
                                    cmdDetalle.Parameters.AddWithValue("@idc", detalle.CodCompra_013AL);
                                    cmdDetalle.Parameters.AddWithValue("@idp", detalle.CodProducto_013AL);
                                    cmdDetalle.Parameters.AddWithValue("@cant", detalle.Cantidad_013AL);
                                    cmdDetalle.Parameters.AddWithValue("@pu", detalle.PrecioUnitario_013AL);
                                    cmdDetalle.ExecuteNonQuery();
                                }
                            }

                            // 3) LEER la fila insertada de la tabla Factura-013AL para calcular el DVH sobre los valores exactos guardados
                            //    (Evita discrepancias si la DB agregó Fecha u otros defaults)
                            DataTable dtFactura = new DataTable();
                            string selectFactura = "SELECT * FROM [Factura-013AL] WHERE [CodCompra-013AL] = @Id";
                            using (SqlCommand cmdSelect = new SqlCommand(selectFactura, con, transaction))
                            {
                                cmdSelect.Parameters.AddWithValue("@Id", factura.CodCompra_013AL);
                                using (SqlDataAdapter da = new SqlDataAdapter(cmdSelect))
                                {
                                    da.Fill(dtFactura);
                                }
                            }

                            if (dtFactura.Rows.Count == 1)
                            {
                                DataRow row = dtFactura.Rows[0];
                                StringBuilder cadena = new StringBuilder();

                                foreach (DataColumn col in dtFactura.Columns)
                                {
                                    // ignorar la columna DVH si existe en la tabla
                                    if (col.ColumnName.Equals("DigitoHorizontal-013AL", StringComparison.OrdinalIgnoreCase))
                                        continue;

                                    // evitar nulls
                                    if (row[col] != DBNull.Value)
                                        cadena.Append(row[col].ToString());
                                }

                                string dvh = HashHelper_013AL.CalcularSHA256_013AL(cadena.ToString());

                                // 4) Actualizar la columna DigitoHorizontal-013AL de la factura dentro de la misma transacción
                                string update = "UPDATE [Factura-013AL] SET [DigitoHorizontal-013AL] = @DVH WHERE [CodCompra-013AL] = @Id";
                                using (SqlCommand cmdUpdateDVH = new SqlCommand(update, con, transaction))
                                {
                                    cmdUpdateDVH.Parameters.AddWithValue("@DVH", dvh);
                                    cmdUpdateDVH.Parameters.AddWithValue("@Id", factura.CodCompra_013AL);
                                    cmdUpdateDVH.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Si por alguna razón no se encuentra la fila, podemos optar por rollback y lanzar excepción
                                throw new Exception("No se pudo leer la factura recién insertada para calcular el DVH.");
                            }

                            // 5) Commit de la transacción
                            transaction.Commit();

                            // 6) Actualizar el DVV de la tabla Factura-013AL (fuera de la transacción)
                            //    Esto recalcula el DVV global concatenando todos los DVH y lo guarda/actualiza.
                            ActualizarDVV("Factura-013AL");

                            return "OK";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error al registrar los detalles de la venta: ", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error general al registrar la venta completa.", ex);
            }
        }




        /*public bool ActualizarDVH(string tabla)
        {
            try
            {
                // 1️⃣ Armo la consulta
                string consulta = $"SELECT * FROM {tabla}";
                DataTable dt = new DataTable();

                // 2️⃣ Abro la conexión y ejecuto el comando
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta, con))
                    {
                        com.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                // 3️⃣ Si no hay registros, salgo
                if (dt.Rows.Count == 0) return false;

                

                // 5️⃣ Tomo la clave primaria (suponiendo que es la primera columna)
                string clavePrimaria = dt.Columns[0].ColumnName;

                // 6️⃣ Recorro filas para calcular y actualizar DVH
                foreach (DataRow row in dt.Rows)
                {
                    string cadena = "";
                    foreach (DataColumn col in dt.Columns)
                    {
                        cadena += row[col].ToString();
                    }

                    string dvh = HashHelper_013AL.CalcularSHA256_013AL(cadena);

                    // 7️⃣ Actualizo el registro con su nuevo DVH
                    string update = $"UPDATE {tabla} SET [DigitoHorizontal-013AL] = @DVH WHERE {clavePrimaria} = @Id";

                    using (SqlConnection con = conexion.ObtenerConexion())
                    {
                        using (SqlCommand com = new SqlCommand(update, con))
                        {
                            com.Parameters.AddWithValue("@DVH", dvh);
                            com.Parameters.AddWithValue("@Id", row[clavePrimaria]);
                            con.Open();
                            com.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar los dígitos verificadores.", ex);
            }
        }*/

        public bool ActualizarDVH(string tabla)
        {
            try
            {
                string consulta = $"SELECT * FROM [{tabla}]";
                DataTable dt = new DataTable();

                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta, con))
                    {
                        com.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                // Si no hay registros, salir sin error
                if (dt.Rows.Count == 0) return true;

                string clavePrimaria = dt.Columns[0].ColumnName;

                foreach (DataRow row in dt.Rows)
                {
                    StringBuilder cadena = new StringBuilder();

                    foreach (DataColumn col in dt.Columns)
                    {
                        // Evitar columnas DVH o columnas nulas
                        if (col.ColumnName.Equals("DigitoHorizontal-013AL", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (row[col] != DBNull.Value)
                            cadena.Append(row[col].ToString());
                    }

                    // Si la fila tiene datos, calcular el DVH
                    string dvh = HashHelper_013AL.CalcularSHA256_013AL(cadena.ToString());

                    string update = $"UPDATE [{tabla}] SET [DigitoHorizontal-013AL] = @DVH WHERE [{clavePrimaria}] = @Id";

                    using (SqlConnection con = conexion.ObtenerConexion())
                    {
                        using (SqlCommand com = new SqlCommand(update, con))
                        {
                            com.Parameters.AddWithValue("@DVH", dvh);
                            com.Parameters.AddWithValue("@Id", row[clavePrimaria]);
                            con.Open();
                            com.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar DVH de {tabla}.", ex);
            }
        }

        public bool VerificarDVV(string tabla)
        {
            try
            {
                DataTable dt1 = new DataTable();

                // 1️⃣ Traigo los DVH de la tabla indicada
                string consulta1 = $"SELECT [DigitoHorizontal-013AL] FROM [{tabla}]";
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta1, con))
                    {
                        com.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dt1);
                        }
                    }
                }

                if (dt1.Rows.Count == 0)
                    return false;

                // 2️⃣ Concateno todos los DVH para calcular el DVV
                StringBuilder sb = new StringBuilder();
                foreach (DataRow row in dt1.Rows)
                {
                    sb.Append(row["DigitoHorizontal-013AL"].ToString());
                }

                string dvvCalculado = HashHelper_013AL.CalcularSHA256_013AL(sb.ToString());

                // 3️⃣ Traigo el DVV guardado para esa tabla
                DataTable dt2 = new DataTable();
                string consulta2 = "SELECT [DVV-013AL] FROM [DVV-013AL] WHERE [Tabla-013AL] = @Tabla";

                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta2, con))
                    {
                        com.CommandType = CommandType.Text;
                        com.Parameters.AddWithValue("@Tabla", tabla);

                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dt2);
                        }
                    }
                }

                if (dt2.Rows.Count == 0)
                    return false;

                string dvvGuardado = dt2.Rows[0]["DVV-013AL"].ToString();

                // 4️⃣ Comparo el DVV calculado con el guardado
                return dvvGuardado == dvvCalculado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar el DVV.", ex);
            }
        }
        /*public bool ActualizarDVV(string tabla)
        {
            try
            {
                // 1️⃣ Traigo los DVH de la tabla indicada
                DataTable dt = new DataTable();
                string consulta = $"SELECT [DigitoHorizontal-013AL] FROM {tabla}";

                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta, con))
                    {
                        com.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count == 0)
                    return false;

                // 2️⃣ Concateno los DVH para calcular el DVV
                StringBuilder sb = new StringBuilder();
                foreach (DataRow row in dt.Rows)
                {
                    sb.Append(row["DigitoHorizontal-013AL"].ToString());
                }

                string dvvCalculado = HashHelper_013AL.CalcularSHA256_013AL(sb.ToString());

                // 3️⃣ Verifico si ya existe un registro de DVV para esa tabla
                bool existe = false;
                string existeConsulta = "SELECT COUNT(*) FROM [DVV-013AL] WHERE [Tabla-013AL] = @Tabla";

                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(existeConsulta, con))
                    {
                        com.Parameters.AddWithValue("@Tabla", tabla);
                        con.Open();

                        int cantidad = Convert.ToInt32(com.ExecuteScalar());
                        existe = (cantidad > 0);
                    }
                }

                // 4️⃣ Armo la consulta según exista o no el registro
                string consulta2 = existe
                    ? "UPDATE [DVV-013AL] SET [DVV-013AL] = @DVV WHERE [Tabla-013AL] = @Tabla"
                    : "INSERT INTO [DVV-013AL] ([Tabla-013AL], [DVV-013AL]) VALUES (@Tabla, @DVV)";

                // 5️⃣ Ejecuto la actualización o inserción
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta2, con))
                    {
                        com.Parameters.AddWithValue("@Tabla", tabla);
                        com.Parameters.AddWithValue("@DVV", dvvCalculado);
                        con.Open();

                        int filasAfectadas = com.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el DVV.", ex);
            }
        }*/

        public bool ActualizarDVV(string tabla)
        {
            try
            {
                DataTable dt = new DataTable();
                string consulta = $"SELECT [DigitoHorizontal-013AL] FROM [{tabla}]";

                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(consulta, con))
                    {
                        com.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                // Si la tabla no tiene registros, crear DVV en 0 (para que no falle)
                if (dt.Rows.Count == 0)
                {
                    using (SqlConnection con = conexion.ObtenerConexion())
                    {
                        string insert = "IF NOT EXISTS (SELECT 1 FROM [DVV-013AL] WHERE [Tabla-013AL] = @Tabla) " +
                                        "INSERT INTO [DVV-013AL] ([Tabla-013AL], [DVV-013AL]) VALUES (@Tabla, '0')";
                        using (SqlCommand com = new SqlCommand(insert, con))
                        {
                            com.Parameters.AddWithValue("@Tabla", tabla);
                            con.Open();
                            com.ExecuteNonQuery();
                        }
                    }
                    return true;
                }

                // Concatenar DVH existentes
                StringBuilder sb = new StringBuilder();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["DigitoHorizontal-013AL"] != DBNull.Value)
                        sb.Append(row["DigitoHorizontal-013AL"].ToString());
                }

                string dvvCalculado = HashHelper_013AL.CalcularSHA256_013AL(sb.ToString());
                string upsert = @"
            IF EXISTS (SELECT 1 FROM [DVV-013AL] WHERE [Tabla-013AL] = @Tabla)
                UPDATE [DVV-013AL] SET [DVV-013AL] = @DVV WHERE [Tabla-013AL] = @Tabla
            ELSE
                INSERT INTO [DVV-013AL] ([Tabla-013AL], [DVV-013AL]) VALUES (@Tabla, @DVV)";

                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    using (SqlCommand com = new SqlCommand(upsert, con))
                    {
                        com.Parameters.AddWithValue("@Tabla", tabla);
                        com.Parameters.AddWithValue("@DVV", dvvCalculado);
                        con.Open();
                        com.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar DVV de {tabla}.", ex);
            }
        }


        public List<ErrorIntegridad_013AL> VerificarIntegridadCompleta(List<string> tablas)
        {
            var errores = new List<ErrorIntegridad_013AL>();

            try
            {
                foreach (var tabla in tablas)
                {
                    DataTable dt = new DataTable();
                    string consulta = $"SELECT * FROM [{tabla}]";

                    using (SqlConnection con = conexion.ObtenerConexion())
                    {
                        using (SqlCommand com = new SqlCommand(consulta, con))
                        {
                            com.CommandType = CommandType.Text;
                            using (SqlDataAdapter da = new SqlDataAdapter(com))
                            {
                                da.Fill(dt);
                            }
                        }
                    }

                    if (dt.Rows.Count == 0)
                        continue;

                    string clavePrimaria = dt.Columns[0].ColumnName;

                    foreach (DataRow row in dt.Rows)
                    {
                        StringBuilder cadena = new StringBuilder();

                        foreach (DataColumn col in dt.Columns)
                        {
                            
                            if (col.ColumnName.Equals("DigitoHorizontal-013AL", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (row[col] != DBNull.Value)
                                cadena.Append(row[col].ToString());
                        }

                        string dvhCalculado = HashHelper_013AL.CalcularSHA256_013AL(cadena.ToString());
                        string dvhGuardado = row["DigitoHorizontal-013AL"].ToString();

                        if (dvhCalculado != dvhGuardado)
                        {
                            errores.Add(new ErrorIntegridad_013AL
                            {
                                Tabla_013AL = tabla,
                                Descripcion_013AL = $"Registro modificado en tabla {tabla}.",
                                IdRegistro_013AL = row[clavePrimaria].ToString()
                            });
                        }
                    }

                    bool dvvOk = VerificarDVV(tabla);

                    if (!dvvOk)
                    {
                        errores.Add(new ErrorIntegridad_013AL
                        {
                            Tabla_013AL = tabla,
                            Descripcion_013AL = $"Se detectaron registros eliminados o alteración masiva en tabla {tabla} (DVV).",
                            IdRegistro_013AL = "-"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la integridad completa.", ex);
            }

            return errores;
        }


    }
}

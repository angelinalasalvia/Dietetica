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
                                    if (col.ColumnName.Equals("DigitoHorizontal-013AL", StringComparison.OrdinalIgnoreCase))
                                        continue;

                                    if (row[col] != DBNull.Value)
                                        cadena.Append(row[col].ToString());
                                }

                                string dvh = HashHelper_013AL.CalcularSHA256_013AL(cadena.ToString());

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
                                throw new Exception("No se pudo leer la factura recién insertada para calcular el DVH.");
                            }

                            transaction.Commit();

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
                if (dt.Rows.Count == 0) return true;

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

                StringBuilder sb = new StringBuilder();
                foreach (DataRow row in dt1.Rows)
                {
                    sb.Append(row["DigitoHorizontal-013AL"].ToString());
                }

                string dvvCalculado = HashHelper_013AL.CalcularSHA256_013AL(sb.ToString());

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

                return dvvGuardado == dvvCalculado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar el DVV.", ex);
            }
        }
        
       

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

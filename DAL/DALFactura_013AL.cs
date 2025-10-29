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
        }

        public bool ActualizarDVH(string tabla)
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

                /*
                var columnasIgnorar = new List<string>
        {
            "DigitoHorizontal_597DG",
            "IntentosFallidos_597DG",
            "Bloqueado_597DG",
            "Activo_597DG",
            "IdiomaPreferido_597DG"
        };*/

                // 5️⃣ Tomo la clave primaria (suponiendo que es la primera columna)
                string clavePrimaria = dt.Columns[0].ColumnName;

                // 6️⃣ Recorro filas para calcular y actualizar DVH
                foreach (DataRow row in dt.Rows)
                {
                    string cadena = "";
                    foreach (DataColumn col in dt.Columns)
                    {
                        /*if (columnasIgnorar.Contains(col.ColumnName))
                            continue;*/

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
        }
        public bool VerificarDVV(string tabla)
        {
            try
            {
                DataTable dt1 = new DataTable();

                // 1️⃣ Traigo los DVH de la tabla indicada
                string consulta1 = $"SELECT [DigitoHorizontal-013AL] FROM {tabla}";
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
                    sb.Append(row["DigitoHorizontal_013AL"].ToString());
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

                string dvvGuardado = dt2.Rows[0]["DVV_013AL"].ToString();

                // 4️⃣ Comparo el DVV calculado con el guardado
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
        }

        public List<ErrorIntegridad_013AL> VerificarIntegridadCompleta(List<string> tablas)
        {
            var errores = new List<ErrorIntegridad_013AL>();

            try
            {
                foreach (var tabla in tablas)
                {
                    // 1️⃣ Traigo todos los registros de la tabla
                    DataTable dt = new DataTable();
                    string consulta = $"SELECT * FROM {tabla}";

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

                    // 2️⃣ Tomo la clave primaria (primera columna)
                    string clavePrimaria = dt.Columns[0].ColumnName;

                    // 3️⃣ Columnas que no se usan para el cálculo del DVH
                    /*var columnasIgnorar = new List<string>
            {
                "DigitoHorizontal_597DG",
                "IntentosFallidos_597DG",
                "Bloqueado_597DG",
                "Activo_597DG",
                "IdiomaPreferido_597DG"
            };*/

                    // 4️⃣ Recorro los registros para verificar DVH
                    foreach (DataRow row in dt.Rows)
                    {
                        string cadena = "";

                        foreach (DataColumn col in dt.Columns)
                        {
                            /*if (columnasIgnorar.Contains(col.ColumnName))
                                continue;*/

                            cadena += row[col].ToString();
                        }

                        string dvhCalculado = HashHelper_013AL.CalcularSHA256_013AL(cadena);
                        string dvhGuardado = row["DigitoHorizontal-013AL"].ToString();

                        if (dvhCalculado != dvhGuardado)
                        {
                            errores.Add(new ErrorIntegridad_013AL
                            {
                                Tabla_597DG = tabla,
                                Descripcion_597DG = $"Registro modificado en tabla {tabla}.",
                                IdRegistro_597DG = row[clavePrimaria].ToString()
                            });
                        }
                    }

                    // 5️⃣ Verifico el DVV de la tabla (usa tu método adaptado anterior)
                    bool dvvOk = VerificarDVV(tabla);

                    if (!dvvOk)
                    {
                        errores.Add(new ErrorIntegridad_013AL
                        {
                            Tabla_597DG = tabla,
                            Descripcion_597DG = $"Se detectaron registros eliminados o alteración masiva en tabla {tabla} (DVV).",
                            IdRegistro_597DG = "-"
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

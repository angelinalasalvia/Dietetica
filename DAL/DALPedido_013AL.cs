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
    public class DALPedido_013AL
    {
        private readonly DALConexiones_013AL conexion = new DALConexiones_013AL();
        SqlCommand com;

        
        public string CrearPedido_013AL(Pedido_013AL pedido)
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
                            string insert = @"
                            INSERT INTO [Pedido-013AL]
                            (
                                [CodCompra-013AL],
                                [Fecha-013AL],
                                [Estado-013AL]
                            )
                            VALUES
                            (
                                @cod,
                                @fecha,
                                @estado
                            )";

                            using (SqlCommand cmd = new SqlCommand(insert, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@cod", pedido.CodCompra_013AL);
                                cmd.Parameters.AddWithValue("@fecha", pedido.Fecha_013AL);
                                cmd.Parameters.AddWithValue("@estado", pedido.Estado_013AL);

                                cmd.ExecuteNonQuery();
                            }


                            DataTable dt = new DataTable();

                            string consulta = @"
                            SELECT * 
                            FROM [Pedido-013AL]
                            WHERE [CodCompra-013AL] = @id";

                            using (SqlCommand cmdSelect = new SqlCommand(consulta, con, transaction))
                            {
                                cmdSelect.Parameters.AddWithValue("@id", pedido.CodCompra_013AL);

                                using (SqlDataAdapter da = new SqlDataAdapter(cmdSelect))
                                {
                                    da.Fill(dt);
                                }
                            }

                            if (dt.Rows.Count == 1)
                            {
                                DataRow row = dt.Rows[0];

                                StringBuilder cadena = new StringBuilder();

                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (col.ColumnName.Equals("DigitoHorizontal-013AL", StringComparison.OrdinalIgnoreCase))
                                        continue;

                                    if (row[col] != DBNull.Value)
                                        cadena.Append(row[col].ToString() + "|");
                                }

                                string dvh = HashHelper_013AL.CalcularSHA256_013AL(cadena.ToString());


                                string updateDVH = @"
                                UPDATE [Pedido-013AL]
                                SET [DigitoHorizontal-013AL] = @dvh
                                WHERE [CodCompra-013AL] = @id";

                                using (SqlCommand cmdDVH = new SqlCommand(updateDVH, con, transaction))
                                {
                                    cmdDVH.Parameters.AddWithValue("@dvh", dvh);
                                    cmdDVH.Parameters.AddWithValue("@id", pedido.CodCompra_013AL);

                                    cmdDVH.ExecuteNonQuery();
                                }
                            }


                            transaction.Commit();


                            ActualizarDVV("Pedido-013AL");

                            return "Pedido registrado correctamente";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error al crear pedido.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error general al crear pedido.", ex);
            }
        }

        
        public string AsignarClientePedido_013AL(int codCompra, int cuil)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"
                    UPDATE [Pedido-013AL]
                    SET [CUILCliente-013AL] = @cuil
                    WHERE [CodCompra-013AL] = @cod";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@cuil", cuil);
                        cmd.Parameters.AddWithValue("@cod", codCompra);

                        con.Open();

                        int filas = cmd.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            return "No se encontró el pedido.";
                        }
                    }
                }

                ActualizarDVH("Pedido-013AL");
                ActualizarDVV("Pedido-013AL");

                return "Cliente asignado correctamente.";
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "DAL: Error al asignar cliente al pedido. " + ex.Message
                );
            }
        }


        public void ActualizarTotalPedido_013AL(int idCompra, decimal total)
        {
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"
            UPDATE [Pedido-013AL]
            SET [Total-013AL] = @total
            WHERE [CodCompra-013AL] = @id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@total", total);
                        cmd.Parameters.AddWithValue("@id", idCompra);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }


                ActualizarDVH("Pedido-013AL");
                ActualizarDVV("Pedido-013AL");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar total del pedido.", ex);
            }
        }


        public List<Pedido_013AL> ListarPedido_013AL()
        {
            List<Pedido_013AL> Lista = new List<Pedido_013AL>();
            try
            {
                using (SqlConnection con = conexion.ObtenerConexion())
                {
                    string query = @"SELECT [CodCompra-013AL], [CUILCliente-013AL], [Fecha-013AL], [Estado-013AL], [Total-013AL], [MetodoPago-013AL], [DigitoHorizontal-013AL] FROM [Pedido-013AL]"; 
                    com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Pedido_013AL()
                            {
                                CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]),
                                CUIL_013AL = Convert.ToInt32(dr["CUILCliente-013AL"]),
                                Fecha_013AL = Convert.ToDateTime(dr["Fecha-013AL"]),
                                Estado_013AL = Convert.ToString(dr["Estado-013AL"]),
                                Total_013AL = Convert.ToInt32(dr["Total-013AL"]),
                                MetPago_013AL = Convert.ToString(dr["MetodoPago-013AL"]),
                                DigitoHorizontal_013AL = Convert.ToString(dr["DigitoHorizontal-013AL"])
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

        
        /*public string RegistrarVentaCompleta_013AL(Pedido_013AL factura, List<Detalle_013AL> detalles)
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

        public List<Pedido_013AL> ListarPedidosPendientes_013AL()
        {
                    List<Pedido_013AL> lista = new List<Pedido_013AL>();

                    try
                    {
                        using (SqlConnection con =
                            conexion.ObtenerConexion())
                        {
                            string query = @"
                            SELECT *
                            FROM [Pedido-013AL]
                            WHERE [Estado-013AL] =
                            'Pendiente'";

                            SqlCommand cmd = new SqlCommand(query, con);

                            con.Open();

                            SqlDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                            {
                                lista.Add(
                                    new Pedido_013AL()
                                    {
                                        CodCompra_013AL =
                                            Convert.ToInt32(
                                                dr["CodCompra-013AL"]
                                            ),

                                        CUIL_013AL =
                                            Convert.ToInt32(
                                                dr["CUILCliente-013AL"]
                                            ),

                                        Fecha_013AL =
                                            Convert.ToDateTime(
                                                dr["Fecha-013AL"]
                                            ),

                                        Estado_013AL =
                                            dr["Estado-013AL"]
                                            .ToString(),

                                        Total_013AL =
                                            Convert.ToInt32(
                                                dr["Total-013AL"]
                                            )
                                    });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            "Error al listar pedidos pendientes: "
                            + ex.Message
                        );
                    }

                    return lista;
        }

        public string ActualizarEstadoPedido_013AL(int codCompra, string estado)
        {
                        try
                        {
                            using (SqlConnection con =
                                conexion.ObtenerConexion())
                            {
                                string query = @"
                                UPDATE [Pedido-013AL]
                                SET [Estado-013AL] = @estado
                                WHERE [CodCompra-013AL] =
                                @cod";

                                SqlCommand cmd =
                                    new SqlCommand(query, con);

                                cmd.Parameters.AddWithValue(
                                    "@estado",
                                    estado
                                );

                                cmd.Parameters.AddWithValue(
                                    "@cod",
                                    codCompra
                                );

                                con.Open();

                                cmd.ExecuteNonQuery();
                            }

                            return "OK";
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(
                                "Error al actualizar estado: "
                                + ex.Message
                            );
                        }
        }
        public Pedido_013AL BuscarPedidoPorId_013AL(int codCompra) 
        { 
            Pedido_013AL pedido = null; 
            try 
            { 
                using (SqlConnection con = conexion.ObtenerConexion()) 
                { 
                    string query = @" SELECT * FROM [Pedido-013AL] WHERE [CodCompra-013AL] = @CodCompra"; 
                    SqlCommand com = new SqlCommand(query, con); 
                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra; 
                    con.Open(); 
                    SqlDataReader dr = com.ExecuteReader(); 
                    if (dr.Read()) 
                    { 
                        pedido = new Pedido_013AL() 
                        { 
                            CodCompra_013AL = Convert.ToInt32(dr["CodCompra-013AL"]), 
                            CUIL_013AL = Convert.ToInt32(dr["CUILCliente-013AL"]), 
                            Fecha_013AL = Convert.ToDateTime(dr["Fecha-013AL"]), 
                            Estado_013AL = dr["Estado-013AL"].ToString(), 
                            Total_013AL = Convert.ToInt32(dr["Total-013AL"]) 
                        }; 
                    } 
                } 
            } 
            catch (Exception ex) 
            { 
                throw new Exception("Error al buscar pedido: " + ex.Message); 
            } 
            return pedido; 
        }
        public void ActualizarMetodoPago_013AL(int codCompra, string metodoPago) 
        { 
            try 
            { 
                using (SqlConnection con = conexion.ObtenerConexion()) 
                { 
                    string query = @" UPDATE [Pedido-013AL] SET [MetodoPago-013AL] = @Metodo, [Estado-013AL] = @Estado WHERE [CodCompra-013AL] = @CodCompra"; 
                    SqlCommand com = new SqlCommand(query, con); 
                    com.Parameters.Add("@Metodo", SqlDbType.NVarChar).Value = metodoPago; 
                    com.Parameters.Add("@Estado", SqlDbType.NVarChar).Value = "Cobrado"; 
                    com.Parameters.Add("@CodCompra", SqlDbType.Int).Value = codCompra; 
                    con.Open(); 
                    com.ExecuteNonQuery(); 
                } 
            } 
            catch (Exception ex) 
            { 
                throw new Exception("Error al actualizar método pago: " + ex.Message); 
            } 
        }
        public List<Pedido_013AL> ListarPedidosAprobados_013AL()
        {
            List<Pedido_013AL> lista = new List<Pedido_013AL>();

            try
            {
                using (SqlConnection con =
                    conexion.ObtenerConexion())
                {
                    string query = @"
                            SELECT *
                            FROM [Pedido-013AL]
                            WHERE [Estado-013AL] =
                            'Aprobado'";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        lista.Add(
                            new Pedido_013AL()
                            {
                                CodCompra_013AL =
                                    Convert.ToInt32(
                                        dr["CodCompra-013AL"]
                                    ),

                                CUIL_013AL =
                                    Convert.ToInt32(
                                        dr["CUILCliente-013AL"]
                                    ),

                                Fecha_013AL =
                                    Convert.ToDateTime(
                                        dr["Fecha-013AL"]
                                    ),

                                Estado_013AL =
                                    dr["Estado-013AL"]
                                    .ToString(),

                                Total_013AL =
                                    Convert.ToInt32(
                                        dr["Total-013AL"]
                                    )
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error al listar pedidos pendientes: "
                    + ex.Message
                );
            }

            return lista;
        }
    }
}

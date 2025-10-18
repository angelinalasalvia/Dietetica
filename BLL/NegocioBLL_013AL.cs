using BE_013AL;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_013AL.Composite;
using Servicios;
using System.Globalization;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using DAL_013AL;

namespace BLL_013AL
{
    public class NegocioBLL_013AL
    {
        public DALConexiones_013AL dal = new DALConexiones_013AL();

        public List<Factura_013AL> ListarCompraCliente_013AL()
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            return dal.ListarFactura_013AL();
        }
        
        public List<Detalle_013AL> ListarCompraProducto_013AL()
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            return dal.ListarCompraProducto_013AL();
        }

        public string AgregarCompraCliente_013AL(int idc, int cuil, string metpago)
        {
            Factura_013AL cc = new Factura_013AL();
            cc.CodCompra_013AL = idc;
            cc.CUIL_013AL = cuil;
            
            cc.MetPago_013AL = metpago;
            return dal.AgregarCompraCliente_013AL(cc);
        }
        public string EliminarDetalle_013AL(int id)
        {
            return dal.EliminarDetalle_013AL(id);
        }

        public string ActualizarCantidadCompraProducto_013AL(int codCompra, int codProducto, int nuevaCantidad)
        {
            return dal.ActualizarCantidadCompraProducto_013AL(codCompra, codProducto, nuevaCantidad);
        }

        public string ObtenerNombreProducto_013AL(int idProducto)
        {
            return dal.ObtenerNombreProducto_013AL(idProducto);
        }
        
        public string AgregarCompraProducto_013AL(int idc, int idp, int cant, int pu)
        {
            Detalle_013AL cp = new Detalle_013AL();
            cp.CodCompra_013AL = idc;
            cp.CodProducto_013AL = idp;
            cp.Cantidad_013AL = cant;
            cp.PrecioUnitario_013AL = pu;
            return dal.AgregarCompraProducto_013AL(cp);
        }

        //NUEVO
        public string RegistrarVentaCompleta_013AL(Factura_013AL factura, List<Detalle_013AL> detalles)
        {
            return dal.RegistrarVentaCompleta_013AL(factura, detalles);
        }
        public void DescontarStock_013AL(int idProducto, int cantidad)
        {
            dal.DescontarStock_013AL(idProducto, cantidad);
        }

        public void RevertirStock_013AL(int idProducto, int cantidad)
        {
            dal.RevertirStock_013AL(idProducto, cantidad);
        }
        //NUEVO

        public Producto_013AL Devolver_Producto_Buscado_x_Id_013AL(int id)
        {
            Producto_013AL producto = new Producto_013AL();

            DataTable dt = dal.BuscarProductoPorId_013AL(id);

            if (dt.Rows.Count > 0)
            {
                DataRow fila = dt.Rows[0];

                producto.CodProducto_013AL = Convert.ToInt32(fila[0]);
                producto.Nombre_013AL = fila[1].ToString();
                producto.Stock_013AL = Convert.ToInt32(fila[2]);
                producto.Precio_013AL = Convert.ToInt32(fila[3]);
                producto.Imagen_013AL = (byte[])fila[4];

                return producto;
            }

            return null;
        }

        

        public List<Producto_013AL> Devolver_Lista_Productos_013AL()
        {
            List<Producto_013AL> lista = new List<Producto_013AL>();
            DataTable dt = dal.ListaProductos_013AL();

            foreach (DataRow fila in dt.Rows)
            {
                Producto_013AL producto = new Producto_013AL
                {
                    CodProducto_013AL = Convert.ToInt32(fila[0]),
                    Nombre_013AL = fila[1].ToString(),
                    Stock_013AL = Convert.ToInt32(fila[2]),
                    Precio_013AL = Convert.ToInt32(fila[3]),
                    Imagen_013AL = (byte[])fila[4]
                };

                lista.Add(producto);
            }

            return lista;
        }




        /*public string ModificarStock(int id, int stock)
        {
            Producto obj = new Producto();
            obj.IdProducto = id;
            obj.Stock = stock;
            return dal.ModificarStock(obj);
        }*/
       
        
        /*public void ActualizarStockProducto_013AL(Producto_013AL producto)
        {
            

            
                dal.ActualizarStockProducto_013AL(producto.CodProducto_013AL, producto.Stock_013AL);
            
          
        }*/



        public int ObtenerSiguienteIdCompra_013AL()
        {
            int ultimoId = dal.ObtenerUltimoIdCompra_013AL();
            return ultimoId + 1; // Incrementamos en 1 para obtener el siguiente ID
        }
      
        public List<Producto_013AL> BuscarProductoxNombre_013AL(string valor)
        {
            DataTable dt = dal.BuscarProductoxNombre_013AL(valor);
            List<Producto_013AL> productos = new List<Producto_013AL>();

            foreach (DataRow row in dt.Rows)
            {
                productos.Add(new Producto_013AL
                {
                    CodProducto_013AL = Convert.ToInt32(row["CodProducto-013AL"]),
                    Nombre_013AL = row["Nombre-013AL"].ToString(),
                    Precio_013AL = Convert.ToInt32(row["Precio-013AL"]),
                    Stock_013AL = Convert.ToInt32(row["Stock-013AL"]),
                    Imagen_013AL = (byte[])row["Imagen-013AL"]
                });
            }

            return productos;
        }
        //Compra

        public List<Producto_013AL> ListarProductosPocoStock_013AL()
        {
            return dal.ListarProductosPocoStock_013AL();
        }

        public List<Proveedor_013AL> ListarProveedores_013AL()
        {
            return dal.ListarProveedores_013AL();
        }

        public bool ExisteCuit_013AL(int cuit)
        {
            return dal.VerificarCuit_013AL(cuit);
        }

        public string PreregistrarProveedor_013AL(string nombre, int cuit, string razonsocial)
        {
            Proveedor_013AL obj = new Proveedor_013AL();
            obj.NombreProveedor_013AL = nombre;
            obj.CUIT_013AL = cuit;
            obj.RazonSocial_013AL = razonsocial;
            /*if (ExisteCuit(cuit))
            {
                return "El CUIT ingresado ya está registrado.";
            }*/
            return dal.PreregistrarProveedor_013AL(obj);
        }

        /*public string AgregarSCotizacion(int valor)
        {
            SolicitudCotizacion obj = new SolicitudCotizacion();
            obj.CUITProveedor = valor;
            return dal.AgregarSCotizacion(obj);
        }*/

        public int AgregarSCotizacion_013AL(int valor)
        {
            SolicitudCotizacion_013AL obj = new SolicitudCotizacion_013AL();
            obj.CUITProveedor_013AL = valor;
            return dal.AgregarSCotizacion_013AL(obj);
        }

        public string AgregarDetalleSC_013AL(int sc, int prod, int cant)
        {
            DetalleSolicitudC_013AL obj = new DetalleSolicitudC_013AL();
            obj.CodSCotizacion_013AL = sc;
            obj.CodProducto_013AL = prod;
            obj.Cantidad_013AL = cant;
            return dal.AgregarDetalleSC_013AL(obj);
        }

        public Producto_013AL ObtenerProductoPorId_013AL(int idProducto)
        {
            return dal.ObtenerProductoPorId_013AL(idProducto);
        }
        public DetalleSolicitudC_013AL ListarProductosOC_013AL(int codsc)
        {
            return dal.ListarProductosOC_013AL(codsc);
        }
        public string AgregarProducto_013AL(string nombre, int cant, int precio, byte[] image, string desc)
        {
            Producto_013AL p = new Producto_013AL();
            p.Nombre_013AL = nombre;
            p.Stock_013AL = cant;
            p.Precio_013AL = precio;
            p.Imagen_013AL = image;
            p.Descripcion_013AL = desc;
            return dal.AgregarProducto_013AL(p);
        }

        public DataTable ListarProductosConImagen_013AL()
        {
            return dal.ObtenerProductosConImagen_013AL();
        }

        public string ModificarProducto_013AL(int id, string nombre, int cant, int precio, byte[] image, string desc)
        {
            Producto_013AL p = new Producto_013AL();
            p.CodProducto_013AL = id;
            p.Nombre_013AL = nombre;
            p.Stock_013AL = cant;
            p.Precio_013AL = precio;
            p.Imagen_013AL = image;
            p.Descripcion_013AL = desc;
            return dal.ModificarProducto_013AL(p);
        }

        public string EliminarProducto_013AL(int id)
        {
            Producto_013AL p = new Producto_013AL();
            p.CodProducto_013AL = id;
            return dal.ModificarProducto_013AL(p);
        }

        public string EliminarDetalleSC_013AL(int idp, int codsc)
        {
            DetalleSolicitudC_013AL p = new DetalleSolicitudC_013AL();
            p.CodProducto_013AL = idp;
            p.CodSCotizacion_013AL = codsc;
            return dal.EliminarDetalleSC_013AL(p);
        }

        public string ModificarProveedor_013AL(int cuit, string apellido, string dom, string mail, int tel)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            p.ApellidoProveedor_013AL = apellido;
            p.Domicilio_013AL = dom;
            p.Mail_013AL = mail;
            p.Telefono_013AL = tel;
            return dal.ModificarProveedor_013AL(p);
        }

        /*public string EncriptarDatosProveedor_013AL(int cuit, string dom)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            
            p.Domicilio_013AL = dom;

            return dal.EncriptarDatosProveedor_013AL(p);
        }*/
        public DataTable ListarProveedoresDGV_013AL()
        {
            return dal.ListarProveedoresDGV_013AL();
        }
        public string ModificarProveedor2_013AL(int cuit, string nombre, string apellido, string dom, string mail, string rs, int tel)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            p.NombreProveedor_013AL = nombre;
            p.ApellidoProveedor_013AL = apellido;
            p.Domicilio_013AL = dom;
            p.Mail_013AL = mail;
            p.RazonSocial_013AL = rs;
            p.Telefono_013AL = tel;
            return dal.ModificarProveedor2_013AL(p);
        }
        public string EliminarProveedor_013AL(int cuit)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            return dal.EliminarProveedor_013AL(p);
        }
        public List<SolicitudCotizacion_013AL> ListarSCotizacion_013AL()
        {
            return dal.ListarSCotizacion_013AL();
        }

        public int GuardarOrdenCompra_013AL(OrdenCompra_013AL nuevaOrden)
        {
            int codOrdenCompra;
            // Llamada al método en la capa DAL para ejecutar el procedimiento almacenado y obtener el codOrdenCompra
            codOrdenCompra = dal.GuardarOrdenCompra_013AL(nuevaOrden); // Aquí `GuardarOrdenCompra` en la DAL manejará el parámetro OUTPUT
            return codOrdenCompra;
        }

        public Proveedor_013AL ObtenerDatosProveedor_013AL(int codOrdenCompra)
        {
            return dal.TraerDatosProveedor_013AL(codOrdenCompra);
        }
        public List<OrdenCompra_013AL> ListarOrdenCompra_013AL()
        {
            return dal.ListarOrdenCompra_013AL();
        }
        public int ObtenerCodOrdenCompra_013AL(int idSolicitud, int cuitProveedor, DateTime fecha)
        {
            return dal.ObtenerCodOrdenCompra_013AL(idSolicitud, cuitProveedor, fecha); // Aquí llamas al método en la DAL
        }

        public string RegistrarCompra_013AL(int id, int stock)
        {
            Producto_013AL p = new Producto_013AL();
            p.CodProducto_013AL = id;
            p.Stock_013AL = stock;
            
            return dal.RegistrarCompra_013AL(p);
        }
        public DataTable ListarOrdenesCompra_013AL()
        {
            // Llamar al DAL para obtener las órdenes de compra
            return dal.ListarOrdenesCompra_013AL();
        }
        public DataTable ListarProductosPorOrden_013AL(int codOrdenCompra)
        {
            return dal.ObtenerProductosPorOrden_013AL(codOrdenCompra);
        }
        public string ActualizarEstadoCompleto_013AL(int codOrdenCompra, bool completo)
        {
            return dal.ActualizarEstadoCompleto_013AL(codOrdenCompra, completo);
        }

        /*ArrayList AL;
        public bool Alta_Compra_Producto(Producto producto, int CC, int Cant)
        {
            AL = new ArrayList();

            string Consulta = "AgregarCompraProducto";

            SqlParameter Param1 = new SqlParameter();
            Param1.ParameterName = "@idc";
            Param1.Value = CC;
            Param1.SqlDbType = SqlDbType.Int;
            AL.Add(Param1);

            SqlParameter Param2 = new SqlParameter();
            Param2.ParameterName = "@idp";
            Param2.Value = producto.IdProducto;
            Param2.SqlDbType = SqlDbType.Int;

            AL.Add(Param2);

            SqlParameter Param3 = new SqlParameter();
            Param3.ParameterName = "@cant";
            Param3.Value = Cant;
            Param3.SqlDbType = SqlDbType.Int;
            AL.Add(Param3);

            SqlParameter Param4 = new SqlParameter();
            Param4.ParameterName = "@sub";
            Param4.Value = Convert.ToDecimal(Cant * producto.Precio);
            Param4.SqlDbType = SqlDbType.Decimal;
            AL.Add(Param4);

            return dal.Escribir(Consulta, AL);
        }*/
    }
}

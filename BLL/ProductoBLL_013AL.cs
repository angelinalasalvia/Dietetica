using BE_013AL;
using DAL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductoBLL_013AL
    {
        public DALProducto_013AL dal = new DALProducto_013AL();

        public string ObtenerNombreProducto_013AL(int idProducto)
        {
            return dal.ObtenerNombreProducto_013AL(idProducto);
        }
        public void DescontarStock_013AL(int idProducto, int cantidad)
        {
            dal.DescontarStock_013AL(idProducto, cantidad);
        }

        public void RevertirStock_013AL(int idProducto, int cantidad)
        {
            dal.RevertirStock_013AL(idProducto, cantidad);
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
        public List<Producto_013AL> ListarProductosPocoStock_013AL()
        {
            return dal.ListarProductosPocoStock_013AL();
        }
        public Producto_013AL ObtenerProductoPorId_013AL(int idProducto)
        {
            return dal.ObtenerProductoPorId_013AL(idProducto);
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
    }
}

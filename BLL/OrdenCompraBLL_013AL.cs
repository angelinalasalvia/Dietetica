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
    public class OrdenCompraBLL_013AL
    {
        public DALOrdenCompra_013AL dal = new DALOrdenCompra_013AL();
           
        public int GuardarOrdenCompra_013AL(OrdenCompra_013AL nuevaOrden)
        {
            int codOrdenCompra;
            codOrdenCompra = dal.GuardarOrdenCompra_013AL(nuevaOrden); 
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
            return dal.ObtenerCodOrdenCompra_013AL(idSolicitud, cuitProveedor, fecha); 
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
    }
}

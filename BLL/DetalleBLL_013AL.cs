using BE_013AL;
using DAL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DetalleBLL_013AL
    {
        public DALDetalle_013AL dal = new DALDetalle_013AL();

        public List<Detalle_013AL> ListarDetalle_013AL()
        {
            return dal.ListarDetalle_013AL();
        }
        public string AgregarDetalle_013AL(Detalle_013AL detalle)
        {
            return dal.AgregarDetalle_013AL(detalle);
        }
        public Detalle_013AL ObtenerDetalle_013AL(int codCompra, int codProducto)
        {
            return dal.ObtenerDetalle_013AL(codCompra, codProducto);
        }
        public string ActualizarCantidadDetalle_013AL(int codCompra, int codProducto, int cantidad)
        {
            return dal.ActualizarCantidadDetalle_013AL(
                codCompra,
                codProducto,
                cantidad);
        }
        public int CalcularTotalPedido_013AL(int codCompra)
        {
            return dal.CalcularTotalPedido_013AL(codCompra);
        }
        
        public int ObtenerSiguienteIdCompra_013AL()
        {
            int ultimoId = dal.ObtenerUltimoIdCompra_013AL();
            return ultimoId + 1; 
        }
        public string EliminarDetalle_013AL(int codCompra, int codProducto) 
        { 
            return dal.EliminarDetalle_013AL(codCompra, codProducto); 
        }
        public List<Detalle_013AL> ListarDetallePorCompra_013AL(int codCompra) 
        { 
            return dal.ListarDetallePorCompra_013AL(codCompra); 
        }
        public string ActualizarPromocionDetalle_013AL(int codCompra, int codProducto, string promocionAplicada, decimal descuentoAplicado)
        {
            return dal.ActualizarPromocionDetalle_013AL(codCompra, codProducto, promocionAplicada, descuentoAplicado);
        }
    }
}

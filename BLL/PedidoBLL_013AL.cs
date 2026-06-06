using BE;
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
    public class PedidoBLL_013AL
    {
        public DALPedido_013AL dal = new DALPedido_013AL();

        public List<Pedido_013AL> ListarPedido_013AL()
        {
            return dal.ListarPedido_013AL();
        }
              
        public string CrearPedido_013AL(Pedido_013AL pedido)
        {
                try
                {
                    if (pedido == null)
                        throw new Exception("El pedido es nulo.");

                    if (string.IsNullOrWhiteSpace(pedido.Estado_013AL))
                        throw new Exception("Debe ingresar un estado.");

                    return dal.CrearPedido_013AL(pedido);
                }
                catch (Exception ex)
                {
                    throw new Exception("BLL: Error al crear pedido.", ex);
                }
        }
        
        public string AsignarClientePedido_013AL(int codCompra, int cuil)
        {
            try
            {
                if (codCompra <= 0)
                {
                    throw new Exception("Código de compra inválido.");
                }

                if (cuil <= 0)
                {
                    throw new Exception("CUIL inválido.");
                }

                return dal.AsignarClientePedido_013AL(codCompra, cuil);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "BLL: Error al asignar cliente al pedido. " + ex.Message
                );
            }
        }



        public void ActualizarTotalPedido_013AL(int idCompra, decimal total)
        {
            dal.ActualizarTotalPedido_013AL(idCompra, total);
        }

        public bool ActualizarDVH(string tabla) => dal.ActualizarDVH(tabla);

        public bool VerificarDVV(string tabla) => dal.VerificarDVV(tabla);

        public bool ActualizarDVV(string tabla) => dal.ActualizarDVV(tabla);

        public List<ErrorIntegridad_013AL> VerificarIntegridadCompleta(List<string> tablas) => dal.VerificarIntegridadCompleta(tablas);

        public void InicializarDVH_DVV(string tabla)
        {
            dal.ActualizarDVH(tabla);
            dal.ActualizarDVV(tabla);
        }

        public List<Pedido_013AL> ListarPedidosPendientes_013AL() 
        { 
            return dal.ListarPedidosPendientes_013AL(); 
        }
        public string ActualizarEstadoPedido_013AL(int codCompra, string estado) 
        { 
            return dal.ActualizarEstadoPedido_013AL(codCompra, estado); 
        }
        public Pedido_013AL BuscarPedidoPorId_013AL(int codCompra) 
        { 
            return dal.BuscarPedidoPorId_013AL(codCompra); 
        }
        public void ActualizarMetodoPago_013AL(int codCompra, string metodo) 
        { 
            dal.ActualizarMetodoPago_013AL(codCompra, metodo); 
        }
        public List<Pedido_013AL> ListarPedidosAprobados_013AL()
        {
            return dal.ListarPedidosAprobados_013AL();
        }
    }

}

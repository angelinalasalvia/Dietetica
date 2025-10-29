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

        public string EliminarDetalle_013AL(int id)
        {
            return dal.EliminarDetalle_013AL(id);
        }

        public string ActualizarCantidadDetalle_013AL(int codCompra, int codProducto, int nuevaCantidad)
        {
            return dal.ActualizarCantidadDetalle_013AL(codCompra, codProducto, nuevaCantidad);
        }
        public string AgregarDetalle_013AL(int idc, int idp, int cant, int pu)
        {
            Detalle_013AL cp = new Detalle_013AL();
            cp.CodCompra_013AL = idc;
            cp.CodProducto_013AL = idp;
            cp.Cantidad_013AL = cant;
            cp.PrecioUnitario_013AL = pu;
            return dal.AgregarDetalle_013AL(cp);
        }
        public int ObtenerSiguienteIdCompra_013AL()
        {
            int ultimoId = dal.ObtenerUltimoIdCompra_013AL();
            return ultimoId + 1; 
        }

    }
}

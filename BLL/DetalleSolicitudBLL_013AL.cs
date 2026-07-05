using BE_013AL;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DetalleSolicitudBLL_013AL
    {
        public DALDetalleSolicitud_013AL dal = new DALDetalleSolicitud_013AL();

        public string AgregarDetalleSC_013AL(int sc, int prod, int cant)
        {
            DetalleSolicitudC_013AL obj = new DetalleSolicitudC_013AL();
            obj.CodSCotizacion_013AL = sc;
            obj.CodProducto_013AL = prod;
            obj.CantidadPedida_013AL = cant;
            return dal.AgregarDetalleSC_013AL(obj);
        }
        public string EliminarDetalleSC_013AL(int idp, int codsc)
        {
            DetalleSolicitudC_013AL p = new DetalleSolicitudC_013AL();
            p.CodProducto_013AL = idp;
            p.CodSCotizacion_013AL = codsc;
            return dal.EliminarDetalleSC_013AL(p);
        }
        public List<DetalleSolicitudC_013AL> ListarProductosOC_013AL(int codsc)
        {
            return dal.ListarProductosOC_013AL(codsc);
        }
        public string ActualizarCantidadDetalleSC_013AL(int codsc, int idp, int cantidad)
        {
            DetalleSolicitudC_013AL detalle = new DetalleSolicitudC_013AL();

            detalle.CodSCotizacion_013AL = codsc;
            detalle.CodProducto_013AL = idp;
            detalle.CantidadPedida_013AL = cantidad;

            return dal.ActualizarCantidadDetalleSC_013AL(detalle);
        }
        public void ActualizarCantidadRecibida_013AL(int codOrden, int codProducto, int cantidad)
        {
            dal.ActualizarCantidadRecibida_013AL(codOrden, codProducto, cantidad);
        }
    }
}

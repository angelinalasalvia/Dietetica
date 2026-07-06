using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLL
{
    public class SolicitudPromocionBLL_013AL
    {
        public DALSolicitudPromocion_013AL dal = new DALSolicitudPromocion_013AL();
        public void AgregarSolicitudPromocion_013AL(SolicitudPromocion_013AL solicitud)
        {
            dal.AgregarSolicitudPromocion_013AL(solicitud);
        }
        public int ContarSolicitudesPendientes_013AL()
        {
            return dal.ContarSolicitudesPendientes_013AL();
        }
        public DataTable ListarSolicitudesPendientes_013AL()
        {
            return dal.ListarSolicitudesPendientes_013AL();
        }
        public DataTable TraerSolicitudPorId_013AL(int codSolicitud)
        {
            return dal.TraerSolicitudPorId_013AL(codSolicitud);
        }
        public bool TuvoPromocionAnterior_013AL(int codLote)
        {
            return dal.TuvoPromocionAnterior_013AL(codLote);
        }
        public bool ExistePromocionActiva_013AL(int codLote)
        {
            return dal.ExistePromocionActiva_013AL(codLote);
        }
        public bool ExisteSolicitudPendiente_013AL(int codLote)
        {
            return dal.ExisteSolicitudPendiente_013AL(codLote);
        }
        public void RechazarSolicitud_013AL(int codSolicitud)
        {
            dal.RechazarSolicitud_013AL(codSolicitud);
        }
        public void AprobarSolicitud_013AL(Promocion_013AL promo, SolicitudPromocion_013AL solicitud)
        {
            dal.AprobarSolicitud_013AL(promo, solicitud);
        }
    }
}

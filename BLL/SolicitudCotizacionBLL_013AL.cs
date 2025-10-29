using BE_013AL;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SolicitudCotizacionBLL_013AL
    {
        public DALSolicitudCotizacion_013AL dal = new DALSolicitudCotizacion_013AL();

        public int AgregarSCotizacion_013AL(int valor)
        {
            SolicitudCotizacion_013AL obj = new SolicitudCotizacion_013AL();
            obj.CUITProveedor_013AL = valor;
            return dal.AgregarSCotizacion_013AL(obj);
        }
        public List<SolicitudCotizacion_013AL> ListarSCotizacion_013AL()
        {
            return dal.ListarSCotizacion_013AL();
        }

    }
}

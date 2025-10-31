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
    public class FacturaBLL_013AL
    {
        public DALFactura_013AL dal = new DALFactura_013AL();

        public List<Factura_013AL> ListarFactura_013AL()
        {
            return dal.ListarFactura_013AL();
        }

        /*public string AgregarFactura_013AL(int idc, int cuil, string metpago)
        {
            Factura_013AL cc = new Factura_013AL();
            cc.CodCompra_013AL = idc;
            cc.CUIL_013AL = cuil;

            cc.MetPago_013AL = metpago;
            return dal.AgregarFactura_013AL(cc);
        }*/

        public string RegistrarVentaCompleta_013AL(Factura_013AL factura, List<Detalle_013AL> detalles)
        {
            return dal.RegistrarVentaCompleta_013AL(factura, detalles);
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


    }

}

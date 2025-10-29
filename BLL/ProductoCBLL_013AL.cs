using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductoCBLL_013AL
    {
        public DALProductoC_013AL dal = new DALProductoC_013AL();

        public DataTable ListarProductosC_013AL()
        {
            return dal.ListarProductosC_013AL();
        }

        public void RestaurarVersionProducto_013AL(int codProductoC)
        {
            try
            {
                dal.RestaurarVersionProducto_013AL(codProductoC);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la lógica de negocio al restaurar la versión del producto: " + ex.Message);
            }
        }

        public DataTable ConsultaProductosC_013AL(int? id, string nombre, DateTime? fechaInicio, DateTime? fechaFin)
        {
            return dal.ConsultaProductosC_013AL(id, nombre, fechaInicio, fechaFin);
        }

    }
}

using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class TraduccionBLL_013AL
    {
        private DALTraduccion_013AL dal = new DALTraduccion_013AL();

        public List<Traduccion_013AL> ObtenerPorIdioma(int idIdioma)
        {
            return dal.ObtenerPorIdioma(idIdioma);
        }
        public void ActualizarTexto(int idTraduccion, string nuevoTexto)
        {
            dal.ActualizarTexto(idTraduccion, nuevoTexto);
        }
    }
}

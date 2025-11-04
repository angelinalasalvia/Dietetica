using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLL
{
    public class IdiomaBLL_013AL
    {
        DALIdioma_013AL dalIdioma = new DALIdioma_013AL();
        DALTraduccion_013AL dalTraduccion = new DALTraduccion_013AL();
        public int CrearIdiomaConTraducciones(Idioma_013AL idioma)
        {
            const int IDIOMA_ESPANOL = LanguageManager_013AL.IDIOMA_ESPANOL;

            int idNuevo = dalIdioma.Agregar(idioma);
            dalTraduccion.ClonarTraduccionesDesdeEspanol(idNuevo, IDIOMA_ESPANOL);

            return idNuevo;
        }
        public List<Idioma_013AL> ListarIdiomas_013AL()
        {
            return dalIdioma.ListarIdiomas_013AL();
        }
    }
}

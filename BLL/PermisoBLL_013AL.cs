using BE_013AL;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_013AL.Composite;
using Servicios;
using System.Globalization;
using System.Data;
using System.Windows.Forms;
using static BLL_013AL.PermisoBLL_013AL;
using DAL_013AL;

namespace BLL_013AL
{
    public class PermisoBLL_013AL
    {
        DALPermiso_013AL dal = new DALPermiso_013AL();

        
        public string ObtenerNombrePermiso_013AL(int idPermiso)
        {
            return dal.ObtenerNombrePermiso_013AL(idPermiso);
        }

        
        public List<Rol_013AL> TraerListaPermisos_013AL()
        {
            List<Rol_013AL> lista = new List<Rol_013AL>();
            DataTable tabla = dal.TraerListaPermisos_013AL();

            foreach (DataRow row in tabla.Rows)
            {
                int tipo = Convert.ToInt32(row["Tipo-013AL"]);
                Rol_013AL componente;

                if (tipo == 1) // Es una Familia
                {
                    componente = new Familia_013AL()
                    {
                        Cod_013AL = Convert.ToInt32(row["CodPermiso-013AL"]),
                        Nombre_013AL = row["NombrePermiso-013AL"].ToString(),
                        Tipo_013AL = "Familia-013AL"
                    };
                }
                else // Es un Permiso Simple
                {
                    componente = new Permiso_013AL()
                    {
                        Cod_013AL = Convert.ToInt32(row["CodPermiso-013AL"]),
                        Nombre_013AL = row["NombrePermiso-013AL"].ToString(),
                        Tipo_013AL = "Simple-013AL"
                    };
                }

                lista.Add(componente);
            }

            return lista;

            
        }
        public List<Rol_013AL> TraerListaPermisosHijo_013AL()
        {
            List<Rol_013AL> lista = new List<Rol_013AL>();
            DataTable tabla = dal.TraerListaPermisos_013AL();

            Rol_013AL permiso = null;
            foreach (DataRow row in tabla.Rows)
            {
                if (Convert.ToBoolean(row[2]) == false)
                {
                    permiso = new Permiso_013AL() { Cod_013AL = Convert.ToInt32(row[0]), Nombre_013AL = row[1].ToString(), Tipo_013AL = "Simple" };
                    lista.Add(permiso);
                }
            }
            return lista;
        }

    }
}

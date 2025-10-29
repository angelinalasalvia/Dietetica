using BE_013AL.Composite;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class RolBLL_013AL
    {
        DALRol_013AL dal = new DALRol_013AL();
                
        public List<Rol_013AL> ListarPermisos_013AL(string dni)
        {
            return dal.ListarPermisos_013AL(dni);
        }
        public List<Rol_013AL> TraerListaHijos_013AL(int idPadre)
        {
            return dal.TraerListaHijos_013AL(idPadre);
        }
        public List<Rol_013AL> TraerListaPermisosRol_013AL(int idRol)
        {
            return dal.TraerListaPermisosRol_013AL(idRol);
        }
        public bool EliminarPermisoRol_013AL(int idRol, int idPermiso)
        {
            return dal.EliminarPermisoRol_013AL(idRol, idPermiso);
        }
        public int CrearRol_013AL(string NombreRol)
        {
            return dal.CrearRol_013AL(NombreRol);
        }
        public string EliminarRol_013AL(int id)
        {
            return dal.EliminarRol_013AL(id);
        }
        public string ModificarRol_013AL(int id, string nombre)
        {
            if (ExisteRol_013AL(nombre))
            {
                return "Ya existe un rol con ese nombre.";
            }

            return dal.ModificarRol_013AL(id, nombre);
        }

        public bool ExisteRol_013AL(string nombre)
        {
            List<Familia_013AL> listaRol = TraerListaRoles_013AL();
            return listaRol.Any(f => f.Nombre_013AL.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
        public Dictionary<int, string> ListarRoles_013AL()
        {
            return dal.ListarRoles_013AL();
        }
        public List<Familia_013AL> TraerListaRoles_013AL()
        {
            List<Familia_013AL> lista = new List<Familia_013AL>();
            DataTable tabla = dal.TraerListaRoles_013AL();

            foreach (DataRow row in tabla.Rows)
            {
                Familia_013AL rol = new Familia_013AL() { Cod_013AL = Convert.ToInt32(row[0]), Nombre_013AL = row[1].ToString() };
                lista.Add(rol);
            }
            return lista;
        }
        public bool RolTieneRelaciones_013AL(int idRol)
        {
            return dal.VerificarRelacionesRol_013AL(idRol);
        }
    }
}

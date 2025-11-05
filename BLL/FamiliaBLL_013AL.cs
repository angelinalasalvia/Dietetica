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
    public class FamiliaBLL_013AL
    {
        DALFamilia_013AL dal = new DALFamilia_013AL();

        public string InsertarFamiliaRol_013AL(int rol, int per)
        {
            try
            {
                return dal.InsertarFamiliaRol_013AL(rol, per);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la capa BLL", ex);
            }
        }
        public bool AsignarHijosAFamilia_013AL(int idFamilia, List<int> hijos)
        {
            int asignados = 0;
            foreach (int idHijo in hijos)
            {
                bool insertado = dal.RegistrarHijoEnFamilia_013AL(idFamilia, idHijo);
                if (insertado)
                    asignados++;
            }

            return asignados > 0; 
        }
        public bool VerificarPermisoEnFamilia_013AL(int permisoId, int familiaId)
        {
            return dal.VerificarPermisoEnFamilia_013AL(permisoId, familiaId);
        }
        public string EliminarPermisoDeFamilia_013AL(int idPermiso, int idFamilia)
        {
            try
            {
                dal.EliminarPermisoDeFamilia_013AL(idPermiso, idFamilia);
                return "Permiso eliminado correctamente de la familia.";
            }
            catch (Exception ex)
            {
                return "Error al eliminar el permiso de la familia: " + ex.Message;
            }
        }
        public List<Familia_013AL> TraerListaFamilias_013AL()
        {
            List<Familia_013AL> lista = new List<Familia_013AL>();
            DataTable tabla = dal.TraerListaFamilias_013AL();

            Familia_013AL permiso = null;
            foreach (DataRow row in tabla.Rows)
            {
                if (Convert.ToBoolean(row[2]) == true)
                {
                    permiso = new Familia_013AL() { Cod_013AL = Convert.ToInt32(row[0]), Nombre_013AL = row[1].ToString(), Tipo_013AL = "Familia" };
                    lista.Add(permiso);
                }
            }
            return lista;
        }
        public int CrearFamilia_013AL(string NombreFamilia)
        {
            return dal.CrearFamilia_013AL(NombreFamilia);
        }
        public string EliminarFamilia_013AL(int id)
        {
            if (FamiliaTieneRelaciones_013AL(id))
            {
                return "No se puede eliminar la familia porque está relacionada con permisos u otras familias.";
            }
            return dal.EliminarFamilia_013AL(id);
        }
        public bool FamiliaTieneRelaciones_013AL(int idFamilia)
        {
            return dal.FamiliaTieneRelaciones_013AL(idFamilia);
        }
        public void ModificarFamilia_013AL(Familia_013AL familiaAModificar)
        {
            dal.ModificarFamilia_013AL(familiaAModificar);
        }
        public bool ExisteFamilia_013AL(string nombre)
        {
            List<Familia_013AL> listaFamilias = TraerListaFamilias_013AL();
            return listaFamilias.Any(f => f.Nombre_013AL.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}

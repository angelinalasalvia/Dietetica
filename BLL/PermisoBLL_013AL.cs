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

        /*public List<Permiso_013AL> ListarPermisos_013AL()
        {
            return dal.ListarPermisos_013AL();
        }*/
        //esteeee
        public string ObtenerNombrePermiso_013AL(int idPermiso)
        {
            return dal.ObtenerNombrePermiso_013AL(idPermiso);
        }

        /*public string ObtenerRolPorNombreUsuario(string nombreUsuario)
        {
            return dal.ObtenerRolPorNombreUsuario(nombreUsuario);
        }

        public List<PermisoCompuesto> ObtenerPermisosPorRol(int rol)
        {
            return dal.ObtenerPermisosPorRol(rol);
        }*/

        /*
        */

        /*public void CargarRolUsuario(Usuarios usuario)
        {
            usuario.oRol = dal.RolDeUsuario(usuario.ID);
        }*/
        /*public List<Familia_013AL> ListarFamilias_013AL()
        {
            return dal.ListarFamilias_013AL();
        }

        
        */
        /*public string AgregarPatenteHijo(string nombremenu )
        {
            
            Componente patente = new Componente();
            patente.Nombre = nombremenu;
           
            return dal.AgregarPatenteHijo(patente);
        }*/
        /*public string AgregarFamiliaPadre_013AL(string nombremenu)
        {

            Familia_013AL patente = new Familia_013AL();
            patente.Nombre_013AL = nombremenu;
            
            return dal.AgregarFamiliaPadre_013AL(patente);
        }*/
        /*public List<Permiso> ListarHijos(int idFamilia)
        {
            return dal.ListarHijos(idFamilia);
        }
        */
        /*public string AgregarRol_013AL(string nombre)
        {
            Rol_013AL rol = new Rol_013AL();
            rol.Nombre_013AL = nombre;
            return dal.AgregarRol_013AL(rol);
        }
        
        
        public string ModificarPermisos_013AL(int id, string nombre)
        {
            Familia_013AL permiso = new Familia_013AL();
            permiso.Cod_013AL = id;
            permiso.Nombre_013AL = nombre;
            return dal.ModificarPermisos_013AL(permiso);
        }*/
        /*public string ModificarPatentes(int id, string nombre)
        {
            Componente permiso = new Componente();
            permiso.Id = id;
            permiso.Nombre = nombre;
            return dal.ModificarPatentes(permiso);
        }*/

        /*public string InsertarFamiliaPatente_013AL(Familia_013AL obj, Permiso_013AL per)
        {
            try
            {
                // Llama al método de la DAL y retorna el resultado
                return dal.InsertarFamiliaPatente_013AL(obj, per);
            }
            catch (Exception ex)
            {
                // Maneja la excepción o la pasa a la capa superior
                throw new Exception("Error en la capa BLL", ex);
            }
        }*/



        /*
        public string Eliminarpermisosfamilia_013AL(int padre, int hijo)
        {
            return dal.Eliminarpermisosfamilia_013AL(padre, hijo);
        }
        
        public bool VerificarRolEnRolPermisos_013AL(int idRol)
        {
            return dal.VerificarRolEnRolPermisos_013AL(idRol);
        }
        public bool VerificarPermisosEnRolPermisos_013AL(int idper)
        {
            return dal.VerificarPermisosEnRolPermisos_013AL(idper);
        }
        public bool VerificarRolEnUsuario_013AL(int idRol)
        {
            return dal.VerificarRolEnUsuario_013AL(idRol);
        }
        public bool VerificarFamiliaEnPermisosComponente_013AL(int idRol)
        {
            return dal.VerificarFamiliaEnPermisosComponente_013AL(idRol);
        }*/
        /*public bool VerificarPatenteEnPermisosFamilia(int idRol)
        {
            return dal.VerificarPatenteEnPermisosFamilia(idRol);
        }*/
        /*public bool VerificarPermisosFamilia_013AL(int idfam, int idpat)
        {
            return dal.VerificarPermisosFamilia_013AL(idfam, idpat);
        }
        public bool VerificarPermisosRol_013AL(int idrol, int idfam)
        {
            return dal.VerificarPermisosRol_013AL(idrol, idfam);
        }
        public int BuscarId_013AL(string nombre)
        {
            try
            {
                // Llama al método de la capa DAL
                return dal.BuscarId_013AL(nombre);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones a nivel BLL si es necesario
                throw new Exception("Error en la capa BLL", ex);
            }
        }
        public int BuscarIdRol_013AL(string nombre)
        {
            try
            {
                // Llama al método de la capa DAL
                return dal.BuscarIdRol_013AL(nombre);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones a nivel BLL si es necesario
                throw new Exception("Error en la capa BLL", ex);
            }
        }*/

        //COMPOSITE NUEVO



        /**/

        //este
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

            /*List<Componente> lista = new List<Componente>();
            DataTable tabla = dal.TraerListaPermisos();

            Componente permiso = null;
            foreach (DataRow row in tabla.Rows)
            {
                if (Convert.ToBoolean(row[2]) == false)
                {
                    permiso = new Permiso() { Id = Convert.ToInt32(row[0]), Nombre = row[1].ToString(), Tipo = "Simple" };
                    lista.Add(permiso);
                }
            }
            return lista;*/
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



        /* public void RegistrarHijos_013AL(int idPadre, int idHijo)
         {
             dal.RegistrarHijos_013AL(idPadre, idHijo);
         }



         public void EliminarHijos_013AL(int idPadre)
         {
             dal.EliminarHijos_013AL(idPadre);
         }

         public Componente_013AL VerificarSiEstaEnFamilia_013AL(int idHijo)
         {
             return dal.VerificarSiEstaEnFamilia_013AL(idHijo);
         }




         public void EliminarPermisosRol_013AL(int idRol)
         {
             dal.EliminarPermisosRol_013AL(idRol);
         }






         public List<Componente_013AL> TraerListaPermisosRolSegunPermiso_013AL(int idPermiso)
         {
             return dal.TraerListaPermisosRolSegunPermiso_013AL(idPermiso);
         }



         


         



         */




    }
}

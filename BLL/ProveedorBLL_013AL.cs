using BE_013AL;
using DAL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLL
{
    public class ProveedorBLL_013AL
    {
        public DALProveedor_013AL dal = new DALProveedor_013AL();


        public List<Proveedor_013AL> ListarProveedores_013AL()
        {
            return dal.ListarProveedores_013AL();
        }

        public bool ExisteCuit_013AL(int cuit)
        {
            return dal.VerificarCuit_013AL(cuit);
        }

        public string PreregistrarProveedor_013AL(string nombre, int cuit, string razonsocial)
        {
            Proveedor_013AL obj = new Proveedor_013AL();
            obj.NombreProveedor_013AL = nombre;
            obj.CUIT_013AL = cuit;
            obj.RazonSocial_013AL = razonsocial;
            /*if (ExisteCuit(cuit))
            {
                return "El CUIT ingresado ya está registrado.";
            }*/
            return dal.PreregistrarProveedor_013AL(obj);
        }
        public string RegistrarProveedor_013AL(int cuit, string apellido, string dom, string mail, int tel)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            p.ApellidoProveedor_013AL = apellido;
            p.Domicilio_013AL = dom;
            p.Mail_013AL = mail;
            p.Telefono_013AL = tel;
            return dal.RegistrarProveedor_013AL(p);
        }
        public DataTable ListarProveedoresDGV_013AL()
        {
            return dal.ListarProveedoresDGV_013AL();
        }
        public string ModificarProveedor_013AL(int cuit, string nombre, string apellido, string dom, string mail, string rs, int tel)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            p.NombreProveedor_013AL = nombre;
            p.ApellidoProveedor_013AL = apellido;
            p.Domicilio_013AL = dom;
            p.Mail_013AL = mail;
            p.RazonSocial_013AL = rs;
            p.Telefono_013AL = tel;
            return dal.ModificarProveedor_013AL(p);
        }
        public string EliminarProveedor_013AL(int cuit)
        {
            Proveedor_013AL p = new Proveedor_013AL();
            p.CUIT_013AL = cuit;
            return dal.EliminarProveedor_013AL(p);
        }
    }
}

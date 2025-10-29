using BE_013AL;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLL
{
    public class ClienteBLL_013AL
    {
        private DALCliente_013AL dal = new DALCliente_013AL();
        public void GuardarClientes_013AL(string NombreTabla, DataSet Dset)
        {
            dal.GuardarClientes_013AL(NombreTabla, Dset);

        }
        public string AgregarCliente_013AL(string nombre, string apellido, int cuil, string domicilio, string mail, int tel)
        {
            Cliente_013AL cliente = new Cliente_013AL();
            cliente.Nombre_013AL = nombre;
            cliente.Apellido_013AL = apellido;
            cliente.CUIL_013AL = cuil;
            cliente.Domicilio_013AL = domicilio;
            cliente.Mail_013AL = mail;
            cliente.Telefono_013AL = tel;
            return dal.AgregarCliente_013AL(cliente);
        }

        public List<Cliente_013AL> BuscarCliente_013AL()
        {
            return dal.BuscarCliente_013AL();

        }
        public Cliente_013AL BuscarClientePorCUIL_013AL(int cuil)
        {
            return dal.BuscarClientePorCUIL_013AL(cuil);
        }

        public DataSet ObtenerClientes_013AL()
        {
            return dal.ObtenerClientes_013AL();
        }
    }
}

using BE_013AL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicios;

namespace BLL_013AL
{
    public class BLLBitacora_013AL
    {
        public DALConexiones_013AL dal = new DALConexiones_013AL();

        public string AgregarEvento_013AL(string login, string modulo, string nomevento, int criticidad)
        {
            Eventos evento = new Eventos();
            evento.Login = login;
            evento.Modulo = modulo;
            evento.Evento = nomevento;
            evento.Criticidad = criticidad;
            return dal.AgregarEvento_013AL(evento);
        }

        public DataTable ListarEventos_013AL()
        {
            return dal.ListarEventos_013AL();
        }
        public DataTable ConsultasEventos_013AL(string login, DateTime? fechaInicio, DateTime? fechaFin, string modulo, string evento, int? criticidad)
        {
            return dal.ConsultasEventos_013AL(login, fechaInicio, fechaFin, modulo, evento, criticidad);
        }
        public DataTable ObtenerNombreApellidoPorLogin_013AL(string login)
        {
            return dal.ObtenerNombreApellidoPorLogin_013AL(login);
        }
        public DataTable ListarProductosC_013AL()
        {
            return dal.ListarProductosC_013AL();
        }
        public void ActivarProductoC_013AL(int idProductoC)
        {
            dal.ActivarProductoC_013AL(idProductoC);
        }
        public DataTable ConsultaProductosC_013AL(int? id, string nombre, DateTime? fechaInicio, DateTime? fechaFin)
        {
            return dal.ConsultaProductosC_013AL(id, nombre, fechaInicio, fechaFin);
        }
        /*public DataTable ConsultasEventos(string login, DateTime fecha, string modulo, string evento, int criticidad)
        {
            return dal.ConsultasEventos(login, fecha, modulo, evento, criticidad);
        }*/

    }
}

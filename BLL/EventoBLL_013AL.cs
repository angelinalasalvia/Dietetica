using BE_013AL;
using DAL_013AL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicios;
using DAL;

namespace BLL_013AL
{
    public class EventoBLL_013AL
    {
        public DALEvento_013AL dal = new DALEvento_013AL();

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
        
        

    }
}

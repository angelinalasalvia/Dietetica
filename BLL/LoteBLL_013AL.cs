using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LoteBLL_013AL
    {
        DALLote_013AL dal = new DALLote_013AL();

        public DataTable ListarLotes_013AL()
        {
            return dal.ListaLotes_013AL();
        }

        public string ModificarLote_013AL(int idLote, DateTime fechaVencimiento, string estado)
        {
            Lote_013AL lote = new Lote_013AL();

            lote.CodLote_013AL = idLote;
            lote.FechaVencimiento_013AL = fechaVencimiento;
            lote.Estado_013AL = estado;

            return dal.ModificarLote_013AL(lote);
        }
        public void AgregarLote_013AL(Lote_013AL lote)
        {
            dal.AgregarLote_013AL(lote);
        }
        public void ActualizarEstadosLotes_013AL()
        {
            dal.ActualizarEstadosLotes_013AL();
        }

        public int ContarLotesProximosAVencer_013AL()
        {
            return dal.ContarLotesProximosAVencer_013AL();
        }
    }

}

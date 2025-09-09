using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE.Composite;

namespace DAL
{
    public class DALPermiso
    {
        DALConexiones dalCon = new DALConexiones();

        public DataTable TraerListaPermisos()
        {
            DataTable tabla = dalCon.TraerTabla("Permisos");
            return tabla;
        }

        
    
}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL_013AL;

namespace BLL_013AL
{
    public class BackupBLL_013AL
    {
        public DALBackUp_013AL dalbackup = new DALBackUp_013AL();

        public void RealizarBackup_013AL(string backupPath)
        {
            dalbackup.RealizarBackup_013AL(backupPath);
        }
        public void RealizarRestore_013AL(string restorePath)
        {
            dalbackup.RealizarRestore_013AL(restorePath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class BackUpRestore_013AL
    {
        public bool VerificarRutaBackup_013AL(string ruta)
        {
            return Directory.Exists(Path.GetDirectoryName(ruta));
        }

        public bool VerificarArchivoRestore_013AL(string rutaArchivo)
        {
            return File.Exists(rutaArchivo) && Path.GetExtension(rutaArchivo).Equals(".bak", StringComparison.OrdinalIgnoreCase);
        }
    }
}

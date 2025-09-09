using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using DAL_013AL; 
using BE_013AL;

namespace BLL_013AL
{
    public class BLLDigitoVerificador_013AL
    {
     /*   DALConexiones dal = new DALConexiones();
        public static int CalcularDVH(string data)
        {
            int dvh = 0;
            foreach (char c in data)
            {
                dvh += (int)c; // Suma los valores ASCII de cada carácter
            }
            return dvh;
        }

        // Método para calcular el DVV de una tabla
        public  int CalcularDVV(List<int> listaDVH)
        {
            return listaDVH.Sum(); // La suma de todos los DVH de la tabla
        }

        // Método para recalcular DVH y actualizar en la BD
        public  void RecalcularDVH(string tabla)
        {
            List<DigitoVerificador> registros = dal.ObtenerRegistros(tabla);
            foreach (var registro in registros)
            {
                string concatenacion = $"{registro.Id}{registro.Campo1}{registro.Campo2}"; // Concatenar campos importantes
                int nuevoDVH = CalcularDVH(concatenacion);
                dal.ActualizarDVH(registro.Id, tabla, nuevoDVH);
            }
        }

        // Método para verificar la integridad de los datos
        public  bool VerificarIntegridad(string tabla)
        {
            List<DigitoVerificador> registros = dal.ObtenerRegistros(tabla);
            List<int> listaDVH = new List<int>();

            foreach (var registro in registros)
            {
                string concatenacion = $"{registro.Id}{registro.Campo1}{registro.Campo2}";
                int dvhCalculado = CalcularDVH(concatenacion);
                if (dvhCalculado != registro.DVH) return false; // Inconsistencia detectada
                listaDVH.Add(dvhCalculado);
            }

            int dvvCalculado = CalcularDVV(listaDVH);
            int dvvGuardado = dal.ObtenerDVV(tabla);
            return dvvCalculado == dvvGuardado; // Verifica DVV
        }*/
    }
}

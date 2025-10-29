using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BE_013AL.Composite
{
    public class Familia_013AL : Rol_013AL
    {
        
        private List<Rol_013AL> listaHijos = new List<Rol_013AL>();

        public override void AgregarHijo_013AL(Rol_013AL comp)
        {
            
            if (!listaHijos.Any(p => p.Cod_013AL == comp.Cod_013AL))
            {
                if (comp is Familia_013AL familia)
                {
                    if (EsCiclo_013AL(familia))
                    {
                        throw new InvalidOperationException("No se puede agregar esta familia porque generaría un ciclo.");
                    }
                }

                listaHijos.Add(comp);
            }
        }

        public override List<Rol_013AL> ObtenerHijos_013AL()
        {
            return listaHijos;
        }

        public override void QuitarHijo_013AL(Rol_013AL comp)
        {
            listaHijos.RemoveAll(p => p.Cod_013AL == comp.Cod_013AL);
        }

        private bool EsCiclo_013AL(Familia_013AL familia)
        {
            if (familia.Cod_013AL == this.Cod_013AL) return true; 

            foreach (var hijo in listaHijos)
            {
                if (hijo is Familia_013AL f && f.EsCiclo_013AL(familia))
                {
                    return true;
                }
            }
            return false;
        }


    }

    
}

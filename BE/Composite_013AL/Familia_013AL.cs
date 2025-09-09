using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BE_013AL.Composite
{
    public class Familia_013AL : Componente_013AL
    {
        /*public override int Id { get; set; }
        public override string Nombre { get; set; }
        public override string Tipo { get; set; }
     
        public List<Permiso> Permisos { get; set; }

        public Familia()
        {
            Permisos = new List<Permiso>();
        }*/
        private List<Componente_013AL> listaHijos = new List<Componente_013AL>();

        public override void AgregarHijo_013AL(Componente_013AL comp)
        {
            // Evitar agregar un duplicado
            if (!listaHijos.Any(p => p.Cod_013AL == comp.Cod_013AL))
            {
                // Validar que no se genere un ciclo
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

        public override List<Componente_013AL> ObtenerHijos_013AL()
        {
            return listaHijos;
        }

        public override void QuitarHijo_013AL(Componente_013AL comp)
        {
            listaHijos.RemoveAll(p => p.Cod_013AL == comp.Cod_013AL);
        }

        private bool EsCiclo_013AL(Familia_013AL familia)
        {
            if (familia.Cod_013AL == this.Cod_013AL) return true; // No se puede agregar a sí misma

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

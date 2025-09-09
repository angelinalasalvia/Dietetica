using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_013AL
{
    public interface ISubject_013AL
    {
        void Agregar_013AL(IObserver_013AL observer);
        void Quitar_013AL(IObserver_013AL observer);
        void Notificar_013AL();
    }
}

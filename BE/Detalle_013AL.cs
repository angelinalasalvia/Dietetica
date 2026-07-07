using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_013AL
{
    public class Detalle_013AL
    {
        public int CodCompra_013AL { get; set; }
        public int CodProducto_013AL { get; set; }
        public int Cantidad_013AL { get; set; }
        public int PrecioUnitario_013AL { get; set; }
        public string PromocionAplicada_013AL { get; set; }
        public decimal DescuentoAplicado_013AL { get; set; }

        public decimal Subtotal_013AL => Cantidad_013AL * PrecioUnitario_013AL;
        public decimal SubtotalConDescuento_013AL => Subtotal_013AL - DescuentoAplicado_013AL;
    }
}

using BE_013AL;
using BLL;

namespace PruebaUnitaria;

[TestClass]
public class Prueba
{
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void RegistrarVentaCompleta_CompraDuplicada_LanzaExcepcion()
    {
        FacturaBLL_013AL facturaBLL = new FacturaBLL_013AL();

        Factura_013AL factura = new Factura_013AL
        {
            CodCompra_013AL = 1, 
            CUIL_013AL = 23924,
            Total_013AL = 1210,
            MetPago_013AL = "Efectivo"
        };

        List<Detalle_013AL> detalles = new List<Detalle_013AL>
    {
        new Detalle_013AL
        {
            CodCompra_013AL = 1,
            CodProducto_013AL = 10,
            Cantidad_013AL = 2,
            PrecioUnitario_013AL = 500
        }
    };

        facturaBLL.RegistrarVentaCompleta_013AL(factura, detalles);
    }
}

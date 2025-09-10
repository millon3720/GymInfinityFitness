namespace Tesina.Models
{
    public class ProductoInventarioViewModel
    {
        public ProductosServicios Producto { get; set; } = new();
        public Inventario Inventario { get; set; } = new();
    }
}

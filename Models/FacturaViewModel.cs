using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tesina.Models
{
    public class FacturaViewModel
    {
        public Facturas Factura { get; set; } = new();
        public List<DetalleFactura> Detalles { get; set; } = new();
        public List<SelectListItem> ProductosDisponibles { get; set; } = new();
        public List<SelectListItem> UsuariosDisponibles { get; set; } = new();
    }
}

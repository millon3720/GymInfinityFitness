using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;
using Tesina.Models;

namespace Tesina.Controllers
{
    public class FacturasController : Controller
    {
        private readonly GymDbContext _context;
        private readonly GenerarFacturaPDF _pdf;

        public FacturasController(GymDbContext context, GenerarFacturaPDF pdf)
        {
            _context = context; 
            _pdf = pdf;

        }

       
        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Facturas.Include(f => f.Usuario);
            return View(await gymDbContext.ToListAsync());
        }
        public async Task<IActionResult> FacturasCliente(int? id)
        {
            var gymDbContext = _context.Facturas.Where(f => f.IdUsuario==id).OrderByDescending(e => e.Fecha);
            return View(await gymDbContext.ToListAsync());
        }
        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var factura = await _context.Facturas
                .Include(f => f.Usuario)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.ProductoServicio)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null)
                return NotFound();

            return View(factura);
        }
        // GET: Facturas/Create
        public async Task<IActionResult> Create()
        {
            var productos = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .ToListAsync();

            var viewModel = new FacturaViewModel
            {
                Factura = new Facturas { Fecha = DateTime.Now },
                Detalles = new List<DetalleFactura> { new DetalleFactura() },
                UsuariosDisponibles = await _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.IdUsuario.ToString(),
                        Text = u.NombreCompleto.ToString()
                    }).ToListAsync(),
                ProductosDisponibles = productos
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdProductoServicio.ToString(),
                        Text = p.Nombre
                    }).ToList()
            };

            ViewBag.ProductosExtendidos = productos.Select(p => new
            {
                id = p.IdProductoServicio,
                nombre = p.Nombre,
                precio = p.Precio,
                tipo = p.Tipo,
                stock = p.Inventario?.CantidadDisponible ?? 0
            }).ToList();

            return View(viewModel);
        }
        // POST: Facturas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacturaViewModel model)
        {
            if (!ModelState.IsValid || model.Detalles == null || !model.Detalles.Any())
            {
                await RecargarListas(model);
                ModelState.AddModelError("", "Debe agregar al menos un producto o servicio.");
                return View(model);
            }

            var idsDuplicados = model.Detalles
                .GroupBy(d => d.IdProductoServicio)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (idsDuplicados.Any())
            {
                await RecargarListas(model);
                ModelState.AddModelError("", "Hay productos o servicios duplicados en la factura.");
                return View(model);
            }

            foreach (var detalle in model.Detalles)
            {
                var producto = await _context.ProductosServicios
                    .Include(p => p.Inventario)
                    .FirstOrDefaultAsync(p => p.IdProductoServicio == detalle.IdProductoServicio);

                if (producto == null)
                {
                    ModelState.AddModelError("", $"Producto o servicio con ID {detalle.IdProductoServicio} no existe.");
                    continue;
                }

                if (producto.Tipo.ToLower() == "producto" &&
                    producto.Inventario != null &&
                    detalle.Cantidad > producto.Inventario.CantidadDisponible)
                {
                    await RecargarListas(model);
                    ModelState.AddModelError("", $"La cantidad de '{producto.Nombre}' supera el stock disponible ({producto.Inventario.CantidadDisponible}).");
                    return View(model);
                }

                detalle.Subtotal = producto.Precio * detalle.Cantidad;
            }

            model.Factura.Total = model.Detalles.Sum(d => d.Subtotal);

            _context.Facturas.Add(model.Factura);
            await _context.SaveChangesAsync();

            foreach (var detalle in model.Detalles)
            {
                detalle.IdFactura = model.Factura.IdFactura;
                _context.DetalleFactura.Add(detalle);

                var producto = await _context.ProductosServicios
                    .Include(p => p.Inventario)
                    .FirstOrDefaultAsync(p => p.IdProductoServicio == detalle.IdProductoServicio);

                if (producto != null && producto.Tipo.ToLower() == "producto" && producto.Inventario != null)
                {
                    producto.Inventario.CantidadDisponible -= detalle.Cantidad;
                    producto.Inventario.CantidadDisponible = Math.Max(producto.Inventario.CantidadDisponible, 0);
                    _context.Inventario.Update(producto.Inventario);
                }
            }

            await _context.SaveChangesAsync();

            var facturaCompleta = await _context.Facturas
                .Include(f => f.Usuario)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.ProductoServicio)
                .FirstOrDefaultAsync(f => f.IdFactura == model.Factura.IdFactura);

            var viewModelFinal = new FacturaViewModel
            {
                Factura = facturaCompleta,
                Detalles = facturaCompleta.DetallesFactura.ToList()
            };

            var fecha = facturaCompleta.Fecha.ToString("dd-MM-yyyy");
            var nombreArchivo = $"Factura_{fecha}.pdf";

            var pdfBytes = _pdf.GenerarFactura(viewModelFinal);
            await _pdf.EnviarFacturaPorCorreoAsync(facturaCompleta.Usuario.Correo, pdfBytes, nombreArchivo);

            TempData["Alerta"] = "✅ Información guardada con éxito.";
            return RedirectToAction(nameof(Index));
        }
        private async Task RecargarListas(FacturaViewModel model)
        {
            var productos = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .ToListAsync();

            model.UsuariosDisponibles = await _context.Usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.IdUsuario.ToString(),
                    Text = u.NombreCompleto.ToString()
                }).ToListAsync();

            model.ProductosDisponibles = productos
                .Select(p => new SelectListItem
                {
                    Value = p.IdProductoServicio.ToString(),
                    Text = p.Nombre
                }).ToList();

            ViewBag.ProductosExtendidos = productos.Select(p => new
            {
                id = p.IdProductoServicio,
                nombre = p.Nombre,
                precio = p.Precio,
                tipo = p.Tipo,
                stock = p.Inventario?.CantidadDisponible ?? 0
            }).ToList();
        }
        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Usuario)
                .Include(f => f.DetallesFactura)
                .ThenInclude(d => d.ProductoServicio)
                .ThenInclude(p => p.Inventario)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null)
                return NotFound();

            var productos = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .ToListAsync();

            var viewModel = new FacturaViewModel
            {
                Factura = factura,
                Detalles = factura.DetallesFactura.ToList(),
                UsuariosDisponibles = await _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.IdUsuario.ToString(),
                        Text = u.NombreCompleto
                    }).ToListAsync(),
                ProductosDisponibles = productos
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdProductoServicio.ToString(),
                        Text = p.Nombre
                    }).ToList()
            };

            ViewBag.ProductosExtendidos = productos.Select(p => new
            {
                id = p.IdProductoServicio,
                nombre = p.Nombre,
                precio = p.Precio,
                tipo = p.Tipo,
                stock = p.Inventario?.CantidadDisponible ?? 0
            }).ToList();

            return View(viewModel);
        }
        // POST: Facturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FacturaViewModel model)
        {
            if (!ModelState.IsValid || model.Detalles == null || !model.Detalles.Any())
            {
                await RecargarListas(model);
                ModelState.AddModelError("", "Debe agregar al menos un producto o servicio.");
                return View(model);
            }

            var idsDuplicados = model.Detalles
                .GroupBy(d => d.IdProductoServicio)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (idsDuplicados.Any())
            {
                await RecargarListas(model);
                ModelState.AddModelError("", "Hay productos o servicios duplicados en la factura.");
                return View(model);
            }

            foreach (var detalle in model.Detalles)
            {
                var producto = await _context.ProductosServicios
                    .Include(p => p.Inventario)
                    .FirstOrDefaultAsync(p => p.IdProductoServicio == detalle.IdProductoServicio);

                if (producto == null)
                {
                    ModelState.AddModelError("", $"Producto o servicio con ID {detalle.IdProductoServicio} no existe.");
                    continue;
                }

                if (producto.Tipo.ToLower() == "producto" &&
                    producto.Inventario != null &&
                    detalle.Cantidad > producto.Inventario.CantidadDisponible)
                {
                    await RecargarListas(model);
                    ModelState.AddModelError("", $"La cantidad de '{producto.Nombre}' supera el stock disponible ({producto.Inventario.CantidadDisponible}).");
                    return View(model);
                }

                detalle.Subtotal = producto.Precio * detalle.Cantidad;
            }

            model.Factura.Total = model.Detalles.Sum(d => d.Subtotal);

            var facturaExistente = await _context.Facturas
                .Include(f => f.DetallesFactura)
                .FirstOrDefaultAsync(f => f.IdFactura == model.Factura.IdFactura);

            if (facturaExistente == null)
                return NotFound();

            // Actualizar datos básicos
            facturaExistente.IdUsuario = model.Factura.IdUsuario;
            facturaExistente.Fecha = model.Factura.Fecha;
            facturaExistente.Total = model.Factura.Total;

            // Eliminar detalles anteriores
            _context.DetalleFactura.RemoveRange(facturaExistente.DetallesFactura);

            // Agregar nuevos detalles y actualizar inventario
            foreach (var detalle in model.Detalles)
            {
                detalle.IdFactura = facturaExistente.IdFactura;
                _context.DetalleFactura.Add(detalle);

                var producto = await _context.ProductosServicios
                    .Include(p => p.Inventario)
                    .FirstOrDefaultAsync(p => p.IdProductoServicio == detalle.IdProductoServicio);

                if (producto != null && producto.Tipo.ToLower() == "producto" && producto.Inventario != null)
                {
                    producto.Inventario.CantidadDisponible -= detalle.Cantidad;
                    if (producto.Inventario.CantidadDisponible < 0)
                        producto.Inventario.CantidadDisponible = 0;

                    _context.Inventario.Update(producto.Inventario);
                }
            }

            await _context.SaveChangesAsync();
            var facturaCompleta = await _context.Facturas
                .Include(f => f.Usuario)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.ProductoServicio)
                .FirstOrDefaultAsync(f => f.IdFactura == model.Factura.IdFactura);

            var viewModelFinal = new FacturaViewModel
            {
                Factura = facturaCompleta,
                Detalles = facturaCompleta.DetallesFactura.ToList()
            };

            var fecha = facturaCompleta.Fecha.ToString("dd-MM-yyyy");
            var nombreArchivo = $"Factura_{fecha}.pdf";

            var pdfBytes = _pdf.GenerarFactura(viewModelFinal);
            await _pdf.EnviarFacturaPorCorreoAsync(facturaCompleta.Usuario.Correo, pdfBytes, nombreArchivo);


            TempData["Alerta"] = "✅ Información actualizada con éxito.";

            return RedirectToAction(nameof(Index));
        }
        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facturas = await _context.Facturas
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (facturas == null)
            {
                return NotFound();
            }

            return View(facturas);
        }
        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facturas = await _context.Facturas.FindAsync(id);
            if (facturas != null)
            {
                _context.Facturas.Remove(facturas);
            }

            await _context.SaveChangesAsync(); 
            TempData["Alerta"] = "✅ La Factura se elimino del sistema.";
            return RedirectToAction(nameof(Index));
        }
        private bool FacturasExists(int id)
        {
            return _context.Facturas.Any(e => e.IdFactura == id);
        }
    }
}

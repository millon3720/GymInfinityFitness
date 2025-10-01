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
    public class InventariosController : Controller
    {
        private readonly GymDbContext _context;

        public InventariosController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Inventarios
        public async Task<IActionResult> Index()
        {
            var productos = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .ToListAsync();

            var viewModel = productos
                .Where(p => p.Inventario != null)
                .Select(p => new ProductoInventarioViewModel
                {
                    Producto = p,
                    Inventario = p.Inventario
                }).ToList();

            return View(viewModel);
        }


        // GET: Inventarios/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .FirstOrDefaultAsync(p => p.IdProductoServicio == id);

            if (producto == null || producto.Inventario == null)
                return NotFound();

            var viewModel = new ProductoInventarioViewModel
            {
                Producto = producto,
                Inventario = producto.Inventario
            };

            return View(viewModel);
        }

        // GET: Inventarios/Create
        public IActionResult Create()
        {
            var viewModel = new ProductoInventarioViewModel
            {
                Producto = new ProductosServicios(),
                Inventario = new Inventario()
            };

            return View(viewModel);
        }

        // POST: Inventarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoInventarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.ProductosServicios.Add(model.Producto);
                await _context.SaveChangesAsync();

                model.Inventario.IdProductoServicio = model.Producto.IdProductoServicio;
                _context.Inventario.Add(model.Inventario);
                await _context.SaveChangesAsync();
                TempData["Alerta"] = "✅ Información guardada con éxito.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: Inventarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .FirstOrDefaultAsync(p => p.IdProductoServicio == id);

            if (producto == null || producto.Inventario == null)
                return NotFound();

            var viewModel = new ProductoInventarioViewModel
            {
                Producto = producto,
                Inventario = producto.Inventario
            };

            return View(viewModel);
        }

        // POST: Inventarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductoInventarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model.Producto);
                model.Inventario.IdProductoServicio = model.Producto.IdProductoServicio;
                _context.Update(model.Inventario);
                await _context.SaveChangesAsync();
                TempData["Alerta"] = "✅ Información actualizada con éxito.";

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Inventarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .FirstOrDefaultAsync(p => p.IdProductoServicio == id);

            if (producto == null || producto.Inventario == null)
                return NotFound();

            var viewModel = new ProductoInventarioViewModel
            {
                Producto = producto,
                Inventario = producto.Inventario
            };

            return View(viewModel);
        }

        // POST: Inventarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.ProductosServicios
                .Include(p => p.Inventario)
                .FirstOrDefaultAsync(p => p.IdProductoServicio == id);

            if (producto != null)
            {
                if (producto.Inventario != null)
                    _context.Inventario.Remove(producto.Inventario);

                _context.ProductosServicios.Remove(producto);
                await _context.SaveChangesAsync();
            }
            TempData["Alerta"] = "✅ El Producto se elimino del sistema.";
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
            return _context.Inventario.Any(e => e.IdInventario == id);
        }
    }
}

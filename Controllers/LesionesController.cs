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
    public class LesionesController : Controller
    {
        private readonly GymDbContext _context;

        public LesionesController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Lesiones
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Lesiones.Include(l => l.Usuario).OrderBy(a => a.FechaDiagnostico);
            return View(await gymDbContext.ToListAsync());
        }

        // GET: Lesiones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesion = await _context.Lesiones
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(m => m.IdLesion == id);
            if (lesion == null)
            {
                return NotFound();
            }

            return View(lesion);
        }

        // GET: Lesiones/Create
        // GET: Lesiones/Create
        public IActionResult Create(int? id)
        {
            if (id == null) return NotFound();

            var cliente = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
            if (cliente == null) return NotFound();

            var viewModel = new ClienteDetalle
            {
                Cliente = cliente,
                Lesion = new List<Lesion>() // inicializamos la lista para agregar dinámicamente
            };

            return View(viewModel);
        }

        // POST: Lesiones/CrearLesiones
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearLesiones(ClienteDetalle model)
        {
            if (model.Lesion != null && model.Lesion.Any())
            {
                foreach (var l in model.Lesion)
                {
                    // l.IdUsuario ya viene del hidden input
                    _context.Lesiones.Add(l);
                }
                await _context.SaveChangesAsync();
                TempData["Alerta"] = "✅ Información guardada con éxito.";
                // Usa el IdUsuario de la primera lesión para redirigir
                return RedirectToAction("Details", "Usuarios", new { id = model.Lesion.First().IdUsuario });
            }

            ViewBag.Usuarios = new SelectList(_context.Usuarios.Where(u => u.Rol == "Cliente"), "IdUsuario", "NombreCompleto");
            return View(model);
        }



        // GET: Lesiones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesion = await _context.Lesiones.FindAsync(id);
            if (lesion == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", lesion.IdUsuario);
            return View(lesion);
        }

        // POST: Lesiones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLesion,IdUsuario,Descripcion,FechaDiagnostico")] Lesion lesion)
        {
            if (id != lesion.IdLesion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LesionExists(lesion.IdLesion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Alerta"] = "✅ Información actualizada con éxito.";

                return RedirectToAction("Details", "Usuarios", new { id = lesion.IdUsuario });
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", lesion.IdUsuario);
            return View(lesion);
        }

        // GET: Lesiones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesion = await _context.Lesiones
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(m => m.IdLesion == id);
            if (lesion == null)
            {
                return NotFound();
            }

            return View(lesion);
        }

        // POST: Lesiones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesion = await _context.Lesiones.FindAsync(id);
            if (lesion != null)
            {
                _context.Lesiones.Remove(lesion);
            }

            await _context.SaveChangesAsync(); 
            TempData["Alerta"] = "✅ La Lesion se elimino del sistema.";

            return RedirectToAction("Details", "Usuarios", new { id = lesion.IdUsuario });
        }

        private bool LesionExists(int id)
        {
            return _context.Lesiones.Any(e => e.IdLesion == id);
        }
    }
}

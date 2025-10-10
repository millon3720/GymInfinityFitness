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
        private bool LesionExists(int id)
        {
            return _context.Lesiones.Any(e => e.IdLesion == id);
        }

        #region Mantenimientos
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
                if (User.HasClaim("Rol", "Cliente"))
                {
                    return RedirectToAction("LesionesCliente", "Lesiones", new { id = model.Lesion.First().IdUsuario });
                }
                if (User.HasClaim("Rol", "Entrenador"))
                {
                    return RedirectToAction("Details", "Usuarios", new { id = model.Lesion.First().IdUsuario });
                }
            }
            ViewBag.Usuarios = new SelectList(_context.Usuarios.Where(u => u.Rol == "Cliente"), "IdUsuario", "NombreCompleto");

            if (User.Identity.IsAuthenticated)
            {
                return View(model);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
                if (User.HasClaim("Rol", "Cliente"))
                {
                    return RedirectToAction("LesionesCliente", "Lesiones", new { id = lesion.IdUsuario });
                }
                if (User.HasClaim("Rol", "Entrenador"))
                {
                    return RedirectToAction("Details", "Usuarios", new { id = lesion.IdUsuario });
                }
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", lesion.IdUsuario);
            return View(lesion);
        }
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
            if (User.HasClaim("Rol", "Cliente"))
            {
                return RedirectToAction("LesionesCliente", "Lesiones", new { id = lesion.IdUsuario });
            }
            if (User.HasClaim("Rol", "Entrenador"))
            {
                return RedirectToAction("Details", "Usuarios", new { id = lesion.IdUsuario });
            }
            return RedirectToAction("Details", "Usuarios", new { id = lesion.IdUsuario });
        }
        #endregion

        #region Views
        public async Task<IActionResult> LesionesCliente(int? Id)
        {
            var gymDbContext = _context.Lesiones.Where(l => l.IdUsuario == Id).OrderBy(a => a.FechaDiagnostico);
            if (User.Identity.IsAuthenticated)
            {
                return View(await gymDbContext.ToListAsync());

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
            if (User.Identity.IsAuthenticated)
            {
                return View(lesion);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
            if (User.Identity.IsAuthenticated)
            {
                return View(viewModel);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
            if (User.Identity.IsAuthenticated)
            {
                return View(lesion);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
            if (User.Identity.IsAuthenticated)
            {
                return View(lesion);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        #endregion            
    }
}
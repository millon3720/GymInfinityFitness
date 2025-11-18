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
    public class EjerciciosController : Controller
    {
        private readonly GymDbContext _context;
        public EjerciciosController(GymDbContext context)
        {
            _context = context;
        }
        private bool EjercicioExists(int id)
        {
            return _context.Ejercicios.Any(e => e.IdEjercicio == id);
        }

        #region Mantenimientos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEjercicio,Nombre,Descripcion,VideoURL")] Ejercicio ejercicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ejercicio);
                await _context.SaveChangesAsync();
                TempData["Alerta"] = "✅ Información guardada con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(ejercicio);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEjercicio,Nombre,Descripcion,VideoURL")] Ejercicio ejercicio)
        {
            if (id != ejercicio.IdEjercicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ejercicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EjercicioExists(ejercicio.IdEjercicio))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Alerta"] = "✅ Información actualizada con éxito.";

                return RedirectToAction(nameof(Index));
            }
            return View(ejercicio);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);
            if (ejercicio != null)
            {
                _context.Ejercicios.Remove(ejercicio);
            }

            await _context.SaveChangesAsync();
            TempData["Alerta"] = "✅ El Ejerccio se elimino del sistema.";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Views
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.Ejercicios.OrderBy(e => e.Nombre).ToListAsync());

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

            var ejercicio = await _context.Ejercicios
                .OrderBy(e => e.Nombre).FirstOrDefaultAsync(m => m.IdEjercicio == id);
            if (ejercicio == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(ejercicio);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(new Ejercicio());

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

            var ejercicio = await _context.Ejercicios.FindAsync(id);
            if (ejercicio == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(ejercicio);

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

            var ejercicio = await _context.Ejercicios
                .FirstOrDefaultAsync(m => m.IdEjercicio == id);
            if (ejercicio == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(ejercicio);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        #endregion
    }
}
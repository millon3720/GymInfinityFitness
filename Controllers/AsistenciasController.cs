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
    public class AsistenciasController : Controller
    {
        private readonly GymDbContext _context;
        public AsistenciasController(GymDbContext context)
        {
            _context = context;
        }
        private bool AsistenciasExists(int id)
        {
            return _context.Asistencias.Any(e => e.IdAsistencia == id);
        }

        #region Mantenimientos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAsistencia(string CodigoUsuario)
        {
            if (string.IsNullOrWhiteSpace(CodigoUsuario) || CodigoUsuario.Length != 4 || !CodigoUsuario.All(char.IsDigit))
            {
                ModelState.AddModelError("", "Debe ingresar un código válido de 4 dígitos.");
                return View(new AsistenciaViewModel());
            }

            int idUsuario = int.Parse(CodigoUsuario);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View(new AsistenciaViewModel());
            }

            var membresia = await _context.Mensualidades
                .Where(m => m.IdUsuario == usuario.IdUsuario)
                .OrderByDescending(m => m.FechaFin)
                .FirstOrDefaultAsync();

            bool activa = membresia != null && membresia.FechaFin >= DateTime.Now;

            var viewModel = new AsistenciaViewModel
            {
                Usuario = usuario,
                MembresiaActiva = activa
            };

            if (!activa)
                return View(viewModel);

            // Buscar la última asistencia sin salida
            var asistenciaPendiente = await _context.Asistencias
                .Where(a => a.IdUsuario == usuario.IdUsuario && a.FechaSalida == null)
                .OrderByDescending(a => a.FechaIngreso)
                .FirstOrDefaultAsync();

            if (asistenciaPendiente == null)
            {
                // No hay entrada abierta → registrar nueva entrada
                var nueva = new Asistencias
                {
                    IdUsuario = usuario.IdUsuario,
                    FechaIngreso = DateTime.Now
                };

                _context.Asistencias.Add(nueva);
            }
            else
            {
                // Hay entrada abierta → registrar salida
                asistenciaPendiente.FechaSalida = DateTime.Now;
                _context.Asistencias.Update(asistenciaPendiente);
            }

            await _context.SaveChangesAsync();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsistencia,IdUsuario,FechaIngreso,FechaSalida")] Asistencias asistencias)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asistencias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", asistencias.IdUsuario);
            if (User.Identity.IsAuthenticated)
            {
                return View(asistencias);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsistencia,IdUsuario,FechaIngreso,FechaSalida")] Asistencias asistencias)
        {
            if (id != asistencias.IdAsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencias);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciasExists(asistencias.IdAsistencia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", asistencias.IdUsuario);
            return View(asistencias);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencias = await _context.Asistencias.FindAsync(id);
            if (asistencias != null)
            {
                _context.Asistencias.Remove(asistencias);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Views
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Asistencias.Include(a => a.Usuario);
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

            var asistencias = await _context.Asistencias
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencias == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(asistencias);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult RegistrarAsistencia()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();

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

            var asistencias = await _context.Asistencias.FindAsync(id);
            if (asistencias == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", asistencias.IdUsuario);
            if (User.Identity.IsAuthenticated)
            {
                return View(asistencias);

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

            var asistencias = await _context.Asistencias
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencias == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(asistencias);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        #endregion       
    }
}
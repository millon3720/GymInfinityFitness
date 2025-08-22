using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesina.Data;
using Tesina.Models;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class RutinasController : Controller
    {
        private readonly GymDbContext _context;

        public RutinasController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Rutinas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rutinas.ToListAsync());
        }

        // GET: Rutinas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rutina = await _context.Rutinas
    .Include(r => r.RutinaEjercicio)
        .ThenInclude(re => re.Ejercicio)
    .FirstOrDefaultAsync(m => m.IdRutina == id);

            if (rutina == null)
            {
                return NotFound();
            }

            return View(rutina);
        }   
        public async Task<IActionResult> Create()
        {
            var ejercicios = await _context.Ejercicios.ToListAsync();
            ViewBag.Ejercicios = new SelectList(ejercicios, "IdEjercicio", "Nombre");

            var viewModel = new RutinaCreateViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RutinaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var ejercicios = await _context.Ejercicios.ToListAsync();
                ViewBag.Ejercicios = new SelectList(ejercicios, "IdEjercicio", "Nombre");
                foreach (var modelState in ModelState)
                {
                    var key = modelState.Key;
                    var errors = modelState.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            model.Rutina.FechaCreacion = DateTime.Now;
            _context.Rutinas.Add(model.Rutina);
            await _context.SaveChangesAsync();

            foreach (var e in model.Ejercicios)
            {
                var rutinaEj = new RutinaEjercicio
                {
                    IdRutina = model.Rutina.IdRutina,
                    IdEjercicio = e.IdEjercicio,
                    Series = e.Series,
                    Repeticiones = e.Repeticiones,
                    DescansoSegundos = e.DescansoSegundos,
                    DiaSemana = e.DiaSemana
                };
                _context.RutinaEjercicios.Add(rutinaEj);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: Rutinas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rutina = await _context.Rutinas
                .Include(r => r.RutinaEjercicio)
                .FirstOrDefaultAsync(r => r.IdRutina == id);

            if (rutina == null) return NotFound();

            var viewModel = new RutinaCreateViewModel
            {
                Rutina = rutina,
                Ejercicios = rutina.RutinaEjercicio.Select(e => new RutinaEjercicioInput
                {
                    IdEjercicio = e.IdEjercicio,
                    Series = e.Series,
                    Repeticiones = e.Repeticiones,
                    DescansoSegundos = e.DescansoSegundos,
                    DiaSemana = e.DiaSemana
                }).ToList()
            };

            ViewBag.Ejercicios = new SelectList(await _context.Ejercicios.ToListAsync(), "IdEjercicio", "Nombre");
            ViewBag.DiasSemana = new SelectList(
    new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" }
);
            return View(viewModel);
        }


        // POST: Rutinas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RutinaCreateViewModel model)
        {
            if (id != model.Rutina.IdRutina)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Ejercicios = new SelectList(await _context.Ejercicios.ToListAsync(), "IdEjercicio", "Nombre");
                return View(model);
            }

            try
            {
                var rutinaExistente = await _context.Rutinas
                    .Include(r => r.RutinaEjercicio)
                    .FirstOrDefaultAsync(r => r.IdRutina == id);

                if (rutinaExistente == null) return NotFound();

                // Actualizar los datos generales
                rutinaExistente.Nombre = model.Rutina.Nombre;
                rutinaExistente.Descripcion = model.Rutina.Descripcion;

                // Eliminar los ejercicios existentes
                _context.RutinaEjercicios.RemoveRange(rutinaExistente.RutinaEjercicio);

                // Agregar los nuevos ejercicios
                foreach (var e in model.Ejercicios)
                {
                    var rutinaEjercicio = new RutinaEjercicio
                    {
                        IdRutina = rutinaExistente.IdRutina,
                        IdEjercicio = e.IdEjercicio,
                        Series = e.Series,
                        Repeticiones = e.Repeticiones,
                        DescansoSegundos = e.DescansoSegundos,
                        DiaSemana = e.DiaSemana
                    };

                    _context.RutinaEjercicios.Add(rutinaEjercicio);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutinaExists(model.Rutina.IdRutina)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Rutinas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(m => m.IdRutina == id);
            if (rutina == null)
            {
                return NotFound();
            }

            return View(rutina);
        }

        // POST: Rutinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina != null)
            {
                _context.Rutinas.Remove(rutina);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RutinaExists(int id)
        {
            return _context.Rutinas.Any(e => e.IdRutina == id);
        }
    }
}

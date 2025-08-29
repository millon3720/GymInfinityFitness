using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesina.Data;
using Tesina.Models;


namespace Tesina.Controllers
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
            return View(await _context.Rutinas.OrderBy(a => a.Nombre).ToListAsync());
        }
  
        // GET: mostrar la vista
        public async Task<IActionResult> AsignarRutina(int IdUsuario)
        {
            var rutinas = await _context.Rutinas.OrderBy(a => a.Nombre).ToListAsync();

            var model = new AsignarRutinaViewModel
            {
                IdUsuario = IdUsuario,
                FechaAsignacion = DateTime.Now,
                RutinasDisponibles = rutinas.Select(r => new RutinaItemViewModel
                {
                    IdRutina = r.IdRutina,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion
                }).ToList()
            };

            return View(model);
        }

        // POST: guardar la asignación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsignar(AsignarRutinaViewModel model)
        {
            if (!ModelState.IsValid || model.RutinaSeleccionada == 0)
            {
                ModelState.AddModelError("", "Debes seleccionar una rutina.");
                // Recargar las rutinas en caso de error
                var rutinas = await _context.Rutinas.OrderBy(a => a.Nombre).ToListAsync();
                model.RutinasDisponibles = rutinas.Select(r => new RutinaItemViewModel
                {
                    IdRutina = r.IdRutina,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion
                }).ToList();
                return View("AsignarRutina", model);
            }

            // Buscar si ya existe asignación
            var asignacionExistente = await _context.ClienteRutina
                .FirstOrDefaultAsync(cr => cr.IdUsuario == model.IdUsuario);

            if (asignacionExistente != null)
            {
                // Sobrescribir
                asignacionExistente.IdRutina = model.RutinaSeleccionada;
                asignacionExistente.FechaAsignacion = model.FechaAsignacion;
                asignacionExistente.Observaciones = model.Observaciones;
                _context.Update(asignacionExistente);
            }
            else
            {
                // Crear nueva
                var nuevaAsignacion = new ClienteRutina
                {
                    IdUsuario = model.IdUsuario,
                    IdRutina = model.RutinaSeleccionada,
                    FechaAsignacion = model.FechaAsignacion,
                    Observaciones = model.Observaciones
                };
                _context.Add(nuevaAsignacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Usuarios", new { id = model.IdUsuario });
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
            var ejercicios = await _context.Ejercicios.OrderBy(a => a.Nombre).ToListAsync();
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
                var ejercicios = await _context.Ejercicios.OrderBy(a => a.Nombre).ToListAsync();
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

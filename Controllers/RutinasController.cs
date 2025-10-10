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
        private bool RutinaExists(int id)
        {
            return _context.Rutinas.Any(e => e.IdRutina == id);
        }

        #region Mantenimientos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsignar(AsignarRutinaViewModel model)
        {
            if (model.RutinaSeleccionada == 0)
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
                TempData["Alerta"] = "Por Favor Seleccione Una Rutina.";
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
            TempData["Alerta"] = "✅ Información guardada con éxito.";
            return RedirectToAction("Details", "Usuarios", new { id = model.IdUsuario });
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
            TempData["Alerta"] = "✅ Información guardada con éxito.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RutinaCreateViewModel model)
        {
            if (id != model.Rutina.IdRutina)
                return NotFound();

            if (!ModelState.IsValid || model.Ejercicios == null || !model.Ejercicios.Any())
            {
                ViewBag.ejerciciosList = new SelectList(await _context.Ejercicios.OrderBy(a => a.Nombre).ToListAsync(), "IdEjercicio", "Nombre");
                ViewBag.DiasSemana = new SelectList(new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" });

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
                TempData["Alerta"] = "✅ Información actualizada con éxito.";
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutinaExists(model.Rutina.IdRutina)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
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
            TempData["Alerta"] = "✅ La Rutina se elimino del sistema.";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Views
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.Rutinas.OrderBy(a => a.Nombre).ToListAsync());

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
                }).OrderBy(a => a.Nombre).ToList()
            };
            if (User.Identity.IsAuthenticated)
            {
                return View(model);

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

            var rutina = await _context.Rutinas
                .Include(r => r.RutinaEjercicio)
                .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(m => m.IdRutina == id);

            if (rutina == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(rutina);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public async Task<IActionResult> RutinaCliente(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteRutina = await _context.ClienteRutina
               .Include(cr => cr.Rutina)
               .ThenInclude(r => r.RutinaEjercicio)
               .ThenInclude(re => re.Ejercicio)
               .FirstOrDefaultAsync(cr => cr.IdUsuario == id);
            if (User.Identity.IsAuthenticated)
            {
                return View(clienteRutina);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public async Task<IActionResult> Create()
        {
            var ejercicios = await _context.Ejercicios.OrderBy(a => a.Nombre).ToListAsync();
            ViewBag.Ejercicios = new SelectList(ejercicios, "IdEjercicio", "Nombre");

            var viewModel = new RutinaCreateViewModel();
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

            ViewBag.ejerciciosList = new SelectList(await _context.Ejercicios.OrderBy(a => a.Nombre).ToListAsync(), "IdEjercicio", "Nombre");
            ViewBag.DiasSemana = new SelectList(new[] { "Seleccione un día", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" });
            if (User.Identity.IsAuthenticated)
            {
                return View(viewModel);

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

            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(m => m.IdRutina == id);
            if (rutina == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(rutina);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        #endregion        
    }
}
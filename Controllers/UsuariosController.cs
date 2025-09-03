using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesina.Data;
using Tesina.Models;

namespace Tesina.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly GymDbContext _context;

        public UsuariosController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.Where(a=> a.Rol == "Cliente").OrderBy(a => a.NombreCompleto).ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
                return NotFound();

            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null)
                return NotFound();

            var expedientes = await _context.Expedientes
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaRegistro)
                .ToListAsync();

            var clienteRutina = await _context.ClienteRutina
                .Include(cr => cr.Rutina)                       
                .ThenInclude(r => r.RutinaEjercicio)            
                .ThenInclude(re => re.Ejercicio)                
                .FirstOrDefaultAsync(cr => cr.IdUsuario == id);
            if (clienteRutina != null && clienteRutina.Rutina?.RutinaEjercicio != null)
            {
                var diasSemanaOrdenados = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

                clienteRutina.Rutina.RutinaEjercicio = clienteRutina.Rutina.RutinaEjercicio
                    .OrderBy(re => Array.IndexOf(diasSemanaOrdenados, re.DiaSemana))
                    .ToList();
            }
            var lesiones = await _context.Lesiones
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaDiagnostico)
                .ToListAsync();

            var facturas = await _context.Lesiones
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaDiagnostico)
                .ToListAsync();

            var asistencia = await _context.Asistencias
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaIngreso)
                .ToListAsync();
            var planNutricional = await _context.PlanesNutricionales
                .Where(p => p.IdUsuario == id)
                .Include(p => p.Alimentos)
                .OrderByDescending(p => p.FechaAsignacion)
                .FirstOrDefaultAsync();

            var viewModel = new ClienteDetalle
            {
                Cliente = cliente,
                Expedientes = expedientes,
                ClienteRutina = clienteRutina,
                Rutina = clienteRutina?.Rutina ?? new Rutina(),
                RutinaEjercicios = clienteRutina?.Rutina?.RutinaEjercicio?.ToList() ?? new List<RutinaEjercicio>(),
                Lesion = lesiones,
                Asistencias = asistencia,
                PlanesNutricionales = planNutricional != null ? new List<PlanesNutricionales> { planNutricional } : new List<PlanesNutricionales>(),
                AlimentosPlanNutricional = planNutricional?.Alimentos?.ToList() ?? new List<AlimentosPlanNutricional>()
            };


            return View(viewModel);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cedula,NombreCompleto,FechaNacimiento,Correo,Telefono,Direccion,Rol,FechaRegistro,Estado")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                bool cedulaExiste = await _context.Usuarios
                    .AnyAsync(u => u.Cedula == usuarios.Cedula);

                if (cedulaExiste)
                {
                    ViewBag.Alerta = "Ya existe un usuario con esta cédula.";
                    return View(usuarios);
                }
                _context.Add(usuarios);
                await _context.SaveChangesAsync();

                var login = new UsuarioLogin
                {
                    IdUsuario = usuarios.IdUsuario,
                    Usuario = usuarios.Correo,
                    Contrasena = usuarios.Cedula
                };

                _context.UsuariosLogin.Add(login);
                await _context.SaveChangesAsync();

                return RedirectToAction("Edit", new { id = usuarios.IdUsuario });
            }

            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
                return NotFound();

            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null)
                return NotFound();

            var expedientes = await _context.Expedientes
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaRegistro)
                .ToListAsync();

            var clienteRutina = await _context.ClienteRutina
                .Include(cr => cr.Rutina)
                .ThenInclude(r => r.RutinaEjercicio)
                .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(cr => cr.IdUsuario == id);
            if (clienteRutina != null && clienteRutina.Rutina?.RutinaEjercicio != null)
            {
                var diasSemanaOrdenados = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

                clienteRutina.Rutina.RutinaEjercicio = clienteRutina.Rutina.RutinaEjercicio
                    .OrderBy(re => Array.IndexOf(diasSemanaOrdenados, re.DiaSemana))
                    .ToList();
            }
            var lesiones = await _context.Lesiones
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaDiagnostico)
                .ToListAsync();

            var facturas = await _context.Lesiones
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaDiagnostico)
                .ToListAsync();

            var asistencia = await _context.Asistencias
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaIngreso)
                .ToListAsync();
            var planNutricional = await _context.PlanesNutricionales
                .Where(p => p.IdUsuario == id)
                .Include(p => p.Alimentos)
                .OrderByDescending(p => p.FechaAsignacion)
                .FirstOrDefaultAsync();

            var viewModel = new ClienteDetalle
            {
                Cliente = cliente,
                Expedientes = expedientes,
                ClienteRutina = clienteRutina,
                Rutina = clienteRutina?.Rutina,
                RutinaEjercicios = clienteRutina?.Rutina?.RutinaEjercicio?.ToList() ?? new List<RutinaEjercicio>(),
                Lesion = lesiones,
                Asistencias = asistencia,
                PlanesNutricionales = planNutricional != null ? new List<PlanesNutricionales> { planNutricional } : new List<PlanesNutricionales>(),
                AlimentosPlanNutricional = planNutricional?.Alimentos?.ToList() ?? new List<AlimentosPlanNutricional>()
            };

            return View(viewModel);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteDetalle usuarios)
        {
            if (id != usuarios.Cliente.IdUsuario)
            {
                return View();
            }

            if (usuarios.Cliente!=null)
            {
                try
                {
                    
                    _context.Update(usuarios.Cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.Cliente.IdUsuario))
                    {
                        return View();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View(CargarEdit(id));
        }
        private async Task<ClienteDetalle> CargarEdit(int? id)
        {
            if (id == null || _context.Usuarios == null)
                return null;

            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null)
                return null;

            var expedientes = await _context.Expedientes
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaRegistro)
                .ToListAsync();

            var clienteRutina = await _context.ClienteRutina
                .Include(cr => cr.Rutina)
                .ThenInclude(r => r.RutinaEjercicio)
                .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(cr => cr.IdUsuario == id);
            if (clienteRutina != null && clienteRutina.Rutina?.RutinaEjercicio != null)
            {
                var diasSemanaOrdenados = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

                clienteRutina.Rutina.RutinaEjercicio = clienteRutina.Rutina.RutinaEjercicio
                    .OrderBy(re => Array.IndexOf(diasSemanaOrdenados, re.DiaSemana))
                    .ToList();
            }
            var lesiones = await _context.Lesiones
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaDiagnostico)
                .ToListAsync();

            var facturas = await _context.Lesiones
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaDiagnostico)
                .ToListAsync();

            var asistencia = await _context.Asistencias
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaIngreso)
                .ToListAsync();
            var planNutricional = await _context.PlanesNutricionales
                .Where(p => p.IdUsuario == id)
                .Include(p => p.Alimentos)
                .OrderByDescending(p => p.FechaAsignacion)
                .FirstOrDefaultAsync();

            var viewModel = new ClienteDetalle
            {
                Cliente = cliente,
                Expedientes = expedientes,
                ClienteRutina = clienteRutina,
                Rutina = clienteRutina?.Rutina,
                RutinaEjercicios = clienteRutina?.Rutina?.RutinaEjercicio?.ToList() ?? new List<RutinaEjercicio>(),
                Lesion = lesiones,
                Asistencias = asistencia,
                PlanesNutricionales = planNutricional != null ? new List<PlanesNutricionales> { planNutricional } : new List<PlanesNutricionales>(),
                AlimentosPlanNutricional = planNutricional?.Alimentos?.ToList() ?? new List<AlimentosPlanNutricional>()
            };
            return viewModel;
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}

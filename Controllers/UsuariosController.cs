using Microsoft.AspNetCore.Identity;
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
        private readonly GenerarFacturaPDF _pdf;
        public UsuariosController(GymDbContext context, GenerarFacturaPDF pdf)
        {
            _context = context;
            _pdf = pdf;
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
        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
        private bool MensualidadExists(int id)
        {
            return _context.Mensualidades.Any(e => e.IdUsuario == id);
        }        

        #region Mantenimientos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCliente(int id, ClienteDetalle usuarios)
        {
            if (id != usuarios.Cliente.IdUsuario)
            {
                return View(id);
            }

            if (usuarios != null)
            {
                try
                {

                    _context.Update(usuarios.Cliente);
                    await _context.SaveChangesAsync();

                    var loginExistente = await _context.UsuariosLogin
                        .FirstOrDefaultAsync(l => l.IdUsuario == usuarios.Cliente.IdUsuario);

                    if (loginExistente != null)
                    {
                        loginExistente.Usuario = usuarios.Cliente.Correo;
                        if (!string.IsNullOrWhiteSpace(usuarios.UsuarioLogin.Contrasena))
                        {
                            var hasher = new PasswordHasher<object>();
                            var hash = hasher.HashPassword(null, usuarios.UsuarioLogin.Contrasena);
                            loginExistente.Contrasena = hash;
                        }

                        _context.Update(loginExistente); // ✅ Ahora sí, porque es una entidad rastreada
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.Cliente.IdUsuario))
                    {
                        TempData["Alerta"] = "❌ Error Al Guardar La Informacion.";

                        return View();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Alerta"] = "✅ Cambios guardados correctamente.";
                return RedirectToAction("EditCliente", new { id = usuarios.Cliente.IdUsuario });
            }

            return View(id);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
            }
            TempData["Alerta"] = "✅ El Cliente se elimino del sistema.";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteDetalle usuarios)
        {
            if (id != usuarios.Cliente.IdUsuario)
            {
                return View();
            }

            if (usuarios.Cliente != null)
            {
                try
                {
                    _context.Update(usuarios.Cliente);
                    await _context.SaveChangesAsync();
                    var loginExistente = await _context.UsuariosLogin
                        .FirstOrDefaultAsync(l => l.IdUsuario == usuarios.Cliente.IdUsuario);

                    if (loginExistente != null)
                    {
                        loginExistente.Usuario = usuarios.Cliente.Correo;

                        _context.Update(loginExistente);
                        await _context.SaveChangesAsync();
                    }
                    var mesualidad = new Mensualidades()
                    {
                        IdMensualidad = usuarios.Mensualidades.IdMensualidad,
                        IdUsuario = usuarios.Cliente.IdUsuario,
                        FechaFin = usuarios.Mensualidades.FechaFin,
                        FechaInicio = usuarios.Mensualidades.FechaInicio
                    };
                    if (MensualidadExists(usuarios.Cliente.IdUsuario))
                    {

                        _context.Update(mesualidad);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (mesualidad.FechaInicio != null && mesualidad.FechaFin != null)
                        {
                            _context.Add(mesualidad);
                            await _context.SaveChangesAsync();
                        }

                    }


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
                TempData["Alerta"] = "✅ Información actualizada con éxito.";
                return View(await CargarEdit(id));
            }

            return View(await CargarEdit(id));
        }
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
                    ViewBag.Alerta = "⚠️ Ya existe un usuario con esta cédula.";
                    return View(usuarios);
                }
                _context.Add(usuarios);
                await _context.SaveChangesAsync();
                var hasher = new PasswordHasher<object>();
                var Contrasenahash = Guid.NewGuid().ToString("N")[..10];
                var login = new UsuarioLogin
                {
                    IdUsuario = usuarios.IdUsuario,
                    Usuario = usuarios.Correo,
                    Contrasena = hasher.HashPassword(null, Contrasenahash)
                };

                _context.UsuariosLogin.Add(login);
                await _context.SaveChangesAsync();
                await _pdf.EnviarNotificacionRegistroAsync(login.Usuario, usuarios.NombreCompleto, Contrasenahash);
                TempData["Alerta"] = "✅ Información guardada con éxito.";
                return RedirectToAction("Edit", new { id = usuarios.IdUsuario });
            }

            return View(usuarios);
        }
        #endregion

        #region Views
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(await _context.Usuarios.Where(a => a.Rol == "Cliente").OrderBy(a => a.NombreCompleto).ToListAsync());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }
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


            var asistencia = await _context.Asistencias
                .Where(e => e.IdUsuario == id)
                .OrderBy(e => e.FechaIngreso)
                .ToListAsync();
            var planNutricional = await _context.PlanesNutricionales
                .Where(p => p.IdUsuario == id)
                .Include(p => p.Alimentos)
                .OrderByDescending(p => p.FechaAsignacion)
                .FirstOrDefaultAsync();
            var mensualidad = await _context.Mensualidades.FirstOrDefaultAsync(m => m.IdUsuario == id);


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
                AlimentosPlanNutricional = planNutricional?.Alimentos?.ToList() ?? new List<AlimentosPlanNutricional>(),
                Mensualidades = mensualidad
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
        public IActionResult Create()
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
            if (id == null || _context.Usuarios == null)
                return NotFound();

            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null)
                return NotFound();

            var expedientes = await _context.Expedientes
                .Where(e => e.IdUsuario == id)
                .OrderByDescending(e => e.FechaRegistro)
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
                .OrderByDescending(e => e.FechaDiagnostico)
                .ToListAsync();
            var asistencia = await _context.Asistencias
                .Where(e => e.IdUsuario == id)
                .OrderByDescending(e => e.FechaIngreso)
                .ToListAsync();
            var planNutricional = await _context.PlanesNutricionales
                .Where(p => p.IdUsuario == id)
                .Include(p => p.Alimentos)
                .OrderByDescending(p => p.FechaAsignacion)
                .FirstOrDefaultAsync();
            var mensualidad = await _context.Mensualidades.FirstOrDefaultAsync(m => m.IdUsuario == id);

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
                AlimentosPlanNutricional = planNutricional?.Alimentos?.ToList() ?? new List<AlimentosPlanNutricional>(),
                Mensualidades = mensualidad
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
            if (User.Identity.IsAuthenticated)
            {
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public async Task<IActionResult> EditCliente(int? id)
        {

            if (id == null || _context.Usuarios == null)
                return NotFound();

            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null)
                return NotFound();
            var mensualidad = await _context.Mensualidades.FirstOrDefaultAsync(m => m.IdUsuario == id);
            var UsuarioLogin = await _context.UsuariosLogin.FindAsync(id);
            var viewModel = new ClienteDetalle
            {
                Cliente = cliente,
                Mensualidades = mensualidad,
                UsuarioLogin = UsuarioLogin
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
        public async Task<IActionResult> AsistenciaCliente(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Asistencias = await _context.Asistencias
                .Where(e => e.IdUsuario == id).OrderByDescending(e => e.FechaIngreso)
                .ToListAsync();
            if (Asistencias == null)
            {
                return View(Asistencias);
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(Asistencias);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }
        #endregion
    }
}
using Microsoft.AspNetCore.Identity;
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
    public class EntrenadoresController : Controller
    {
        private readonly GymDbContext _context;
        private readonly GenerarFacturaPDF _pdf;

        public EntrenadoresController(GymDbContext context, GenerarFacturaPDF pdf)
        {
            _context = context;
            _pdf = pdf;
        }

        // GET: Entrenadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.Where(a=> a.Rol=="Entrenador").OrderBy(a => a.NombreCompleto).ToListAsync());
        }

        // GET: Entrenadores/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Entrenadores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entrenadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Cedula,NombreCompleto,FechaNacimiento,Correo,Telefono,Direccion,Rol,FechaRegistro,Estado")] Usuarios usuarios)
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
                return RedirectToAction(nameof(Index));
            }            
            return View(usuarios);
        }

        // GET: Entrenadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound();
            }
            var UsuarioLogin = await _context.UsuariosLogin.FindAsync(id);
            var viewModel = new ClienteDetalle
            {
                Cliente = usuarios,
                UsuarioLogin = new UsuarioLogin()
            };
            return View(viewModel);
        }

        // POST: Entrenadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ClienteDetalle usuarios)
        {
            if (id != usuarios.Cliente.IdUsuario)
            {
                return NotFound();
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
            return View(usuarios);
        }

        // GET: Entrenadores/Delete/5
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

        // POST: Entrenadores/Delete/5
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
            TempData["Alerta"] = "✅ El Entrenador se elimino del sistema.";
            return RedirectToAction(nameof(Index));
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}

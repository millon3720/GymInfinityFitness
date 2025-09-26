using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;
using Tesina.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Tesina.Controllers
{
    public class LoginController : Controller
    {
        private readonly GymDbContext _context;

        public LoginController(GymDbContext context)
        {
            _context = context;
        }
        // GET: LoginController
        //public ActionResult Login()
        //{
        //    return View();
        //}
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: LoginController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UsuarioLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.UsuariosLogin.Include(p => p.InfoUsuario)
                .FirstOrDefaultAsync(u => u.Usuario == model.Usuario && u.Contrasena == model.Contrasena);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // Crear claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Usuario),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("Nombre", usuario.InfoUsuario.NombreCompleto.ToString()),
                new Claim("Rol", usuario.InfoUsuario.Rol.ToString())
                // Podés agregar más claims si querés (rol, nombre completo, etc.)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Autenticar
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
 
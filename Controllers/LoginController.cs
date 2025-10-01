using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tesina.Data;
using Tesina.Models;

namespace Tesina.Controllers
{
    public class LoginController : Controller
    {
        private readonly GymDbContext _context;
        private readonly GenerarFacturaPDF _pdf;

        public LoginController(GymDbContext context, GenerarFacturaPDF pdf)
        {
            _context = context;
            _pdf = pdf;
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
        public IActionResult SolicitarRecuperacion()
        {
            return View(new UsuarioLogin());
        }

        // POST: LoginController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Login(UsuarioLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Buscar por nombre de usuario
            var usuario = await _context.UsuariosLogin
                .Include(p => p.InfoUsuario)
                .FirstOrDefaultAsync(u => u.Usuario == model.Usuario);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // Verificar contraseña hasheada
            var hasher = new PasswordHasher<object>();
            var resultado = hasher.VerifyHashedPassword(null, usuario.Contrasena, model.Contrasena);

            if (resultado != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // Crear claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Usuario),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("Nombre", usuario.InfoUsuario.NombreCompleto),
                new Claim("Rol", usuario.InfoUsuario.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Autenticar
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RecuperarContrasena(UsuarioLogin model)
        {
            if (string.IsNullOrWhiteSpace(model.Usuario))
            {
                TempData["Alerta"] = "❌ Debe ingresar un correo válido.";
                return RedirectToAction("Login");
            }

            var usuarioLogin = await _context.UsuariosLogin
                .Include(u => u.InfoUsuario)
                .FirstOrDefaultAsync(u => u.Usuario == model.Usuario);

            if (usuarioLogin == null)
            {
                TempData["Alerta"] = "❌ No se encontró ningún usuario con ese correo.";
                return RedirectToAction("Login");
            }

            // Generar contraseña temporal
            var nuevaContrasena = Guid.NewGuid().ToString("N")[..10];

            // Hashearla
            var hasher = new PasswordHasher<object>();
            var hash = hasher.HashPassword(null, nuevaContrasena);

            // Guardar en la base de datos
            usuarioLogin.Contrasena = hash;
            _context.Update(usuarioLogin);
            await _context.SaveChangesAsync();
            // Enviar correo usando el método externo
            await _pdf.EnviarRecuperacionContrasenaAsync(
                correoDestino: model.Usuario,
                nombreUsuario: usuarioLogin.InfoUsuario.NombreCompleto,
                nuevaContrasena: nuevaContrasena
            );

            TempData["Alerta"] = "✅ Se ha enviado una nueva contraseña temporal a tu correo.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
 
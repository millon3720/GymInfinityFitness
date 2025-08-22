using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;
using Tesina.Models;

namespace WebApplication1.Controllers
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
        public ActionResult Login(UsuarioLogin model)
        {
            return View(model);
        }

        // POST: LoginController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UsuarioLogin model, IFormCollection collection)
        {
            if (!ModelState.IsValid)
                return Login(model);

            var usuario = await _context.UsuariosLogin
                .FirstOrDefaultAsync(u => u.Usuario == model.Usuario && u.Contrasena == model.Contrasena);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return Login(model);
            }

            return RedirectToAction("Index", "Home");
        }

       
    }
}

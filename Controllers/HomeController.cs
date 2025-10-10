using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;

namespace Tesina.Controlador
{
    public class HomeController : Controller
    {
        private readonly GymDbContext _context;
        public HomeController(GymDbContext context)
        {
            _context = context;
        }
        public IActionResult VerImagen(int id)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.IdEvento == id);
            if (evento?.Imagen == null) return NotFound();
            return File(evento.Imagen, "image/jpeg"); // o "image/png"
        }

        #region Mantenimientos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Views
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }
            var eventos = await _context.Eventos
                .Where(e => e.FechaEvento >= DateTime.Now)
                .OrderBy(e => e.FechaEvento)
                .ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                return View(eventos);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Details(int id)
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
        public ActionResult Create()
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
        public ActionResult Edit(int id)
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
        public ActionResult Delete(int id)
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
        #endregion      
    }
}
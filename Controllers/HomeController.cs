using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;

namespace Tesina.Controlador
{
    public class HomeController : Controller
    {
        // GET: HomeController 
        private readonly GymDbContext _context;

        public HomeController(GymDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");// Página para usuarios logueados
            }
            var eventos = await _context.Eventos
                .Where(e => e.FechaEvento >= DateTime.Now)
                .OrderBy(e => e.FechaEvento)
                .ToListAsync(); 
            return View(eventos);
        }

        public IActionResult VerImagen(int id)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.IdEvento == id);
            if (evento?.Imagen == null) return NotFound();
            return File(evento.Imagen, "image/jpeg"); // o "image/png"
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
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

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
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

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
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
    }
}

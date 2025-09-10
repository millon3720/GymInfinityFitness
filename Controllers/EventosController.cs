using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;
using Tesina.Models;

namespace Tesina.Controllers
{
    public class EventosController : Controller
    {
        private readonly GymDbContext _context;

        public EventosController(GymDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var eventos = await _context.Eventos.OrderBy(e => e.FechaEvento).ToListAsync();
            return View(eventos);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Eventos evento, IFormFile imagenArchivo)
        {
            if (ModelState.IsValid)
            {
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await imagenArchivo.CopyToAsync(ms);
                    evento.Imagen = ms.ToArray();
                }

                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(evento);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]    
        public async Task<IActionResult> Edit(int id, Eventos evento, IFormFile imagenArchivo)
        {
            if (id != evento.IdEvento)
                return NotFound();

            var eventoExistente = await _context.Eventos.FindAsync(id);
            if (eventoExistente == null)
                return NotFound();

            // Actualizar campos básicos
            eventoExistente.Nombre = evento.Nombre;
            eventoExistente.Descripcion = evento.Descripcion;
            eventoExistente.FechaEvento = evento.FechaEvento;

            // Solo actualizar imagen si se sube una nueva
            if (imagenArchivo != null && imagenArchivo.Length > 0)
            {
                using var ms = new MemoryStream();
                await imagenArchivo.CopyToAsync(ms);
                eventoExistente.Imagen = ms.ToArray();
            }
            ModelState.Clear();
            // Validar el objeto final que se va a guardar
            if (!TryValidateModel(eventoExistente))
            {
                return View(eventoExistente); // Devolvés el objeto completo, con imagen
            }

            _context.Update(eventoExistente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // DELETE (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // IMAGEN (GET)
        public async Task<IActionResult> VerImagen(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento?.Imagen != null)
            {
                return File(evento.Imagen, "image/jpeg");
            }

            return NotFound();
        }
    }

}

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
        public async Task<IActionResult> EventoCliente()
        {

            var idUsuario = Convert.ToInt32(User.FindFirst("IdUsuario")?.Value);

            var eventos = await _context.Eventos
                .Where(e => e.FechaEvento > DateTime.Now)
                .OrderBy(e => e.FechaEvento)
                .ToListAsync();

            var inscripciones = await _context.ClienteEvento
                .Where(i => i.IdUsuario == idUsuario)
                .Select(i => i.IdEvento)
                .ToListAsync();

            ViewBag.EventosInscritos = inscripciones;

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
        public async Task<IActionResult> Inscribirse(int IdEvento, int IdUsuario)
        {
            if (IdEvento == 0 || IdUsuario == 0)
                return BadRequest("Parámetros inválidos.");

            var yaInscrito = await _context.ClienteEvento
                .AnyAsync(i => i.IdEvento == IdEvento && i.IdUsuario == IdUsuario);

            if (yaInscrito)
            {
                TempData["Alerta"] = "⚠️ Ya estás inscrito en este evento.";
                return RedirectToAction("EventoCliente");
            }

            var inscripcion = new ClienteEvento
            {
                IdEvento = IdEvento,
                IdUsuario = IdUsuario,
                FechaInscripcion = DateTime.Now
            };

            _context.ClienteEvento.Add(inscripcion);
            await _context.SaveChangesAsync();

            TempData["Alerta"] = "✅ Inscripción realizada con éxito.";
            return RedirectToAction("EventoCliente");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnularInscripcion(int IdEvento, int IdUsuario)
        {
            var inscripcion = await _context.ClienteEvento
                .FirstOrDefaultAsync(i => i.IdEvento == IdEvento && i.IdUsuario == IdUsuario);

            if (inscripcion != null)
            {
                _context.ClienteEvento.Remove(inscripcion);
                await _context.SaveChangesAsync();
                TempData["Alerta"] = "❌ Inscripción anulada correctamente.";
            }
            else
            {
                TempData["Alerta"] = "⚠️ No estás inscrito en este evento.";
            }

            return RedirectToAction("EventoCliente");
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
                TempData["Alerta"] = "✅ Información guardada con éxito.";

                return RedirectToAction(nameof(Index));
            }

            return View(evento);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Inscripciones)
                .ThenInclude(ce => ce.Usuario)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento != null)
            {
                evento.Inscripciones = evento.Inscripciones
                    .OrderBy(i => i.Usuario.NombreCompleto)
                    .ToList();
            }

            if (evento == null)
                return View("index");

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
            TempData["Alerta"] = "✅ Información actualizada con éxito.";

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
            TempData["Alerta"] = "✅ El Evento se elimino del sistema.";
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

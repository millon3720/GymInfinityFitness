using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tesina.Data;
using Tesina.Models;

namespace WebApplication1.Controllers
{
    public class ExpedientesController : Controller
    {
        private readonly GymDbContext _context;
        public ExpedientesController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Expedientes
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Expedientes.Include(e => e.Usuario);
            return View(await gymDbContext.ToListAsync());
        }

        // GET: Expedientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.IdExpediente == id);
            if (expediente == null)
            {
                return NotFound();
            }

            return View(expediente);
        }

        // GET: Expedientes/Create
        public IActionResult Create(int id)
        {        
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios.Where(u => u.Rol == "Cliente"), 
                "IdUsuario", "NombreCompleto", selectedValue: (id != 0 ? id : (int?)null));

            return View();
        }
       
        // POST: Expedientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expediente expediente)
        {

            if (ModelState.IsValid)
            {
                _context.Add(expediente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details",
                                        "Usuarios",
                                        new { id = expediente.IdUsuario });


            }

            // 🔹 Reconstruir el SelectList con el valor ya seleccionado
            ViewData["IdUsuario"] = new SelectList(
                _context.Usuarios.Where(u => u.Rol == "Cliente"),
                "IdUsuario",
                "NombreCompleto",
                expediente.IdUsuario // <- mantiene el valor seleccionado
            );

            // 🔹 Mantener el valor de Agregar en la vista

            return View(expediente);
        }

        // GET: Expedientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes.FindAsync(id);
            if (expediente == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", expediente.IdUsuario);
            return View(expediente);
        }

        // POST: Expedientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdExpediente,IdUsuario,FechaRegistro,PesoKg,AlturaCm,IMC,PorcentajeGrasa,PorcentajeMuscular,MedidaPecho,MedidaEspalda,MedidaCintura,MedidaCadera,BicepDerecho,BicepIzquierdo,PiernaDerecha,PiernaIzquierda,PantorrillaDerecha,PantorrillaIzquierda,Observaciones")] Expediente expediente)
        {
            if (id != expediente.IdExpediente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expediente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpedienteExists(expediente.IdExpediente))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details",
                                                        "Usuarios",
                                                        new { id = expediente.IdUsuario });
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", expediente.IdUsuario);
            return View(expediente);
        }

        // GET: Expedientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.IdExpediente == id);
            if (expediente == null)
            {
                return NotFound();
            }

            return View(expediente);
        }

        // POST: Expedientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expediente = await _context.Expedientes.FindAsync(id);
            if (expediente != null)
            {
                _context.Expedientes.Remove(expediente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details",
                                                    "Usuarios",
                                                    new { id = expediente.IdUsuario });
        }

        private bool ExpedienteExists(int id)
        {
            return _context.Expedientes.Any(e => e.IdExpediente == id);
        }
    }
}

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
    public class PlanesNutricionalesController : Controller
    {
        private readonly GymDbContext _context;

        public PlanesNutricionalesController(GymDbContext context)
        {
            _context = context;
        }

        // GET: PlanesNutricionales
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.PlanesNutricionales.Include(p => p.Usuario);
            return View(await gymDbContext.ToListAsync());
        }

        // GET: PlanesNutricionales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planesNutricionales = await _context.PlanesNutricionales
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.IdPlan == id);
            if (planesNutricionales == null)
            {
                return NotFound();
            }

            return View(planesNutricionales);
        }

        // GET: PlanesNutricionales/Create
        public IActionResult Create()
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula");
            return View();
        }

        // POST: PlanesNutricionales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPlan,IdUsuario,Descripcion,FechaAsignacion")] PlanesNutricionales planesNutricionales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planesNutricionales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", planesNutricionales.IdUsuario);
            return View(planesNutricionales);
        }

        // GET: PlanesNutricionales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planesNutricionales = await _context.PlanesNutricionales.FindAsync(id);
            if (planesNutricionales == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", planesNutricionales.IdUsuario);
            return View(planesNutricionales);
        }

        // POST: PlanesNutricionales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlan,IdUsuario,Descripcion,FechaAsignacion")] PlanesNutricionales planesNutricionales)
        {
            if (id != planesNutricionales.IdPlan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planesNutricionales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanesNutricionalesExists(planesNutricionales.IdPlan))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", planesNutricionales.IdUsuario);
            return View(planesNutricionales);
        }

        // GET: PlanesNutricionales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planesNutricionales = await _context.PlanesNutricionales
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.IdPlan == id);
            if (planesNutricionales == null)
            {
                return NotFound();
            }

            return View(planesNutricionales);
        }

        // POST: PlanesNutricionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planesNutricionales = await _context.PlanesNutricionales.FindAsync(id);
            if (planesNutricionales != null)
            {
                _context.PlanesNutricionales.Remove(planesNutricionales);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanesNutricionalesExists(int id)
        {
            return _context.PlanesNutricionales.Any(e => e.IdPlan == id);
        }
    }
}

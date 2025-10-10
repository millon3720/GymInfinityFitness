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
    public class ExpedientesController : Controller
    {
        private readonly GymDbContext _context;
        public ExpedientesController(GymDbContext context)
        {
            _context = context;
        }
        private bool ExpedienteExists(int id)
        {
            return _context.Expedientes.Any(e => e.IdExpediente == id);
        }

        #region Mantenimientos
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
            TempData["Alerta"] = "✅ Información guardada con éxito.";

            // 🔹 Mantener el valor de Agregar en la vista

            return View(expediente);
        }
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
                    TempData["Alerta"] = "✅ Información actualizada con éxito.";

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
            TempData["Alerta"] = "✅ El Registro se elimino del sistema.";
            return RedirectToAction("Details",
                                                    "Usuarios",
                                                    new { id = expediente.IdUsuario });
        }
        #endregion

        #region Views
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Expedientes.Include(e => e.Usuario);
            if (User.Identity.IsAuthenticated)
            {
                return View(await gymDbContext.ToListAsync());

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
            if (User.Identity.IsAuthenticated)
            {
                return View(expediente);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public async Task<IActionResult> MedidasCliente(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes
                .Where(e => e.IdUsuario == id)
                .OrderByDescending(e => e.FechaRegistro)
                .ToListAsync();
            if (expediente == null)
            {
                return View(expediente);
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(expediente);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult Create(int id)
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios.Where(u => u.Rol == "Cliente"),
                "IdUsuario", "NombreCompleto", selectedValue: (id != 0 ? id : (int?)null));
            if (User.Identity.IsAuthenticated)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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

            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreCompleto", expediente.IdUsuario);
            if (User.Identity.IsAuthenticated)
            {
                return View(expediente);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
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
            if (User.Identity.IsAuthenticated)
            {
                return View(expediente);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        #endregion      
    }
}
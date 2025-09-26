using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesina.Data;
using Tesina.Models;

namespace Tesina.Controllers
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

            var plan = await _context.PlanesNutricionales.Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.IdPlan == id);

            if (plan == null)
            {
                return NotFound();
            }

            var alimentos = await _context.AlimentosPlanNutricional
                .Where(a => a.IdPlan == id)
                .ToListAsync();

            var viewModel = new PlanAlimenticioViewModel
            {
                Plan = plan,
                Alimentos = alimentos
            };

            return View(viewModel);
        }
        public async Task<IActionResult> PlanCliente(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.PlanesNutricionales.Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.IdUsuario == id);
            var alimentos = await _context.AlimentosPlanNutricional
               .Where(a => a.IdPlan == plan.IdPlan)
               .ToListAsync();

            var viewModel = new PlanAlimenticioViewModel
            {
                Plan = plan,
                Alimentos = alimentos
            }; 
            if (plan == null)
            {
                return View(viewModel);
            }

           

            return View(viewModel);
        }

        // GET: PlanesNutricionales/Create
        public IActionResult Create()
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios.Where(a => a.Rol == "Cliente"), "IdUsuario", "NombreCompleto");
            return View();
        }

        // POST: PlanesNutricionales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanAlimenticioViewModel model)
        {
            var existePlan = await _context.PlanesNutricionales
                .AnyAsync(p => p.IdUsuario == model.Plan.IdUsuario);

            if (existePlan && !Request.Form.ContainsKey("Sobreescribir"))
            {
                TempData["Alimentos"] = JsonConvert.SerializeObject(model.Alimentos);
                ViewBag.ExistePlan = true;
                ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", model.Plan.IdUsuario);
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var plan = model.Plan;
                plan.Alimentos = model.Alimentos;
                
                // Si ya existe y se desea sobreescribir, eliminamos el anterior
                if (existePlan)
                {
                    var anterior = await _context.PlanesNutricionales
                        .Include(p => p.Alimentos)
                        .FirstOrDefaultAsync(p => p.IdUsuario == model.Plan.IdUsuario);

                    _context.AlimentosPlanNutricional.RemoveRange(anterior.Alimentos);
                    _context.PlanesNutricionales.Remove(anterior);
                }
                _context.Add(plan);
                await _context.SaveChangesAsync();
                if (TempData["Alimentos"] != null)
                {
                    model.Alimentos = JsonConvert.DeserializeObject<List<AlimentosPlanNutricional>>(
                        TempData["Alimentos"].ToString()
                    );
                }
                // Asignar el IdPlan generado a cada alimento
                foreach (var alimento in model.Alimentos)
                {
                    alimento.IdPlan = plan.IdPlan;
                }

                // Guardar los alimentos
                _context.AlimentosPlanNutricional.AddRange(model.Alimentos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Cedula", model.Plan.IdUsuario);
            return View(model);
        }



        // GET: PlanesNutricionales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.PlanesNutricionales
                .Include(p => p.Alimentos).Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.IdPlan == id);

            if (plan == null) return NotFound();

            var viewModel = new PlanAlimenticioViewModel
            {
                Plan = plan,
                Alimentos = plan.Alimentos.ToList()
            };
            ViewBag.DiasSemana = new SelectList(new[] { "Seleccione un día", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" });
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreCompleto", plan.IdUsuario);
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlanAlimenticioViewModel model)
        {
            if (id != model.Plan.IdPlan) return NotFound();
            
            if (ModelState.IsValid)
            {
                var planDb = await _context.PlanesNutricionales
                    .Include(p => p.Alimentos).Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.IdPlan == id);

                if (planDb == null) return NotFound();

                // Actualizar datos del plan
                planDb.Descripcion = model.Plan.Descripcion;
                planDb.FechaAsignacion = model.Plan.FechaAsignacion;
                planDb.IdUsuario = model.Plan.IdUsuario;

                // Manejo de alimentos
                var alimentosDb = planDb.Alimentos.ToList();

                foreach (var alimentoVm in model.Alimentos)
                {
                    if (alimentoVm.Id == 0)
                    {
                        // Nuevo → se agrega
                        alimentoVm.IdPlan = planDb.IdPlan;
                        _context.AlimentosPlanNutricional.Add(alimentoVm);
                    }
                    else
                    {
                        // Ya existe → actualizar
                        var alimentoDb = alimentosDb.FirstOrDefault(a => a.Id == alimentoVm.Id);
                        if (alimentoDb != null)
                        {
                            alimentoDb.DiaSemana = alimentoVm.DiaSemana;
                            alimentoDb.Comida = alimentoVm.Comida;
                            alimentoDb.HoraEstimada = alimentoVm.HoraEstimada;
                            alimentoDb.Alimento = alimentoVm.Alimento;
                            alimentoDb.Porciones = alimentoVm.Porciones;
                            alimentoDb.Comentarios = alimentoVm.Comentarios;
                        }
                    }
                }

                // Eliminar los que ya no estén en el modelo
                foreach (var alimentoDb in alimentosDb)
                {
                    if (!model.Alimentos.Any(a => a.Id == alimentoDb.Id))
                    {
                        _context.AlimentosPlanNutricional.Remove(alimentoDb);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DiasSemana = new SelectList(new[] { "Seleccione un día", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" });
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreCompleto", model.Plan.IdUsuario);
            return View(model);
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


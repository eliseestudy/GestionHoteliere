using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Web.Services;

namespace Web.Controllers
{
    public class TypeChambresController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;

        public TypeChambresController(GestionHoteliereDbContext context, IBusinessRulesService rules)
        {
            _context = context;
            _rules = rules;
        }

        // GET: TypeChambres
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypesChambres.ToListAsync());
        }

        // GET: TypeChambres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeChambre = await _context.TypesChambres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeChambre == null)
            {
                return NotFound();
            }

            return View(typeChambre);
        }

        // GET: TypeChambres/Create
        public IActionResult Create()
        {
            return View(new TypeChambre());
        }

        // POST: TypeChambres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Libelle,Description,Capacite,PrixParNuit")] TypeChambre typeChambre)
        {
            if (ModelState.IsValid)
            {
                typeChambre.CreatedAt = DateTimeOffset.UtcNow;
                typeChambre.CreatedById = null;
                typeChambre.IsDeleted = false;
                typeChambre.UpdatedAt = null;
                typeChambre.UpdatedById = null;
                typeChambre.DeletedAt = null;
                typeChambre.DeletedById = null;

                _context.Add(typeChambre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeChambre);
        }

        // GET: TypeChambres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeChambre = await _context.TypesChambres.FindAsync(id);
            if (typeChambre == null)
            {
                return NotFound();
            }
            return View(typeChambre);
        }

        // POST: TypeChambres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Libelle,Description,Capacite,PrixParNuit,Id")] TypeChambre typeChambre)
        {
            if (id != typeChambre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTypeChambre = await _context.TypesChambres.FindAsync(id);
                    if (existingTypeChambre == null)
                    {
                        return NotFound();
                    }

                    existingTypeChambre.Libelle = typeChambre.Libelle;
                    existingTypeChambre.Description = typeChambre.Description;
                    existingTypeChambre.Capacite = typeChambre.Capacite;
                    existingTypeChambre.PrixParNuit = typeChambre.PrixParNuit;
                    existingTypeChambre.UpdatedAt = DateTimeOffset.UtcNow;
                    existingTypeChambre.UpdatedById = null;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeChambreExists(typeChambre.Id))
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
            return View(typeChambre);
        }

        // GET: TypeChambres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeChambre = await _context.TypesChambres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeChambre == null)
            {
                return NotFound();
            }

            return View(typeChambre);
        }

        // POST: TypeChambres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeChambre = await _context.TypesChambres.FindAsync(id);
            if (typeChambre != null)
            {
                if (!await _rules.CanDeleteTypeChambreAsync(id))
                {
                    TempData["Toast.Type"] = "warning";
                    TempData["Toast.Message"] = "Ce type de chambre est utilise et ne peut pas etre supprime.";
                    return RedirectToAction(nameof(Index));
                }

                typeChambre.IsDeleted = true;
                typeChambre.DeletedAt = DateTimeOffset.UtcNow;
                typeChambre.DeletedById = null;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeChambreExists(int id)
        {
            return _context.TypesChambres.Any(e => e.Id == id);
        }
    }
}

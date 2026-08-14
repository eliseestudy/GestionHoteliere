using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;

namespace Web.Controllers
{
    public class PaiementsController : Controller
    {
        private readonly GestionHoteliereDbContext _context;

        public PaiementsController(GestionHoteliereDbContext context)
        {
            _context = context;
        }

        // GET: Paiements
        public async Task<IActionResult> Index()
        {
            var gestionHoteliereDbContext = _context.Paiements.Include(p => p.Facture).Include(p => p.User);
            return View(await gestionHoteliereDbContext.ToListAsync());
        }

        // GET: Paiements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paiement = await _context.Paiements
                .Include(p => p.Facture)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paiement == null)
            {
                return NotFound();
            }

            return View(paiement);
        }

        // GET: Paiements/Create
        public IActionResult Create()
        {
            ViewData["FactureId"] = new SelectList(_context.Factures, "Id", "NumeroFacture");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "PasswordHash");
            return View();
        }

        // POST: Paiements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FactureId,DatePaiement,Montant,Mode,Statut,TransactionReference,UserId,Id,CreatedAt,CreatedById,UpdatedAt,UpdatedById,IsDeleted,DeletedAt,DeletedById,RowVersion")] Paiement paiement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paiement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FactureId"] = new SelectList(_context.Factures, "Id", "NumeroFacture", paiement.FactureId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "PasswordHash", paiement.UserId);
            return View(paiement);
        }

        // GET: Paiements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement == null)
            {
                return NotFound();
            }
            ViewData["FactureId"] = new SelectList(_context.Factures, "Id", "NumeroFacture", paiement.FactureId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "PasswordHash", paiement.UserId);
            return View(paiement);
        }

        // POST: Paiements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FactureId,DatePaiement,Montant,Mode,Statut,TransactionReference,UserId,Id,CreatedAt,CreatedById,UpdatedAt,UpdatedById,IsDeleted,DeletedAt,DeletedById,RowVersion")] Paiement paiement)
        {
            if (id != paiement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paiement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaiementExists(paiement.Id))
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
            ViewData["FactureId"] = new SelectList(_context.Factures, "Id", "NumeroFacture", paiement.FactureId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "PasswordHash", paiement.UserId);
            return View(paiement);
        }

        // GET: Paiements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paiement = await _context.Paiements
                .Include(p => p.Facture)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paiement == null)
            {
                return NotFound();
            }

            return View(paiement);
        }

        // POST: Paiements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement != null)
            {
                _context.Paiements.Remove(paiement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaiementExists(int id)
        {
            return _context.Paiements.Any(e => e.Id == id);
        }
    }
}

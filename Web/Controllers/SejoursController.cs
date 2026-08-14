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
    public class SejoursController : Controller
    {
        private readonly GestionHoteliereDbContext _context;

        public SejoursController(GestionHoteliereDbContext context)
        {
            _context = context;
        }

        // GET: Sejours
        public async Task<IActionResult> Index()
        {
            var gestionHoteliereDbContext = _context.Sejours.Include(s => s.Chambre).Include(s => s.Client).Include(s => s.Reservation);
            return View(await gestionHoteliereDbContext.ToListAsync());
        }

        // GET: Sejours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sejour = await _context.Sejours
                .Include(s => s.Chambre)
                .Include(s => s.Client)
                .Include(s => s.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sejour == null)
            {
                return NotFound();
            }

            return View(sejour);
        }

        // GET: Sejours/Create
        public IActionResult Create()
        {
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero");
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom");
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "NumeroReservation");
            return View();
        }

        // POST: Sejours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,ClientId,ChambreId,DateEntree,DateSortie,NbNuits,TarifApplique,Remise,Statut,Id,CreatedAt,CreatedById,UpdatedAt,UpdatedById,IsDeleted,DeletedAt,DeletedById,RowVersion")] Sejour sejour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sejour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero", sejour.ChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", sejour.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "NumeroReservation", sejour.ReservationId);
            return View(sejour);
        }

        // GET: Sejours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sejour = await _context.Sejours.FindAsync(id);
            if (sejour == null)
            {
                return NotFound();
            }
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero", sejour.ChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", sejour.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "NumeroReservation", sejour.ReservationId);
            return View(sejour);
        }

        // POST: Sejours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,ClientId,ChambreId,DateEntree,DateSortie,NbNuits,TarifApplique,Remise,Statut,Id,CreatedAt,CreatedById,UpdatedAt,UpdatedById,IsDeleted,DeletedAt,DeletedById,RowVersion")] Sejour sejour)
        {
            if (id != sejour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sejour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SejourExists(sejour.Id))
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
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero", sejour.ChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", sejour.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "NumeroReservation", sejour.ReservationId);
            return View(sejour);
        }

        // GET: Sejours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sejour = await _context.Sejours
                .Include(s => s.Chambre)
                .Include(s => s.Client)
                .Include(s => s.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sejour == null)
            {
                return NotFound();
            }

            return View(sejour);
        }

        // POST: Sejours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sejour = await _context.Sejours.FindAsync(id);
            if (sejour != null)
            {
                _context.Sejours.Remove(sejour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SejourExists(int id)
        {
            return _context.Sejours.Any(e => e.Id == id);
        }
    }
}

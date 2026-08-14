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
    public class ReservationsController : Controller
    {
        private readonly GestionHoteliereDbContext _context;

        public ReservationsController(GestionHoteliereDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var gestionHoteliereDbContext = _context.Reservations.Include(r => r.Chambre).Include(r => r.Client).Include(r => r.TypeChambre);
            return View(await gestionHoteliereDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Chambre)
                .Include(r => r.Client)
                .Include(r => r.TypeChambre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero");
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom");
            ViewData["TypeChambreId"] = new SelectList(_context.TypesChambres, "Id", "Libelle");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroReservation,ClientId,ChambreId,TypeChambreId,DateCreation,DateArrivee,DateDepart,Statut,NombrePersonnes,MontantEstime,Id,CreatedAt,CreatedById,UpdatedAt,UpdatedById,IsDeleted,DeletedAt,DeletedById,RowVersion")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero", reservation.ChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", reservation.ClientId);
            ViewData["TypeChambreId"] = new SelectList(_context.TypesChambres, "Id", "Libelle", reservation.TypeChambreId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero", reservation.ChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", reservation.ClientId);
            ViewData["TypeChambreId"] = new SelectList(_context.TypesChambres, "Id", "Libelle", reservation.TypeChambreId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumeroReservation,ClientId,ChambreId,TypeChambreId,DateCreation,DateArrivee,DateDepart,Statut,NombrePersonnes,MontantEstime,Id,CreatedAt,CreatedById,UpdatedAt,UpdatedById,IsDeleted,DeletedAt,DeletedById,RowVersion")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["ChambreId"] = new SelectList(_context.Chambres, "Id", "Numero", reservation.ChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", reservation.ClientId);
            ViewData["TypeChambreId"] = new SelectList(_context.TypesChambres, "Id", "Libelle", reservation.TypeChambreId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Chambre)
                .Include(r => r.Client)
                .Include(r => r.TypeChambre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}

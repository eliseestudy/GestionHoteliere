using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Services;
using System.Data;

namespace Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;
        private readonly IDocumentNumberService _documentNumbers;

        public ReservationsController(
            GestionHoteliereDbContext context,
            IBusinessRulesService rules,
            IDocumentNumberService documentNumbers)
        {
            _context = context;
            _rules = rules;
            _documentNumbers = documentNumbers;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Chambre)
                .Include(r => r.Client)
                .Include(r => r.TypeChambre)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();

            var sejourReservationIds = await _context.Sejours
                .Where(s => s.ReservationId.HasValue)
                .Select(s => s.ReservationId!.Value)
                .ToListAsync();

            var model = reservations.Select(reservation =>
            {
                var hasStay = sejourReservationIds.Contains(reservation.Id);
                return new ReservationListItemViewModel
                {
                    Reservation = reservation,
                    CanConfirm = _rules.CanConfirmReservation(reservation),
                    CanCancel = _rules.CanCancelReservation(reservation),
                    CanNoShow = _rules.CanNoShowReservation(reservation),
                    CanCheckIn = _rules.CanCheckInReservation(reservation, hasStay),
                    CanDelete = _rules.CanDeleteReservation(reservation, hasStay)
                };
            }).ToList();

            return View(model);
        }

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

        public IActionResult Create()
        {
            PopulateSelectLists();
            var today = DateTimeOffset.UtcNow;
            return View(new ReservationFormViewModel
            {
                DateArrivee = today,
                DateDepart = today.AddDays(1),
                NombreNuits = 1
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationFormViewModel model)
        {
            await CalculateReservationAsync(model);

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                var now = DateTimeOffset.UtcNow;
                var reservation = new Reservation
                {
                    NumeroReservation = await _documentNumbers.NextReservationNumberAsync(now),
                    ClientId = model.ClientId,
                    ChambreId = model.ChambreId,
                    TypeChambreId = model.TypeChambreId,
                    DateCreation = now,
                    DateArrivee = model.DateArrivee,
                    DateDepart = model.DateDepart,
                    NombrePersonnes = model.NombrePersonnes,
                    MontantEstime = model.MontantEstime,
                    Statut = ReservationStatut.EnAttente,
                    CreatedAt = now
                };

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                SetToast("success", "Reservation creee en attente.");
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(model);
            return View(model);
        }

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

            if (reservation.Statut == ReservationStatut.Termine ||
                reservation.Statut == ReservationStatut.Annulee ||
                reservation.Statut == ReservationStatut.NoShow)
            {
                SetToast("warning", "Cette reservation est terminale et ne peut plus etre modifiee.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new ReservationFormViewModel
            {
                Id = reservation.Id,
                NumeroReservation = reservation.NumeroReservation,
                ClientId = reservation.ClientId,
                ChambreId = reservation.ChambreId,
                TypeChambreId = reservation.TypeChambreId,
                DateArrivee = reservation.DateArrivee,
                DateDepart = reservation.DateDepart,
                NombrePersonnes = reservation.NombrePersonnes,
                NombreNuits = Math.Max(1, (int)Math.Ceiling((reservation.DateDepart.Date - reservation.DateArrivee.Date).TotalDays)),
                MontantEstime = reservation.MontantEstime
            };
            model.PrixParNuit = model.NombreNuits == 0 ? 0 : model.MontantEstime / model.NombreNuits;
            PopulateSelectLists(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            await CalculateReservationAsync(model);

            if (ModelState.IsValid)
            {
                var existingReservation = await _context.Reservations.FindAsync(id);
                if (existingReservation == null)
                {
                    return NotFound();
                }

                if (existingReservation.Statut == ReservationStatut.Termine ||
                    existingReservation.Statut == ReservationStatut.Annulee ||
                    existingReservation.Statut == ReservationStatut.NoShow)
                {
                    SetToast("warning", "Cette reservation est terminale et ne peut plus etre modifiee.");
                    return RedirectToAction(nameof(Details), new { id });
                }

                existingReservation.ClientId = model.ClientId;
                existingReservation.ChambreId = model.ChambreId;
                existingReservation.TypeChambreId = model.TypeChambreId;
                existingReservation.DateArrivee = model.DateArrivee;
                existingReservation.DateDepart = model.DateDepart;
                existingReservation.NombrePersonnes = model.NombrePersonnes;
                existingReservation.MontantEstime = model.MontantEstime;
                existingReservation.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();
                SetToast("success", "Reservation mise a jour.");
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(model);
            return View(model);
        }

        public async Task<IActionResult> CheckIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Chambre)
                .Include(r => r.TypeChambre)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var hasStay = await _context.Sejours.AnyAsync(s => s.ReservationId == reservation.Id);
            if (!_rules.CanCheckInReservation(reservation, hasStay))
            {
                SetToast("warning", "Le check-in n'est pas disponible pour cette reservation.");
                return RedirectToAction(nameof(Index));
            }

            PopulateAvailableRooms(reservation.ChambreId, reservation.TypeChambreId);
            return View(new ReservationCheckInViewModel
            {
                ReservationId = reservation.Id,
                NumeroReservation = reservation.NumeroReservation,
                Client = reservation.Client == null ? string.Empty : $"{reservation.Client.Prenom} {reservation.Client.Nom}",
                ChambreId = reservation.ChambreId ?? 0,
                DateEntree = DateTimeOffset.UtcNow,
                TarifApplique = reservation.TypeChambre?.PrixParNuit ?? reservation.MontantEstime,
                Remise = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id, ReservationCheckInViewModel model)
        {
            if (id != model.ReservationId)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.TypeChambre)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var hasStay = await _context.Sejours.AnyAsync(s => s.ReservationId == reservation.Id);
            if (!_rules.CanCheckInReservation(reservation, hasStay))
            {
                SetToast("warning", "Le check-in n'est pas disponible pour cette reservation.");
                return RedirectToAction(nameof(Index));
            }

            var chambre = await _context.Chambres.FindAsync(model.ChambreId);
            if (chambre == null || chambre.Statut != ChambreStatut.Libre && chambre.Id != reservation.ChambreId)
            {
                ModelState.AddModelError(nameof(model.ChambreId), "Selectionnez une chambre libre.");
            }

            if (!ModelState.IsValid)
            {
                PopulateAvailableRooms(reservation.ChambreId, reservation.TypeChambreId);
                return View(model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            reservation.Statut = ReservationStatut.ArriveeEnCours;
            reservation.ChambreId = chambre!.Id;
            reservation.UpdatedAt = DateTimeOffset.UtcNow;

            chambre.Statut = ChambreStatut.Occupee;
            chambre.UpdatedAt = DateTimeOffset.UtcNow;

            _context.Sejours.Add(new Sejour
            {
                ReservationId = reservation.Id,
                ClientId = reservation.ClientId,
                ChambreId = chambre.Id,
                DateEntree = model.DateEntree == default ? DateTimeOffset.UtcNow : model.DateEntree,
                NbNuits = Math.Max(1, (int)Math.Ceiling((reservation.DateDepart.Date - reservation.DateArrivee.Date).TotalDays)),
                TarifApplique = model.TarifApplique,
                Remise = model.Remise,
                Statut = SejourStatut.EnCours,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SetToast("success", "Check-in effectue. Le sejour est ouvert et la chambre est occupee.");
            return RedirectToAction("Index", "Sejours");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            return await TransitionReservation(id, ReservationStatut.Confirmee, _rules.CanConfirmReservation, "Reservation confirmee.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            return await TransitionReservation(id, ReservationStatut.Annulee, _rules.CanCancelReservation, "Reservation annulee.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NoShow(int id)
        {
            return await TransitionReservation(id, ReservationStatut.NoShow, _rules.CanNoShowReservation, "Reservation marquee absente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var hasStay = await _context.Sejours.AnyAsync(s => s.ReservationId == id);
            if (!_rules.CanDeleteReservation(reservation, hasStay))
            {
                SetToast("warning", "Seule une reservation en attente sans sejour peut etre supprimee.");
                return RedirectToAction(nameof(Index));
            }

            reservation.IsDeleted = true;
            reservation.DeletedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            SetToast("success", "Reservation supprimee.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> TransitionReservation(int id, ReservationStatut next, Func<Reservation, bool> canApply, string message)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (!canApply(reservation))
            {
                SetToast("warning", "Action indisponible pour le statut actuel.");
                return RedirectToAction(nameof(Index));
            }

            reservation.Statut = next;
            reservation.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            SetToast("success", message);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists(ReservationFormViewModel? reservation = null)
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", reservation?.ClientId);
            ViewData["ChambreOptions"] = _context.Chambres
                .OrderBy(c => c.Numero)
                .Select(c => new ChambreOptionViewModel
                {
                    Id = c.Id,
                    Numero = c.Numero,
                    TypeChambreId = c.TypeChambreId
                })
                .ToList();
            ViewData["TypeChambreOptions"] = _context.TypesChambres
                .OrderBy(t => t.Libelle)
                .Select(t => new TypeChambreOptionViewModel
                {
                    Id = t.Id,
                    Libelle = t.Libelle,
                    PrixParNuit = t.PrixParNuit,
                    Capacite = t.Capacite
                })
                .ToList();
        }

        private void PopulateAvailableRooms(int? selectedRoomId, int? typeChambreId)
        {
            var rooms = _context.Chambres
                .Where(c => c.Statut == ChambreStatut.Libre || c.Id == selectedRoomId);

            if (typeChambreId.HasValue)
            {
                rooms = rooms.Where(c => c.TypeChambreId == typeChambreId.Value || c.Id == selectedRoomId);
            }

            ViewData["ChambreId"] = new SelectList(rooms.OrderBy(c => c.Numero), "Id", "Numero", selectedRoomId);
        }

        private async Task CalculateReservationAsync(ReservationFormViewModel model)
        {
            if (model.DateDepart.Date <= model.DateArrivee.Date)
            {
                return;
            }

            if (model.ChambreId.HasValue)
            {
                var roomTypeId = await _context.Chambres
                    .Where(c => c.Id == model.ChambreId.Value)
                    .Select(c => (int?)c.TypeChambreId)
                    .SingleOrDefaultAsync();
                if (!roomTypeId.HasValue)
                {
                    ModelState.AddModelError(nameof(model.ChambreId), "La chambre sélectionnée est introuvable.");
                    return;
                }

                model.TypeChambreId = roomTypeId;
                ModelState.Remove(nameof(model.TypeChambreId));
            }

            if (!model.TypeChambreId.HasValue)
            {
                return;
            }

            var price = await _context.TypesChambres
                .Where(t => t.Id == model.TypeChambreId.Value)
                .Select(t => (decimal?)t.PrixParNuit)
                .SingleOrDefaultAsync();
            if (!price.HasValue)
            {
                ModelState.AddModelError(nameof(model.TypeChambreId), "Le type de chambre sélectionné est introuvable.");
                return;
            }

            var calculation = ReservationPricingCalculator.Calculate(model.DateArrivee, model.DateDepart, price.Value);
            model.NombreNuits = calculation.Nights;
            model.PrixParNuit = calculation.PricePerNight;
            model.MontantEstime = calculation.Total;
        }

        private void SetToast(string type, string message)
        {
            TempData["Toast.Type"] = type;
            TempData["Toast.Message"] = message;
        }
    }
}

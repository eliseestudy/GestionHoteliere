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
    public class SejoursController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;
        private readonly IDocumentNumberService _documentNumbers;

        public SejoursController(
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
            var sejours = await _context.Sejours
                .Include(s => s.Chambre)
                .Include(s => s.Client)
                .Include(s => s.Reservation)
                .OrderByDescending(s => s.DateEntree)
                .ToListAsync();

            var sejourIds = sejours.Select(s => s.Id).ToList();
            var factureSejourIds = await _context.Factures
                .Where(f => f.SejourId.HasValue && sejourIds.Contains(f.SejourId.Value))
                .Select(f => f.SejourId!.Value)
                .ToListAsync();
            var paiementSejourIds = await _context.Paiements
                .Include(p => p.Facture)
                .Where(p => p.Facture != null && p.Facture.SejourId.HasValue && sejourIds.Contains(p.Facture.SejourId.Value))
                .Select(p => p.Facture!.SejourId!.Value)
                .ToListAsync();

            var model = sejours.Select(sejour =>
            {
                var hasInvoice = factureSejourIds.Contains(sejour.Id);
                var hasPayment = paiementSejourIds.Contains(sejour.Id);
                return new SejourListItemViewModel
                {
                    Sejour = sejour,
                    CanCheckout = _rules.CanCheckoutSejour(sejour, hasInvoice),
                    CanCancel = _rules.CanCancelSejour(sejour, hasInvoice, hasPayment)
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

        public IActionResult Create()
        {
            PopulateSelectLists();
            return View(new Sejour
            {
                DateEntree = DateTimeOffset.UtcNow,
                Statut = SejourStatut.EnCours
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,ClientId,ChambreId,DateEntree,DateSortie,NbNuits,TarifApplique,Remise")] Sejour sejour)
        {
            ValidateSejourDates(sejour);

            var chambre = await _context.Chambres.FindAsync(sejour.ChambreId);
            if (chambre == null || chambre.Statut != ChambreStatut.Libre)
            {
                ModelState.AddModelError(nameof(Sejour.ChambreId), "Selectionnez une chambre libre pour une arrivee sans reservation.");
            }

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                sejour.ReservationId = null;
                sejour.Statut = SejourStatut.EnCours;
                sejour.DateEntree = sejour.DateEntree == default ? DateTimeOffset.UtcNow : sejour.DateEntree;
                sejour.CreatedAt = DateTimeOffset.UtcNow;

                chambre!.Statut = ChambreStatut.Occupee;
                chambre.UpdatedAt = DateTimeOffset.UtcNow;

                _context.Add(sejour);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                SetToast("success", "Arrivee sans reservation enregistree.");
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(sejour);
            return View(sejour);
        }

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

            if (sejour.Statut != SejourStatut.EnCours)
            {
                SetToast("warning", "Seul un sejour en cours peut etre modifie.");
                return RedirectToAction(nameof(Details), new { id });
            }

            PopulateSelectLists(sejour);
            return View(sejour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,ClientId,ChambreId,DateEntree,DateSortie,NbNuits,TarifApplique,Remise,Id")] Sejour sejour)
        {
            if (id != sejour.Id)
            {
                return NotFound();
            }

            ValidateSejourDates(sejour);

            if (ModelState.IsValid)
            {
                var existingSejour = await _context.Sejours.FindAsync(id);
                if (existingSejour == null)
                {
                    return NotFound();
                }

                if (existingSejour.Statut != SejourStatut.EnCours)
                {
                    SetToast("warning", "Seul un sejour en cours peut etre modifie.");
                    return RedirectToAction(nameof(Details), new { id });
                }

                existingSejour.ReservationId = sejour.ReservationId;
                existingSejour.ClientId = sejour.ClientId;
                existingSejour.ChambreId = sejour.ChambreId;
                existingSejour.DateEntree = sejour.DateEntree;
                existingSejour.DateSortie = sejour.DateSortie;
                existingSejour.NbNuits = sejour.NbNuits;
                existingSejour.TarifApplique = sejour.TarifApplique;
                existingSejour.Remise = sejour.Remise;
                existingSejour.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();
                SetToast("success", "Sejour mis a jour.");
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(sejour);
            return View(sejour);
        }

        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sejour = await _context.Sejours
                .Include(s => s.Client)
                .Include(s => s.Chambre)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sejour == null)
            {
                return NotFound();
            }

            var hasInvoice = await _context.Factures.AnyAsync(f => f.SejourId == sejour.Id);
            if (!_rules.CanCheckoutSejour(sejour, hasInvoice))
            {
                SetToast("warning", "Le check-out n'est pas disponible pour ce sejour.");
                return RedirectToAction(nameof(Index));
            }

            var dateSortie = DateTimeOffset.UtcNow;
            var nights = Math.Max(1, (int)Math.Ceiling((dateSortie.Date - sejour.DateEntree.Date).TotalDays));
            return View(new SejourCheckoutViewModel
            {
                SejourId = sejour.Id,
                Client = sejour.Client == null ? string.Empty : $"{sejour.Client.Prenom} {sejour.Client.Nom}",
                Chambre = sejour.Chambre?.Numero ?? string.Empty,
                DateSortie = dateSortie,
                NbNuits = nights,
                TarifApplique = sejour.TarifApplique,
                Remise = sejour.Remise
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int id, SejourCheckoutViewModel model)
        {
            if (id != model.SejourId)
            {
                return NotFound();
            }

            var sejour = await _context.Sejours
                .Include(s => s.Reservation)
                .Include(s => s.Chambre)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sejour == null)
            {
                return NotFound();
            }

            var hasInvoice = await _context.Factures.AnyAsync(f => f.SejourId == sejour.Id);
            if (!_rules.CanCheckoutSejour(sejour, hasInvoice))
            {
                SetToast("warning", "Le check-out n'est pas disponible pour ce sejour.");
                return RedirectToAction(nameof(Index));
            }

            if (model.DateSortie.Date < sejour.DateEntree.Date)
            {
                ModelState.AddModelError(nameof(model.DateSortie), "La date de sortie doit etre posterieure a l'entree.");
            }

            if (!ModelState.IsValid)
            {
                model.Client = sejour.Client == null ? string.Empty : $"{sejour.Client.Prenom} {sejour.Client.Nom}";
                model.Chambre = sejour.Chambre?.Numero ?? string.Empty;
                return View(model);
            }

            model.NbNuits = Math.Max(1, (int)Math.Ceiling((model.DateSortie.Date - sejour.DateEntree.Date).TotalDays));
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var now = DateTimeOffset.UtcNow;

            sejour.DateSortie = model.DateSortie;
            sejour.NbNuits = model.NbNuits;
            sejour.TarifApplique = model.TarifApplique;
            sejour.Remise = model.Remise;
            sejour.Statut = SejourStatut.Termine;
            sejour.UpdatedAt = DateTimeOffset.UtcNow;

            if (sejour.Reservation != null)
            {
                sejour.Reservation.Statut = ReservationStatut.Termine;
                sejour.Reservation.UpdatedAt = DateTimeOffset.UtcNow;
            }

            if (sejour.Chambre != null)
            {
                sejour.Chambre.Statut = ChambreStatut.Nettoyage;
                sejour.Chambre.UpdatedAt = DateTimeOffset.UtcNow;
            }

            var facture = new Facture
            {
                NumeroFacture = await _documentNumbers.NextInvoiceNumberAsync(now),
                SejourId = sejour.Id,
                ClientId = sejour.ClientId,
                DateEmission = now,
                MontantHT = model.MontantHT,
                Taxe = model.Taxe,
                MontantTTC = model.MontantTTC,
                MontantPaye = 0,
                Statut = FactureStatut.Brouillon,
                CreatedAt = now
            };
            _context.Factures.Add(facture);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SetToast("success", "Check-out effectue. Une facture brouillon a ete creee.");
            return RedirectToAction("Details", "Factures", new { id = facture.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var sejour = await _context.Sejours
                .Include(s => s.Reservation)
                .Include(s => s.Chambre)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sejour == null)
            {
                return NotFound();
            }

            var hasInvoice = await _context.Factures.AnyAsync(f => f.SejourId == id);
            var hasPayment = await _context.Paiements.AnyAsync(p => p.Facture != null && p.Facture.SejourId == id);
            if (!_rules.CanCancelSejour(sejour, hasInvoice, hasPayment))
            {
                SetToast("warning", "Ce sejour ne peut pas etre annule car il possede deja une suite operationnelle.");
                return RedirectToAction(nameof(Index));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            sejour.Statut = SejourStatut.Annule;
            sejour.UpdatedAt = DateTimeOffset.UtcNow;

            if (sejour.Reservation != null)
            {
                sejour.Reservation.Statut = ReservationStatut.Confirmee;
                sejour.Reservation.UpdatedAt = DateTimeOffset.UtcNow;
            }

            if (sejour.Chambre != null)
            {
                sejour.Chambre.Statut = ChambreStatut.Libre;
                sejour.Chambre.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            SetToast("success", "Sejour annule et contexte restaure.");
            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists(Sejour? sejour = null)
        {
            var selectedChambreId = sejour?.ChambreId;
            ViewData["ChambreId"] = new SelectList(_context.Chambres.Where(c => c.Statut == ChambreStatut.Libre || c.Id == selectedChambreId), "Id", "Numero", selectedChambreId);
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", sejour?.ClientId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations.Where(r => r.Statut == ReservationStatut.Confirmee), "Id", "NumeroReservation", sejour?.ReservationId);
        }

        private void ValidateSejourDates(Sejour sejour)
        {
            if (sejour.DateSortie.HasValue && sejour.DateSortie.Value.Date < sejour.DateEntree.Date)
            {
                ModelState.AddModelError(nameof(Sejour.DateSortie), "La date de sortie doit etre posterieure ou egale a la date d'entree.");
            }
        }

        private void SetToast(string type, string message)
        {
            TempData["Toast.Type"] = type;
            TempData["Toast.Message"] = message;
        }
    }
}

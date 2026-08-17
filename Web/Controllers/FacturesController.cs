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
    public class FacturesController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;
        private readonly IDocumentNumberService _documentNumbers;

        public FacturesController(
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
            var factures = await _context.Factures
                .Include(f => f.Client)
                .Include(f => f.Sejour)
                .Include(f => f.Paiements)
                .OrderByDescending(f => f.DateEmission)
                .ToListAsync();

            foreach (var facture in factures.Where(f => f.Statut != FactureStatut.Brouillon && f.Statut != FactureStatut.Annulee && f.Statut != FactureStatut.EnLitige))
            {
                facture.MontantPaye = facture.Paiements.Where(p => p.Statut == PaiementStatut.Effectue).Sum(p => p.Montant);
                facture.Statut = facture.MontantPaye <= 0
                    ? FactureStatut.Emise
                    : facture.MontantPaye >= facture.MontantTTC
                        ? FactureStatut.Payee
                        : FactureStatut.PartiellementPayee;
            }

            await _context.SaveChangesAsync();

            var model = factures.Select(facture => new FactureListItemViewModel
            {
                Facture = facture,
                Solde = Math.Max(0, facture.MontantTTC - facture.MontantPaye),
                CanEdit = _rules.CanEditFacture(facture),
                CanDelete = _rules.CanDeleteFacture(facture),
                CanEmit = _rules.CanEmitFacture(facture),
                CanDispute = _rules.CanDisputeFacture(facture),
                CanResolveDispute = _rules.CanResolveDispute(facture),
                CanCancel = _rules.CanCancelFacture(facture),
                CanPay = _rules.CanPayFacture(facture)
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Factures
                .Include(f => f.Client)
                .Include(f => f.Sejour)
                .Include(f => f.Paiements)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }

            return View(facture);
        }

        public IActionResult Create()
        {
            PopulateSelectLists();
            return View(new FactureFormViewModel
            {
                DateEmission = DateTimeOffset.UtcNow
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FactureFormViewModel model)
        {
            ValidateFactureDates(model);
            model.MontantTTC = Math.Max(0, model.MontantHT + model.Taxe);
            ModelState.Remove(nameof(model.MontantTTC));

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                var now = DateTimeOffset.UtcNow;
                var facture = new Facture
                {
                    NumeroFacture = await _documentNumbers.NextInvoiceNumberAsync(now),
                    ClientId = model.ClientId,
                    DateEmission = model.DateEmission,
                    DateEcheance = model.DateEcheance,
                    MontantHT = model.MontantHT,
                    Taxe = model.Taxe,
                    MontantTTC = model.MontantTTC,
                    MontantPaye = 0,
                    Statut = FactureStatut.Brouillon,
                    CreatedAt = now
                };

                _context.Add(facture);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                SetToast("success", "Facture hors sejour creee en brouillon.");
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

            var facture = await _context.Factures
                .Include(f => f.Sejour)
                .ThenInclude(s => s!.Chambre)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (facture == null)
            {
                return NotFound();
            }

            if (!_rules.CanEditFacture(facture))
            {
                SetToast("warning", "Une facture emise ne peut plus etre modifiee.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new FactureFormViewModel
            {
                Id = facture.Id,
                NumeroFacture = facture.NumeroFacture,
                SejourId = facture.SejourId,
                SejourLabel = facture.Sejour == null
                    ? null
                    : $"Séjour #{facture.Sejour.Id} - Chambre {facture.Sejour.Chambre?.Numero}",
                ClientId = facture.ClientId,
                DateEmission = facture.DateEmission,
                DateEcheance = facture.DateEcheance,
                MontantHT = facture.MontantHT,
                Taxe = facture.Taxe,
                MontantTTC = facture.MontantTTC
            };
            PopulateSelectLists(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FactureFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            ValidateFactureDates(model);
            model.MontantTTC = Math.Max(0, model.MontantHT + model.Taxe);
            ModelState.Remove(nameof(model.MontantTTC));

            if (ModelState.IsValid)
            {
                var existingFacture = await _context.Factures.FindAsync(id);
                if (existingFacture == null)
                {
                    return NotFound();
                }

                if (!_rules.CanEditFacture(existingFacture))
                {
                    SetToast("warning", "Une facture emise ne peut plus etre modifiee.");
                    return RedirectToAction(nameof(Details), new { id });
                }

                existingFacture.ClientId = model.ClientId;
                existingFacture.DateEmission = model.DateEmission;
                existingFacture.DateEcheance = model.DateEcheance;
                existingFacture.MontantHT = model.MontantHT;
                existingFacture.Taxe = model.Taxe;
                existingFacture.MontantTTC = model.MontantTTC;
                existingFacture.MontantPaye = 0;
                existingFacture.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();
                SetToast("success", "Facture brouillon mise a jour.");
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emit(int id)
        {
            return await ApplyStatus(id, FactureStatut.Emise, _rules.CanEmitFacture, "Facture emise.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispute(int id)
        {
            return await ApplyStatus(id, FactureStatut.EnLitige, _rules.CanDisputeFacture, "Facture mise en litige.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveDispute(int id)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
            {
                return NotFound();
            }

            if (!_rules.CanResolveDispute(facture))
            {
                SetToast("warning", "Cette facture n'est pas en litige.");
                return RedirectToAction(nameof(Index));
            }

            facture.Statut = FactureStatut.Emise;
            facture.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            await _rules.RecalculateFactureAsync(facture.Id);
            await _context.SaveChangesAsync();

            SetToast("success", "Litige resolu.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            return await ApplyStatus(id, FactureStatut.Annulee, _rules.CanCancelFacture, "Facture annulee.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
            {
                return NotFound();
            }

            if (!_rules.CanDeleteFacture(facture))
            {
                SetToast("warning", "Seule une facture brouillon peut etre supprimee.");
                return RedirectToAction(nameof(Index));
            }

            facture.IsDeleted = true;
            facture.DeletedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            SetToast("success", "Facture supprimee.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ApplyStatus(int id, FactureStatut next, Func<Facture, bool> canApply, string message)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
            {
                return NotFound();
            }

            if (!canApply(facture))
            {
                SetToast("warning", "Action indisponible pour cette facture.");
                return RedirectToAction(nameof(Index));
            }

            facture.Statut = next;
            facture.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            SetToast("success", message);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists(FactureFormViewModel? facture = null)
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Nom", facture?.ClientId);
        }

        private void ValidateFactureDates(FactureFormViewModel facture)
        {
            if (facture.DateEcheance.HasValue && facture.DateEcheance.Value.Date < facture.DateEmission.Date)
            {
                ModelState.AddModelError(nameof(Facture.DateEcheance), "La date d'echeance doit etre posterieure ou egale a la date d'emission.");
            }
        }

        private void SetToast(string type, string message)
        {
            TempData["Toast.Type"] = type;
            TempData["Toast.Message"] = message;
        }
    }
}

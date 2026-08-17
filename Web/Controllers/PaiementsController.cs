using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class PaiementsController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;

        public PaiementsController(GestionHoteliereDbContext context, IBusinessRulesService rules)
        {
            _context = context;
            _rules = rules;
        }

        public async Task<IActionResult> Index()
        {
            var paiements = await _context.Paiements
                .Include(p => p.Facture)
                .Include(p => p.User)
                .OrderByDescending(p => p.DatePaiement)
                .ToListAsync();

            var model = paiements.Select(paiement => new PaiementListItemViewModel
            {
                Paiement = paiement,
                CanRefund = _rules.CanRefundPaiement(paiement)
            }).ToList();

            return View(model);
        }

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

        [HttpGet]
        public async Task<IActionResult> CreateForFacture(int factureId)
        {
            var model = await BuildPaymentModel(factureId);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_PaymentModalForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForFacture(PaiementContextViewModel model)
        {
            var facture = await _context.Factures
                .Include(f => f.Paiements)
                .FirstOrDefaultAsync(f => f.Id == model.FactureId);
            if (facture == null)
            {
                return NotFound();
            }

            await _rules.RecalculateFactureAsync(facture.Id);
            await _context.SaveChangesAsync();
            await _context.Entry(facture).ReloadAsync();

            var solde = Math.Max(0, facture.MontantTTC - facture.MontantPaye);
            model.NumeroFacture = facture.NumeroFacture;
            model.SoldeRestant = solde;

            if (!_rules.CanPayFacture(facture))
            {
                ModelState.AddModelError(string.Empty, "Cette facture ne peut pas recevoir de paiement.");
            }

            if (model.Montant <= 0)
            {
                ModelState.AddModelError(nameof(model.Montant), "Le montant doit etre positif.");
            }

            if (model.Montant > solde)
            {
                ModelState.AddModelError(nameof(model.Montant), "Le montant ne peut pas depasser le solde restant.");
            }

            if (!ModelState.IsValid)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return PartialView("_PaymentModalForm", model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.Paiements.Add(new Paiement
            {
                FactureId = facture.Id,
                DatePaiement = model.DatePaiement == default ? DateTimeOffset.UtcNow : model.DatePaiement,
                Montant = model.Montant,
                Mode = model.Mode,
                Statut = PaiementStatut.Effectue,
                TransactionReference = model.TransactionReference,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _context.SaveChangesAsync();
            await _rules.RecalculateFactureAsync(facture.Id);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Toast.Type"] = "success";
            TempData["Toast.Message"] = "Paiement enregistre.";
            return Json(new { success = true, message = "Paiement enregistre." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(int id)
        {
            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement == null)
            {
                return NotFound();
            }

            if (!_rules.CanRefundPaiement(paiement))
            {
                SetToast("warning", "Seul un paiement effectue peut etre rembourse.");
                return RedirectToAction(nameof(Index));
            }

            paiement.Statut = PaiementStatut.Rembourse;
            paiement.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            await _rules.RecalculateFactureAsync(paiement.FactureId);
            await _context.SaveChangesAsync();

            SetToast("success", "Paiement rembourse integralement.");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            SetToast("warning", "Les paiements se creent depuis une facture eligible.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SetToast("warning", "Un paiement enregistre est consultable mais non modifiable.");
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SetToast("warning", "Les paiements ne sont pas supprimables. Utilisez le remboursement total.");
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<PaiementContextViewModel?> BuildPaymentModel(int factureId)
        {
            var facture = await _context.Factures
                .Include(f => f.Paiements)
                .FirstOrDefaultAsync(f => f.Id == factureId);
            if (facture == null)
            {
                return null;
            }

            var solde = Math.Max(0, facture.MontantTTC - facture.MontantPaye);
            return new PaiementContextViewModel
            {
                FactureId = facture.Id,
                NumeroFacture = facture.NumeroFacture,
                SoldeRestant = solde,
                Montant = solde,
                DatePaiement = DateTimeOffset.UtcNow
            };
        }

        private void SetToast(string type, string message)
        {
            TempData["Toast.Type"] = type;
            TempData["Toast.Message"] = message;
        }
    }
}

using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class ChambresController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;

        public ChambresController(GestionHoteliereDbContext context, IBusinessRulesService rules)
        {
            _context = context;
            _rules = rules;
        }

        public async Task<IActionResult> Index()
        {
            var chambres = await _context.Chambres
                .Include(c => c.TypeChambre)
                .OrderBy(c => c.Numero)
                .ToListAsync();

            var model = new List<ChambreListItemViewModel>();
            foreach (var chambre in chambres)
            {
                model.Add(new ChambreListItemViewModel
                {
                    Chambre = chambre,
                    CanSetCleaning = _rules.CanChangeRoomState(chambre) && chambre.Statut != ChambreStatut.Nettoyage,
                    CanSetMaintenance = _rules.CanChangeRoomState(chambre) && chambre.Statut != ChambreStatut.Maintenance,
                    CanSetOutOfService = _rules.CanChangeRoomState(chambre) && chambre.Statut != ChambreStatut.HorsService,
                    CanSetReady = _rules.CanMarkRoomReady(chambre),
                    CanDelete = await _rules.CanDeleteChambreAsync(chambre.Id)
                });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambres
                .Include(c => c.TypeChambre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        public IActionResult Create()
        {
            PopulateTypeOptions();
            return View(new ChambreFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChambreFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var chambre = new Chambre
                {
                    Numero = model.Numero,
                    Etage = model.Etage,
                    TypeChambreId = model.TypeChambreId,
                    NbLits = model.NbLits,
                    Observations = model.Observations,
                    Statut = ChambreStatut.Libre,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _context.Add(chambre);
                await _context.SaveChangesAsync();
                SetToast("success", "Chambre creee et marquee libre.");
                return RedirectToAction(nameof(Index));
            }

            PopulateTypeOptions();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambres.FindAsync(id);
            if (chambre == null)
            {
                return NotFound();
            }

            PopulateTypeOptions();
            return View(new ChambreFormViewModel
            {
                Id = chambre.Id,
                Numero = chambre.Numero,
                Etage = chambre.Etage,
                TypeChambreId = chambre.TypeChambreId,
                NbLits = chambre.NbLits,
                Observations = chambre.Observations
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChambreFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingChambre = await _context.Chambres.FindAsync(id);
                if (existingChambre == null)
                {
                    return NotFound();
                }

                existingChambre.Numero = model.Numero;
                existingChambre.Etage = model.Etage;
                existingChambre.TypeChambreId = model.TypeChambreId;
                existingChambre.NbLits = model.NbLits;
                existingChambre.Observations = model.Observations;
                existingChambre.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();
                SetToast("success", "Chambre mise a jour.");
                return RedirectToAction(nameof(Index));
            }

            PopulateTypeOptions();
            return View(model);
        }

        private void PopulateTypeOptions()
        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetState(int id, ChambreStatut statut)
        {
            var chambre = await _context.Chambres.FindAsync(id);
            if (chambre == null)
            {
                return NotFound();
            }

            if (statut == ChambreStatut.Libre)
            {
                if (!_rules.CanMarkRoomReady(chambre))
                {
                    SetToast("warning", "Cette chambre ne peut pas etre marquee prete.");
                    return RedirectToAction(nameof(Index));
                }

                var hasReservation = await _context.Reservations.AnyAsync(r =>
                    r.ChambreId == id &&
                    r.Statut == ReservationStatut.Confirmee &&
                    r.DateDepart >= DateTimeOffset.UtcNow);
                chambre.Statut = hasReservation ? ChambreStatut.Reservee : ChambreStatut.Libre;
            }
            else
            {
                if ((statut == ChambreStatut.Maintenance || statut == ChambreStatut.HorsService || statut == ChambreStatut.Nettoyage) &&
                    !_rules.CanChangeRoomState(chambre))
                {
                    SetToast("warning", "Une chambre occupee ou reservee ne peut pas changer vers cet etat.");
                    return RedirectToAction(nameof(Index));
                }

                if (statut != ChambreStatut.Maintenance && statut != ChambreStatut.HorsService && statut != ChambreStatut.Nettoyage)
                {
                    SetToast("warning", "Etat de chambre non autorise depuis cette action.");
                    return RedirectToAction(nameof(Index));
                }

                chambre.Statut = statut;
            }

            chambre.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            SetToast("success", "Etat de chambre mis a jour.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chambre = await _context.Chambres.FindAsync(id);
            if (chambre == null)
            {
                return NotFound();
            }

            if (!await _rules.CanDeleteChambreAsync(id))
            {
                SetToast("warning", "Cette chambre est occupee, reservee ou referencee par l'historique.");
                return RedirectToAction(nameof(Index));
            }

            chambre.IsDeleted = true;
            chambre.DeletedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            SetToast("success", "Chambre supprimee.");
            return RedirectToAction(nameof(Index));
        }

        private void SetToast(string type, string message)
        {
            TempData["Toast.Type"] = type;
            TempData["Toast.Message"] = message;
        }
    }
}

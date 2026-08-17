using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Infrastructure.Data;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(GestionHoteliereDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var maintenant = DateTimeOffset.UtcNow;
            var aujourdHui = new DateTimeOffset(maintenant.Year, maintenant.Month, maintenant.Day, 0, 0, 0, TimeSpan.Zero);
            var demain = aujourdHui.AddDays(1);
            var debutMois = new DateTimeOffset(maintenant.Year, maintenant.Month, 1, 0, 0, 0, TimeSpan.Zero);

            var totalChambres = await _context.Chambres.CountAsync();
            var chambresLibres = await _context.Chambres.CountAsync(c => c.Statut == ChambreStatut.Libre);
            var chambresOccupees = await _context.Chambres.CountAsync(c => c.Statut == ChambreStatut.Occupee);
            var chambresMaintenance = await _context.Chambres.CountAsync(c =>
                c.Statut == ChambreStatut.Maintenance || c.Statut == ChambreStatut.HorsService);

            var reservationsActives = await _context.Reservations.CountAsync(r =>
                r.Statut == ReservationStatut.EnAttente ||
                r.Statut == ReservationStatut.Confirmee ||
                r.Statut == ReservationStatut.ArriveeEnCours);

            var arriveesDuJour = await _context.Reservations.CountAsync(r =>
                r.DateArrivee >= aujourdHui && r.DateArrivee < demain &&
                r.Statut != ReservationStatut.Annulee &&
                r.Statut != ReservationStatut.NoShow);

            var departsDuJour = await _context.Reservations.CountAsync(r =>
                r.DateDepart >= aujourdHui && r.DateDepart < demain &&
                r.Statut != ReservationStatut.Annulee &&
                r.Statut != ReservationStatut.NoShow);

            var revenusDuMois = await _context.Paiements
                .Where(p => p.DatePaiement >= debutMois && p.Statut == PaiementStatut.Effectue)
                .SumAsync(p => p.Montant);

            var facturesEnAttente = await _context.Factures.CountAsync(f =>
                f.Statut == FactureStatut.Emise ||
                f.Statut == FactureStatut.PartiellementPayee ||
                f.MontantPaye < f.MontantTTC);

            var statutsChambres = await _context.Chambres
                .GroupBy(c => c.Statut)
                .Select(g => new
                {
                    Statut = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            var reservationsRecentes = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Chambre)
                .Include(r => r.TypeChambre)
                .OrderByDescending(r => r.DateCreation)
                .Take(5)
                .Select(r => new RecentReservationDashboardItem
                {
                    Id = r.Id,
                    Numero = r.NumeroReservation,
                    Client = r.Client == null ? "Client non renseigne" : r.Client.Prenom + " " + r.Client.Nom,
                    Chambre = r.Chambre != null ? "Chambre " + r.Chambre.Numero : r.TypeChambre != null ? r.TypeChambre.Libelle : "A assigner",
                    DateArrivee = r.DateArrivee,
                    DateDepart = r.DateDepart,
                    Statut = r.Statut,
                    MontantEstime = r.MontantEstime
                })
                .ToListAsync();

            var model = new HomeDashboardViewModel
            {
                TotalChambres = totalChambres,
                ChambresLibres = chambresLibres,
                ChambresOccupees = chambresOccupees,
                ChambresMaintenance = chambresMaintenance,
                ReservationsActives = reservationsActives,
                ArriveesDuJour = arriveesDuJour,
                DepartsDuJour = departsDuJour,
                ClientsTotal = await _context.Clients.CountAsync(),
                SejoursEnCours = await _context.Sejours.CountAsync(s => s.Statut == SejourStatut.EnCours),
                FacturesEnAttente = facturesEnAttente,
                RevenusDuMois = revenusDuMois,
                TauxOccupation = totalChambres == 0 ? 0 : Math.Round((totalChambres - chambresLibres) * 100m / totalChambres, 1),
                LastUpdated = maintenant,
                StatutsChambres = statutsChambres
                    .OrderBy(s => s.Statut)
                    .Select(s => new RoomStatusDashboardItem
                    {
                        Statut = s.Statut,
                        Libelle = GetRoomStatusLabel(s.Statut),
                        Total = s.Total,
                        Pourcentage = totalChambres == 0 ? 0 : Math.Round(s.Total * 100m / totalChambres, 1),
                        ClasseCss = GetRoomStatusCssClass(s.Statut)
                    })
                    .ToList(),
                ReservationsRecentes = reservationsRecentes
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View("Error");
        }

        private static string GetRoomStatusLabel(ChambreStatut statut)
        {
            return statut switch
            {
                ChambreStatut.Libre => "Libres",
                ChambreStatut.Reservee => "Reservees",
                ChambreStatut.Occupee => "Occupees",
                ChambreStatut.Nettoyage => "Nettoyage",
                ChambreStatut.Maintenance => "Maintenance",
                ChambreStatut.HorsService => "Hors service",
                _ => statut.ToString()
            };
        }

        private static string GetRoomStatusCssClass(ChambreStatut statut)
        {
            return statut switch
            {
                ChambreStatut.Libre => "status-free",
                ChambreStatut.Reservee => "status-booked",
                ChambreStatut.Occupee => "status-busy",
                ChambreStatut.Nettoyage => "status-cleaning",
                ChambreStatut.Maintenance => "status-maintenance",
                ChambreStatut.HorsService => "status-offline",
                _ => "status-neutral"
            };
        }
    }
}

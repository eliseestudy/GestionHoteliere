using Domain.Enums;

namespace Web.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalChambres { get; set; }
        public int ChambresLibres { get; set; }
        public int ChambresOccupees { get; set; }
        public int ChambresMaintenance { get; set; }
        public int ReservationsActives { get; set; }
        public int ArriveesDuJour { get; set; }
        public int DepartsDuJour { get; set; }
        public int ClientsTotal { get; set; }
        public int SejoursEnCours { get; set; }
        public int FacturesEnAttente { get; set; }
        public decimal RevenusDuMois { get; set; }
        public decimal TauxOccupation { get; set; }
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
        public List<RoomStatusDashboardItem> StatutsChambres { get; set; } = new();
        public List<RecentReservationDashboardItem> ReservationsRecentes { get; set; } = new();
    }

    public class RoomStatusDashboardItem
    {
        public ChambreStatut Statut { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public int Total { get; set; }
        public decimal Pourcentage { get; set; }
        public string ClasseCss { get; set; } = string.Empty;
    }

    public class RecentReservationDashboardItem
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string Chambre { get; set; } = string.Empty;
        public DateTimeOffset DateArrivee { get; set; }
        public DateTimeOffset DateDepart { get; set; }
        public ReservationStatut Statut { get; set; }
        public decimal MontantEstime { get; set; }
    }
}

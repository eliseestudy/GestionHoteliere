using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class Sejour : BaseEntity
    {
        [ForeignKey(nameof(Reservation))]
        public int? ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [ForeignKey(nameof(Chambre))]
        public int ChambreId { get; set; }
        public Chambre? Chambre { get; set; }

        public DateTimeOffset DateEntree { get; set; }
        public DateTimeOffset? DateSortie { get; set; }

        public int NbNuits { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TarifApplique { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Remise { get; set; } = 0m;

        public SejourStatut Statut { get; set; } = SejourStatut.EnCours;
    }
}

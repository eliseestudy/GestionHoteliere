using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class Sejour : BaseEntity
    {
        [ForeignKey(nameof(Reservation))]
        [Display(Name = "Réservation")]
        public int? ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [ForeignKey(nameof(Client))]
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [ForeignKey(nameof(Chambre))]
        [Display(Name = "Chambre")]
        public int ChambreId { get; set; }
        public Chambre? Chambre { get; set; }

        [Display(Name = "Date d’entrée")]
        public DateTimeOffset DateEntree { get; set; }
        [Display(Name = "Date de sortie")]
        public DateTimeOffset? DateSortie { get; set; }

        [Display(Name = "Nombre de nuits")]
        public int NbNuits { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tarif par nuit")]
        public decimal TarifApplique { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Remise")]
        public decimal Remise { get; set; } = 0m;

        public SejourStatut Statut { get; set; } = SejourStatut.EnCours;
    }
}

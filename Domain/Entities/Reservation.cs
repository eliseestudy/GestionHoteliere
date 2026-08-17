using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "N° réservation")]
        public string NumeroReservation { get; set; } = string.Empty;

        [ForeignKey(nameof(Client))]
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [ForeignKey(nameof(Chambre))]
        [Display(Name = "Chambre")]
        public int? ChambreId { get; set; }
        public Chambre? Chambre { get; set; }

        [ForeignKey(nameof(TypeChambre))]
        [Display(Name = "Type de chambre")]
        public int? TypeChambreId { get; set; }
        public TypeChambre? TypeChambre { get; set; }

        [Display(Name = "Date de création")]
        public DateTimeOffset DateCreation { get; set; } = DateTimeOffset.UtcNow;
        [Display(Name = "Date d’arrivée")]
        public DateTimeOffset DateArrivee { get; set; }
        [Display(Name = "Date de départ")]
        public DateTimeOffset DateDepart { get; set; }

        public ReservationStatut Statut { get; set; } = ReservationStatut.EnAttente;

        [Range(1, 100)]
        [Display(Name = "Nombre de personnes")]
        public int NombrePersonnes { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Montant estimé")]
        public decimal MontantEstime { get; set; } = 0m;
    }
}

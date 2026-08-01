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
        public string NumeroReservation { get; set; } = string.Empty;

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [ForeignKey(nameof(Chambre))]
        public int? ChambreId { get; set; }
        public Chambre? Chambre { get; set; }

        [ForeignKey(nameof(TypeChambre))]
        public int? TypeChambreId { get; set; }
        public TypeChambre? TypeChambre { get; set; }

        public DateTimeOffset DateCreation { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset DateArrivee { get; set; }
        public DateTimeOffset DateDepart { get; set; }

        public ReservationStatut Statut { get; set; } = ReservationStatut.EnAttente;

        [Range(1, 100)]
        public int NombrePersonnes { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantEstime { get; set; } = 0m;
    }
}

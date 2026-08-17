using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class Paiement : BaseEntity
    {
        [ForeignKey(nameof(Facture))]
        [Display(Name = "Facture")]
        public int FactureId { get; set; }
        public Facture? Facture { get; set; }

        [Display(Name = "Date de paiement")]
        public DateTimeOffset DatePaiement { get; set; } = DateTimeOffset.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; } = 0m;

        [Display(Name = "Mode de paiement")]
        public ModePaiement Mode { get; set; } = ModePaiement.Carte;

        public PaiementStatut Statut { get; set; } = PaiementStatut.Initie;

        [MaxLength(200)]
        [Display(Name = "Référence")]
        public string? TransactionReference { get; set; }

        [ForeignKey(nameof(User))]
        [Display(Name = "Utilisateur")]
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}

using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class Facture : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string NumeroFacture { get; set; } = string.Empty;

        [ForeignKey(nameof(Sejour))]
        public int? SejourId { get; set; }
        public Sejour? Sejour { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTimeOffset DateEmission { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DateEcheance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantHT { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Taxe { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantTTC { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantPaye { get; set; } = 0m;

        public FactureStatut Statut { get; set; } = FactureStatut.Brouillon;

        public ICollection<Paiement> Paiements { get; set; } = new List<Paiement>();
    }
}

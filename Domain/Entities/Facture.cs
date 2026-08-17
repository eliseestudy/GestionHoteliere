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
        [Display(Name = "N° facture")]
        public string NumeroFacture { get; set; } = string.Empty;

        [ForeignKey(nameof(Sejour))]
        [Display(Name = "Séjour")]
        public int? SejourId { get; set; }
        public Sejour? Sejour { get; set; }

        [ForeignKey(nameof(Client))]
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Display(Name = "Date d’émission")]
        public DateTimeOffset DateEmission { get; set; } = DateTimeOffset.UtcNow;
        [Display(Name = "Date d’échéance")]
        public DateTimeOffset? DateEcheance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Montant HT")]
        public decimal MontantHT { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "TVA")]
        public decimal Taxe { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Montant TTC")]
        public decimal MontantTTC { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Montant payé")]
        public decimal MontantPaye { get; set; } = 0m;

        public FactureStatut Statut { get; set; } = FactureStatut.Brouillon;

        public ICollection<Paiement> Paiements { get; set; } = new List<Paiement>();
    }
}

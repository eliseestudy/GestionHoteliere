using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionHoteliere.Domain.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(50)]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        [MaxLength(300)]
        public string? Adresse { get; set; }

        [Display(Name = "Date de naissance")]
        public DateTime? DateNaissance { get; set; }

        [MaxLength(100)]
        [Display(Name = "N° pièce d’identité")]
        public string? IdentifiantNational { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Sejour> Sejours { get; set; } = new List<Sejour>();
        public ICollection<Facture> Factures { get; set; } = new List<Facture>();
    }
}

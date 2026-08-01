using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionHoteliere.Domain.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(50)]
        public string? Telephone { get; set; }

        [MaxLength(300)]
        public string? Adresse { get; set; }

        public DateTime? DateNaissance { get; set; }

        [MaxLength(100)]
        public string? IdentifiantNational { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Sejour> Sejours { get; set; } = new List<Sejour>();
        public ICollection<Facture> Factures { get; set; } = new List<Facture>();
    }
}

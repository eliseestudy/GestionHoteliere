using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestionHoteliere.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Receptionniste;

        [MaxLength(100)]
        public string? Prenom { get; set; }

        [MaxLength(100)]
        public string? Nom { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTimeOffset? DerniereConnexion { get; set; }
    }
}

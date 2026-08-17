using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestionHoteliere.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Nom d’utilisateur")]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(200)]
        [Display(Name = "Mot de passe")]
        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "Rôle")]
        public UserRole Role { get; set; } = UserRole.Receptionniste;

        [MaxLength(100)]
        [Display(Name = "Prénom")]
        public string? Prenom { get; set; }

        [MaxLength(100)]
        public string? Nom { get; set; }

        [Display(Name = "Compte actif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Dernière connexion")]
        public DateTimeOffset? DerniereConnexion { get; set; }
    }
}

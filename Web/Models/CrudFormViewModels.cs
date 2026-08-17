using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ReservationFormViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string NumeroReservation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sélectionnez un client.")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Display(Name = "Chambre (facultative)")]
        public int? ChambreId { get; set; }

        [Required(ErrorMessage = "Sélectionnez un type de chambre.")]
        [Display(Name = "Type de chambre")]
        public int? TypeChambreId { get; set; }

        [Required]
        [Display(Name = "Date d’arrivée prévue")]
        public DateTimeOffset DateArrivee { get; set; }

        [Required]
        [Display(Name = "Date de départ prévue")]
        public DateTimeOffset DateDepart { get; set; }

        [Range(1, 100, ErrorMessage = "Le nombre de personnes doit être compris entre 1 et 100.")]
        [Display(Name = "Nombre de personnes")]
        public int NombrePersonnes { get; set; } = 1;

        [Display(Name = "Prix par nuit")]
        public decimal PrixParNuit { get; set; }

        [Display(Name = "Nombre de nuits")]
        public int NombreNuits { get; set; } = 1;

        [Display(Name = "Montant estimé")]
        public decimal MontantEstime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateDepart.Date <= DateArrivee.Date)
            {
                yield return new ValidationResult(
                    "La date de départ doit être postérieure à la date d’arrivée.",
                    new[] { nameof(DateDepart) });
            }
        }
    }

    public class ChambreFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "N° de chambre")]
        public string Numero { get; set; } = string.Empty;

        [Display(Name = "Étage")]
        public int Etage { get; set; }

        [Required(ErrorMessage = "Sélectionnez un type de chambre.")]
        [Display(Name = "Type de chambre")]
        public int TypeChambreId { get; set; }

        [Range(1, 100)]
        [Display(Name = "Nombre de lits")]
        public int NbLits { get; set; } = 1;

        [MaxLength(500)]
        [Display(Name = "Observations")]
        public string? Observations { get; set; }
    }

    public class FactureFormViewModel
    {
        public int Id { get; set; }
        public string NumeroFacture { get; set; } = string.Empty;
        public int? SejourId { get; set; }
        public string? SejourLabel { get; set; }

        [Required(ErrorMessage = "Sélectionnez un client.")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required]
        [Display(Name = "Date d’émission")]
        public DateTimeOffset DateEmission { get; set; }

        [Display(Name = "Date d’échéance")]
        public DateTimeOffset? DateEcheance { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "Montant HT")]
        public decimal MontantHT { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "TVA (18 %)")]
        public decimal Taxe { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "Montant TTC")]
        public decimal MontantTTC { get; set; }
    }

    public class TypeChambreOptionViewModel
    {
        public int Id { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public decimal PrixParNuit { get; set; }
        public int Capacite { get; set; }
    }

    public class ChambreOptionViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int TypeChambreId { get; set; }
    }
}

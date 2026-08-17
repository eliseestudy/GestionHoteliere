using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class BusinessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string> Errors { get; set; } = new();

        public static BusinessResult Ok(string message)
        {
            return new BusinessResult { Success = true, Message = message };
        }

        public static BusinessResult Fail(string message, string? key = null)
        {
            var result = new BusinessResult { Success = false, Message = message };
            if (!string.IsNullOrWhiteSpace(key))
            {
                result.Errors[key] = message;
            }

            return result;
        }
    }

    public class ReservationListItemViewModel
    {
        public Reservation Reservation { get; set; } = new();
        public bool CanConfirm { get; set; }
        public bool CanCancel { get; set; }
        public bool CanNoShow { get; set; }
        public bool CanCheckIn { get; set; }
        public bool CanDelete { get; set; }
    }

    public class SejourListItemViewModel
    {
        public Sejour Sejour { get; set; } = new();
        public bool CanCheckout { get; set; }
        public bool CanCancel { get; set; }
    }

    public class FactureListItemViewModel
    {
        public Facture Facture { get; set; } = new();
        public decimal Solde { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanEmit { get; set; }
        public bool CanDispute { get; set; }
        public bool CanResolveDispute { get; set; }
        public bool CanCancel { get; set; }
        public bool CanPay { get; set; }
    }

    public class PaiementListItemViewModel
    {
        public Paiement Paiement { get; set; } = new();
        public bool CanRefund { get; set; }
    }

    public class ChambreListItemViewModel
    {
        public Chambre Chambre { get; set; } = new();
        public bool CanSetCleaning { get; set; }
        public bool CanSetMaintenance { get; set; }
        public bool CanSetOutOfService { get; set; }
        public bool CanSetReady { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ReservationCheckInViewModel
    {
        public int ReservationId { get; set; }
        public string NumeroReservation { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Chambre")]
        public int ChambreId { get; set; }

        [Required]
        [Display(Name = "Date d’entrée")]
        public DateTimeOffset DateEntree { get; set; } = DateTimeOffset.UtcNow;

        [Range(0, 999999999)]
        [Display(Name = "Tarif par nuit")]
        public decimal TarifApplique { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "Remise")]
        public decimal Remise { get; set; }
    }

    public class SejourCheckoutViewModel
    {
        public int SejourId { get; set; }
        public string Client { get; set; } = string.Empty;
        public string Chambre { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date de sortie")]
        public DateTimeOffset DateSortie { get; set; } = DateTimeOffset.UtcNow;

        [Range(1, 1000)]
        [Display(Name = "Nombre de nuits")]
        public int NbNuits { get; set; } = 1;

        [Range(0, 999999999)]
        [Display(Name = "Tarif par nuit")]
        public decimal TarifApplique { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "Remise")]
        public decimal Remise { get; set; }

        public decimal MontantHT => Math.Max(0, NbNuits * TarifApplique - Remise);
        public decimal Taxe => Math.Round(MontantHT * 0.18m, 2);
        public decimal MontantTTC => MontantHT + Taxe;
    }

    public class PaiementContextViewModel
    {
        public int FactureId { get; set; }
        public string NumeroFacture { get; set; } = string.Empty;
        public decimal SoldeRestant { get; set; }

        [Required]
        public DateTimeOffset DatePaiement { get; set; } = DateTimeOffset.UtcNow;

        [Range(0.01, 999999999)]
        public decimal Montant { get; set; }

        public ModePaiement Mode { get; set; } = ModePaiement.Carte;

        [MaxLength(200)]
        public string? TransactionReference { get; set; }
    }
}

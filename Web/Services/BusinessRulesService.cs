using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Services
{
    public interface IBusinessRulesService
    {
        bool CanConfirmReservation(Reservation reservation);
        bool CanCancelReservation(Reservation reservation);
        bool CanNoShowReservation(Reservation reservation);
        bool CanCheckInReservation(Reservation reservation, bool hasStay);
        bool CanDeleteReservation(Reservation reservation, bool hasStay);
        bool CanCheckoutSejour(Sejour sejour, bool hasInvoice);
        bool CanCancelSejour(Sejour sejour, bool hasInvoice, bool hasPayment);
        bool CanEditFacture(Facture facture);
        bool CanDeleteFacture(Facture facture);
        bool CanEmitFacture(Facture facture);
        bool CanDisputeFacture(Facture facture);
        bool CanResolveDispute(Facture facture);
        bool CanCancelFacture(Facture facture);
        bool CanPayFacture(Facture facture);
        bool CanRefundPaiement(Paiement paiement);
        bool CanChangeRoomState(Chambre chambre);
        bool CanMarkRoomReady(Chambre chambre);
        Task<bool> CanDeleteChambreAsync(int chambreId);
        Task<bool> CanDeleteClientAsync(int clientId);
        Task<bool> CanDeleteTypeChambreAsync(int typeChambreId);
        Task<bool> CanDeleteUserAsync(int userId);
        Task RecalculateFactureAsync(int factureId);
    }

    public class BusinessRulesService : IBusinessRulesService
    {
        private readonly GestionHoteliereDbContext _context;

        public BusinessRulesService(GestionHoteliereDbContext context)
        {
            _context = context;
        }

        public bool CanConfirmReservation(Reservation reservation)
        {
            return reservation.Statut == ReservationStatut.EnAttente;
        }

        public bool CanCancelReservation(Reservation reservation)
        {
            return reservation.Statut == ReservationStatut.EnAttente ||
                reservation.Statut == ReservationStatut.Confirmee;
        }

        public bool CanNoShowReservation(Reservation reservation)
        {
            return reservation.Statut == ReservationStatut.Confirmee;
        }

        public bool CanCheckInReservation(Reservation reservation, bool hasStay)
        {
            return reservation.Statut == ReservationStatut.Confirmee && !hasStay;
        }

        public bool CanDeleteReservation(Reservation reservation, bool hasStay)
        {
            return reservation.Statut == ReservationStatut.EnAttente && !hasStay;
        }

        public bool CanCheckoutSejour(Sejour sejour, bool hasInvoice)
        {
            return sejour.Statut == SejourStatut.EnCours && !hasInvoice;
        }

        public bool CanCancelSejour(Sejour sejour, bool hasInvoice, bool hasPayment)
        {
            return sejour.Statut == SejourStatut.EnCours && !hasInvoice && !hasPayment;
        }

        public bool CanEditFacture(Facture facture)
        {
            return facture.Statut == FactureStatut.Brouillon;
        }

        public bool CanDeleteFacture(Facture facture)
        {
            return facture.Statut == FactureStatut.Brouillon;
        }

        public bool CanEmitFacture(Facture facture)
        {
            return facture.Statut == FactureStatut.Brouillon && facture.MontantTTC > 0;
        }

        public bool CanDisputeFacture(Facture facture)
        {
            return facture.Statut == FactureStatut.Emise ||
                facture.Statut == FactureStatut.PartiellementPayee;
        }

        public bool CanResolveDispute(Facture facture)
        {
            return facture.Statut == FactureStatut.EnLitige;
        }

        public bool CanCancelFacture(Facture facture)
        {
            return facture.Statut == FactureStatut.Brouillon ||
                facture.Statut == FactureStatut.Emise ||
                facture.Statut == FactureStatut.EnLitige;
        }

        public bool CanPayFacture(Facture facture)
        {
            return (facture.Statut == FactureStatut.Emise ||
                facture.Statut == FactureStatut.PartiellementPayee) &&
                facture.MontantTTC > facture.MontantPaye;
        }

        public bool CanRefundPaiement(Paiement paiement)
        {
            return paiement.Statut == PaiementStatut.Effectue;
        }

        public bool CanChangeRoomState(Chambre chambre)
        {
            return chambre.Statut != ChambreStatut.Occupee &&
                chambre.Statut != ChambreStatut.Reservee;
        }

        public bool CanMarkRoomReady(Chambre chambre)
        {
            return chambre.Statut == ChambreStatut.Nettoyage ||
                chambre.Statut == ChambreStatut.Maintenance ||
                chambre.Statut == ChambreStatut.HorsService;
        }

        public async Task<bool> CanDeleteChambreAsync(int chambreId)
        {
            var chambre = await _context.Chambres.FindAsync(chambreId);
            if (chambre == null || !CanChangeRoomState(chambre))
            {
                return false;
            }

            return !await _context.Sejours.AnyAsync(s => s.ChambreId == chambreId) &&
                !await _context.Reservations.AnyAsync(r => r.ChambreId == chambreId);
        }

        public async Task<bool> CanDeleteClientAsync(int clientId)
        {
            return !await _context.Reservations.AnyAsync(r => r.ClientId == clientId) &&
                !await _context.Sejours.AnyAsync(s => s.ClientId == clientId) &&
                !await _context.Factures.AnyAsync(f => f.ClientId == clientId);
        }

        public async Task<bool> CanDeleteTypeChambreAsync(int typeChambreId)
        {
            return !await _context.Chambres.AnyAsync(c => c.TypeChambreId == typeChambreId) &&
                !await _context.Reservations.AnyAsync(r => r.TypeChambreId == typeChambreId);
        }

        public async Task<bool> CanDeleteUserAsync(int userId)
        {
            return !await _context.Paiements.AnyAsync(p => p.UserId == userId);
        }

        public async Task RecalculateFactureAsync(int factureId)
        {
            var facture = await _context.Factures
                .Include(f => f.Paiements)
                .FirstOrDefaultAsync(f => f.Id == factureId);

            if (facture == null || facture.Statut == FactureStatut.Annulee)
            {
                return;
            }

            facture.MontantPaye = facture.Paiements
                .Where(p => p.Statut == PaiementStatut.Effectue)
                .Sum(p => p.Montant);

            if (facture.Statut == FactureStatut.EnLitige)
            {
                return;
            }

            facture.Statut = facture.MontantPaye <= 0
                ? FactureStatut.Emise
                : facture.MontantPaye >= facture.MontantTTC
                    ? FactureStatut.Payee
                    : FactureStatut.PartiellementPayee;
        }
    }
}

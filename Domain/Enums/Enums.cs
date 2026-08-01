using System;

namespace Domain.Enums
{
    public enum ReservationStatut
    {
        EnAttente = 0,
        Confirmee = 1,
        ArriveeEnCours = 2,
        Termine = 3,
        Annulee = 4,
        NoShow = 5
    }

    public enum ChambreStatut
    {
        Libre = 0,
        Reservee = 1,
        Occupee = 2,
        Nettoyage = 3,
        Maintenance = 4,
        HorsService = 5
    }

    public enum FactureStatut
    {
        Brouillon = 0,
        Emise = 1,
        PartiellementPayee = 2,
        Payee = 3,
        EnLitige = 4,
        Annulee = 5
    }

    public enum PaiementStatut
    {
        Initie = 0,
        Authorise = 1,
        Effectue = 2,
        Echoue = 3,
        Rembourse = 4,
        Partiel = 5
    }

    public enum ModePaiement
    {
        Carte = 0,
        Especes = 1,
        Virement = 2,
        Cheque = 3,
        MobileMoney = 4
    }

    public enum UserRole
    {
        Admin = 0,
        Receptionniste = 1,
        Comptable = 2,
        Housekeeping = 3
    }

    public enum SejourStatut
    {
        EnCours = 0,
        Termine = 1,
        Annule = 2
    }
}

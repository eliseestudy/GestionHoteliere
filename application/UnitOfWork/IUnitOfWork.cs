using Application.Repositories;

namespace Application.UnitOfWork
{

    public interface IUnitOfWork : IDisposable
    {
        IChambreRepository Chambres { get; }
        ITypeChambreRepository TypesChambres { get; }
        IClientRepository Clients { get; }
        IReservationRepository Reservations { get; }
        ISejourRepository Sejours { get; }
        IFactureRepository Factures { get; }
        IPaiementRepository Paiements { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}

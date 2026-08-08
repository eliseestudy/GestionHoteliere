using Application.Repositories;
using Application.UnitOfWork;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        // Une seule instance de contexte, partagee par tous les repositories.
        // C'est ce qui rend l'ecriture atomique.
        private readonly GestionHoteliereDbContext _context;

        private IChambreRepository? _chambres;
        private ITypeChambreRepository? _typesChambres;
        private IClientRepository? _clients;
        private IReservationRepository? _reservations;
        private ISejourRepository? _sejours;
        private IFactureRepository? _factures;
        private IPaiementRepository? _paiements;

        public UnitOfWork(GestionHoteliereDbContext context)
        {
            _context = context;
        }

        // Creation a la demande : un repository jamais utilise n'est jamais instancie.
        public IChambreRepository Chambres =>
            _chambres ??= new ChambreRepository(_context);

        public ITypeChambreRepository TypesChambres =>
            _typesChambres ??= new TypeChambreRepository(_context);

        public IClientRepository Clients =>
            _clients ??= new ClientRepository(_context);

        public IReservationRepository Reservations =>
            _reservations ??= new ReservationRepository(_context);

        public ISejourRepository Sejours =>
            _sejours ??= new SejourRepository(_context);

        public IFactureRepository Factures =>
            _factures ??= new FactureRepository(_context);

        public IPaiementRepository Paiements =>
            _paiements ??= new PaiementRepository(_context);

        // Un seul appel valide toutes les ecritures en attente.
        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
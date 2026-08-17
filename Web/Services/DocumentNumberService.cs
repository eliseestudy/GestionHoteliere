using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Web.Services
{
    public interface IDocumentNumberService
    {
        Task<string> NextReservationNumberAsync(DateTimeOffset date, CancellationToken cancellationToken = default);
        Task<string> NextInvoiceNumberAsync(DateTimeOffset date, CancellationToken cancellationToken = default);
    }

    public class DocumentNumberService : IDocumentNumberService
    {
        private readonly GestionHoteliereDbContext _context;

        public DocumentNumberService(GestionHoteliereDbContext context)
        {
            _context = context;
        }

        public Task<string> NextReservationNumberAsync(DateTimeOffset date, CancellationToken cancellationToken = default)
        {
            return NextAsync("RES", date.Year, cancellationToken);
        }

        public Task<string> NextInvoiceNumberAsync(DateTimeOffset date, CancellationToken cancellationToken = default)
        {
            return NextAsync("FAC", date.Year, cancellationToken);
        }

        private async Task<string> NextAsync(string documentType, int year, CancellationToken cancellationToken)
        {
            var counter = await _context.DocumentNumberCounters
                .SingleOrDefaultAsync(x => x.DocumentType == documentType && x.Year == year, cancellationToken);

            int value;
            if (counter == null)
            {
                value = 1;
                _context.DocumentNumberCounters.Add(new DocumentNumberCounter
                {
                    DocumentType = documentType,
                    Year = year,
                    NextValue = 2
                });
            }
            else
            {
                value = counter.NextValue;
                counter.NextValue++;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Format(documentType, year, value);
        }

        public static string Format(string documentType, int year, int value)
        {
            return $"{documentType}-{year:D4}-{value:D6}";
        }
    }
}

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Web.Services;

namespace Tests;

public class DocumentNumberServiceTests
{
    [Fact]
    public async Task Counters_are_independent_and_reset_each_year()
    {
        await using var context = CreateContext();
        var service = new DocumentNumberService(context);

        Assert.Equal("RES-2026-000001", await service.NextReservationNumberAsync(new DateTimeOffset(2026, 8, 14, 0, 0, 0, TimeSpan.Zero)));
        Assert.Equal("RES-2026-000002", await service.NextReservationNumberAsync(new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero)));
        Assert.Equal("FAC-2026-000001", await service.NextInvoiceNumberAsync(new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero)));
        Assert.Equal("RES-2027-000001", await service.NextReservationNumberAsync(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Theory]
    [InlineData("RES", 2026, 42, "RES-2026-000042")]
    [InlineData("FAC", 2030, 1, "FAC-2030-000001")]
    public void Format_uses_a_six_digit_sequence(string type, int year, int value, string expected)
    {
        Assert.Equal(expected, DocumentNumberService.Format(type, year, value));
    }

    private static GestionHoteliereDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GestionHoteliereDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GestionHoteliereDbContext(options);
    }
}

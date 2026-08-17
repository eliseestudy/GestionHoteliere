using Web.Models;
using Web.Services;

namespace Tests;

public class ReservationPricingCalculatorTests
{
    [Fact]
    public void Calculate_returns_nights_and_total_from_dates_and_database_price()
    {
        var result = ReservationPricingCalculator.Calculate(
            new DateTimeOffset(2026, 8, 14, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 18, 0, 0, 0, TimeSpan.Zero),
            35_000m);

        Assert.Equal(4, result.Nights);
        Assert.Equal(35_000m, result.PricePerNight);
        Assert.Equal(140_000m, result.Total);
    }

    [Fact]
    public void Calculate_rejects_an_invalid_period()
    {
        var date = new DateTimeOffset(2026, 8, 14, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentException>(() => ReservationPricingCalculator.Calculate(date, date, 35_000m));
    }

    [Fact]
    public void Reservation_view_model_rejects_departure_before_arrival()
    {
        var model = new ReservationFormViewModel
        {
            DateArrivee = new DateTimeOffset(2026, 8, 15, 0, 0, 0, TimeSpan.Zero),
            DateDepart = new DateTimeOffset(2026, 8, 14, 0, 0, 0, TimeSpan.Zero)
        };

        Assert.NotEmpty(model.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(model)));
    }
}

namespace Web.Services
{
    public record ReservationPriceCalculation(int Nights, decimal PricePerNight, decimal Total);

    public static class ReservationPricingCalculator
    {
        public static ReservationPriceCalculation Calculate(
            DateTimeOffset arrival,
            DateTimeOffset departure,
            decimal pricePerNight)
        {
            if (departure.Date <= arrival.Date)
            {
                throw new ArgumentException("La date de départ doit être postérieure à la date d’arrivée.");
            }

            if (pricePerNight < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pricePerNight));
            }

            var nights = Math.Max(1, (int)Math.Ceiling((departure.Date - arrival.Date).TotalDays));
            return new ReservationPriceCalculation(nights, pricePerNight, nights * pricePerNight);
        }
    }
}

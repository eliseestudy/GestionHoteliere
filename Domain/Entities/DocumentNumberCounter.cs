namespace GestionHoteliere.Domain.Entities
{
    public class DocumentNumberCounter
    {
        public string DocumentType { get; set; } = string.Empty;
        public int Year { get; set; }
        public int NextValue { get; set; } = 1;
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class TypeChambre : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Libelle { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(1, 100)]
        [Display(Name = "Capacité maximale")]
        public int Capacite { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Prix par nuit")]
        public decimal PrixParNuit { get; set; } = 0m;

        public ICollection<Chambre> Chambres { get; set; } = new List<Chambre>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    }
}

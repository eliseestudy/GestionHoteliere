using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionHoteliere.Domain.Entities
{
    public class Chambre : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "N° de chambre")]
        public string Numero { get; set; } = string.Empty;

        [Display(Name = "Étage")]
        public int Etage { get; set; } = 0;

        [ForeignKey(nameof(TypeChambre))]
        [Display(Name = "Type de chambre")]
        public int TypeChambreId { get; set; }
        public TypeChambre? TypeChambre { get; set; }

        public ChambreStatut Statut { get; set; } = ChambreStatut.Libre;

        [Display(Name = "Nombre de lits")]
        public int NbLits { get; set; } = 1;

        [MaxLength(500)]
        public string? Observations { get; set; }

        public virtual ICollection<Sejour> Sejours { get; set; } = new List<Sejour>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

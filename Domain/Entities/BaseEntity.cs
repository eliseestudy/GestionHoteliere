using System;
using System.ComponentModel.DataAnnotations;

namespace GestionHoteliere.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Audit
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public int? CreatedById { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }

        // Soft-delete
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedAt { get; set; }
        public int? DeletedById { get; set; }

        // Concurrency token
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}

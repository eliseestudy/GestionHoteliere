using System;
using System.Collections.Generic;
using System.Text;
using GestionHoteliere.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GestionHoteliereDbContext : DbContext
    {
        public GestionHoteliereDbContext(DbContextOptions<GestionHoteliereDbContext> options)
            : base(options)
        {
        }


        public DbSet<TypeChambre> TypesChambres =>
        Set<TypeChambre>();

        public DbSet<Chambre> Chambres =>
            Set<Chambre>();

        public DbSet<Client> Clients =>
            Set<Client>();

        public DbSet<Reservation> Reservations =>
            Set<Reservation>();

        public DbSet<Sejour> Sejours =>
            Set<Sejour>();

        public DbSet<Facture> Factures =>
            Set<Facture>();

        public DbSet<Paiement> Paiements =>
            Set<Paiement>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurerTypeChambre(modelBuilder);
            ConfigurerChambre(modelBuilder);
            ConfigurerClient(modelBuilder);
            ConfigurerReservation(modelBuilder);
            ConfigurerSejour(modelBuilder);
            ConfigurerFacture(modelBuilder);
            ConfigurerPaiement(modelBuilder);
            ConfigurerSuppressionLogique(modelBuilder);
        }

        private static void ConfigurerTypeChambre(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeChambre>(entity =>
            {
                entity.ToTable("TypesChambres");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Libelle)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.PrixParNuit)
                    .HasPrecision(18, 2);

                entity.HasIndex(x => x.Libelle)
                    .IsUnique();
            });
        }

        private static void ConfigurerChambre(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chambre>(entity =>
            {
                entity.ToTable("Chambres");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Numero)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Observations)
                    .HasMaxLength(500);

                entity.HasIndex(x => x.Numero)
                    .IsUnique();

                entity.HasOne(x => x.TypeChambre)
                    .WithMany(x => x.Chambres)
                    .HasForeignKey(x => x.TypeChambreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurerClient(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Clients");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Prenom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Email)
                    .HasMaxLength(200);

                entity.Property(x => x.Telephone)
                    .HasMaxLength(50);

                entity.Property(x => x.Adresse)
                    .HasMaxLength(300);

                entity.Property(x => x.IdentifiantNational)
                    .HasMaxLength(100);

                entity.HasIndex(x => x.Email);

                entity.HasIndex(x => x.IdentifiantNational);
            });
        }

        private static void ConfigurerReservation(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservations");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.NumeroReservation)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.MontantEstime)
                    .HasPrecision(18, 2);

                entity.HasIndex(x => x.NumeroReservation)
                    .IsUnique();

                entity.HasOne(x => x.Client)
                    .WithMany(x => x.Reservations)
                    .HasForeignKey(x => x.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Chambre)
                    .WithMany(x => x.Reservations)
                    .HasForeignKey(x => x.ChambreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TypeChambre)
                    .WithMany(x => x.Reservations)
                    .HasForeignKey(x => x.TypeChambreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurerSejour(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sejour>(entity =>
            {
                entity.ToTable("Sejours");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.TarifApplique)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Remise)
                    .HasPrecision(18, 2);

                entity.HasOne(x => x.Client)
                    .WithMany(x => x.Sejours)
                    .HasForeignKey(x => x.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Chambre)
                    .WithMany(x => x.Sejours)
                    .HasForeignKey(x => x.ChambreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurerFacture(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Facture>(entity =>
            {
                entity.ToTable("Factures");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.NumeroFacture)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.MontantHT)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Taxe)
                    .HasPrecision(18, 2);

                entity.Property(x => x.MontantTTC)
                    .HasPrecision(18, 2);

                entity.Property(x => x.MontantPaye)
                    .HasPrecision(18, 2);

                entity.HasIndex(x => x.NumeroFacture)
                    .IsUnique();

                entity.HasOne(x => x.Client)
                    .WithMany(x => x.Factures)
                    .HasForeignKey(x => x.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurerPaiement(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paiement>(entity =>
            {
                entity.ToTable("Paiements");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Montant)
                    .HasPrecision(18, 2);

                entity.Property(x => x.TransactionReference)
                    .HasMaxLength(200);

                entity.HasOne(x => x.Facture)
                    .WithMany(x => x.Paiements)
                    .HasForeignKey(x => x.FactureId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurerSuppressionLogique(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeChambre>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Chambre>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Client>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Reservation>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Sejour>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Facture>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Paiement>()
                .HasQueryFilter(x => !x.IsDeleted);
        }





    }
}

using System;
using KinoApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Film> Films { get; set; } = null!;
        public DbSet<Sala> Sale { get; set; } = null!;
        public DbSet<Miejsce> Miejsca { get; set; } = null!;
        public DbSet<Seans> Seanse { get; set; } = null!;
        public DbSet<Uzytkownik> Uzytkownicy { get; set; } = null!;
        public DbSet<Rezerwacja> Rezerwacje { get; set; } = null!;
        public DbSet<Bilet> Bilety { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Konfiguracja modelu (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Film - konfiguracja zgodna z Twoim modelem (polskie nazwy pól)
            modelBuilder.Entity<Film>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Tytul)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(f => f.Opis)
                    .HasMaxLength(4000);

                entity.Property(f => f.PlakatPath)
                    .HasMaxLength(1024);

                entity.Property(f => f.Gatunek)
                    .HasMaxLength(200);

                entity.Property(f => f.Rezyser)
                    .HasMaxLength(200);

                entity.Property(f => f.Obsada)
                    .HasMaxLength(1000);

                entity.Property(f => f.KategoriaWiekowa)
                    .HasMaxLength(50);

                entity.Property(f => f.Ocena)
                    .HasDefaultValue(0);
                
                entity.Property(f => f.CzasTrwaniaMin)
                    .IsRequired();
            });

            // Sala - minimalna konfiguracja (dostosuj w razie potrzeby)
            modelBuilder.Entity<Sala>(entity =>
            {
                entity.HasKey(s => s.Id);
                // Załóżmy, że Sala zawiera Rows / Cols - jeśli nie, dopasuj później
            });

            // Seans - minimalna konfiguracja
            modelBuilder.Entity<Seans>(entity =>
            {
                entity.HasKey(s => s.Id);
                // Zakładamy, że Seans ma właściwości Start (DateTime), FilmId (int) i SalaId (int)
                // Jeśli Twój model ma inne nazwy, powiadom, a dopasuję.
            });

            // Rezerwacja / Bilet / Miejsce / Uzytkownik - minimalne ustawienia
            modelBuilder.Entity<Rezerwacja>(entity =>
            {
                entity.HasKey(r => r.Id);
            });

            modelBuilder.Entity<Bilet>(entity =>
            {
                entity.HasKey(b => b.Id);
            });

            modelBuilder.Entity<Miejsce>(entity =>
            {
                entity.HasKey(m => m.Id);
            });

            modelBuilder.Entity<Uzytkownik>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Login).HasMaxLength(150);
            });

            // Seed przykładowego filmu (dostosowane do Twojego modelu)
            modelBuilder.Entity<Film>().HasData(new Film
            {
                Id = 1,
                Tytul = "Powrót do przyszłości",
                Opis = "Klasyczny film przygodowy o podróżach w czasie.",
                CzasTrwaniaMin = 116,
                PlakatPath = "Resources/Posters/back-to-the-future.jpg",
                Gatunek = "Przygodowy",
                DataPremiery = new DateTime(1985, 7, 3),
                Rezyser = "Robert Zemeckis",
                Obsada = "Michael J. Fox, Christopher Lloyd, Lea Thompson",
                KategoriaWiekowa = "PG",
                Ocena = 9
            });

            // UWAGA:
            // - Jeżeli Twoje modele (Sala, Miejsce, Seans, Rezerwacja, Bilet, Uzytkownik) mają inne pola lub relacje,
            //   dopasuję mapowanie po otrzymaniu ich definicji (widzę pliki w repo i mogę zrobić to automatycznie).
            // - W razie potrzeby dopiszę relacje FK (HasOne/WithMany) na podstawie Twoich modeli.
        }
    }
}

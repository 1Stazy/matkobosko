using System;

namespace KinoApp.Core.Models
{
    public class Bilet
    {
        public int Id { get; set; }
        public int RezerwacjaId { get; set; }
        public Rezerwacja Rezerwacja { get; set; } = null!;
        public DateTime DataSprzedazy { get; set; } = DateTime.UtcNow;
        public decimal Cena { get; set; }
        public string Numer { get; set; } = ""; // numer biletu
    }
}

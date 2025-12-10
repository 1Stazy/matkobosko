using System;
using System.Collections.Generic;

namespace KinoApp.Core.Models
{
    public class Rezerwacja
    {
        public int Id { get; set; }
        public int SeansId { get; set; }
        public Seans Seans { get; set; } = null!;
        public DateTime DataRezerwacji { get; set; } = DateTime.UtcNow;
        public List<Miejsce> Miejsca { get; set; } = new();
        public string Status { get; set; } = "Zarezerwowano"; // Zarezerwowano, Anulowane, Sprzedane
        public int UzytkownikId { get; set; }
    }
}

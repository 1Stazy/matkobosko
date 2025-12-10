using System;

namespace KinoApp.Core.Models
{
    public class Film
    {
        public int Id { get; set; }

        public string Tytul { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        /// <summary>
        /// Długość w minutach
        /// </summary>
        public int CzasTrwaniaMin { get; set; }

        /// <summary>
        /// Relatywna ścieżka do pliku plakatu, np. "Resources/Posters/back-to-the-future.jpg"
        /// </summary>
        public string PlakatPath { get; set; } = string.Empty;

        public string Gatunek { get; set; } = string.Empty;

        public DateTime DataPremiery { get; set; }

        public string Rezyser { get; set; } = string.Empty;

        /// <summary>
        /// Najpopularniejsi trzej aktorzy — zapis jako przecinkowy string
        /// </summary>
        public string Obsada { get; set; } = string.Empty;

        public string KategoriaWiekowa { get; set; } = string.Empty;

        /// <summary>
        /// Ocena z Filmwebu (1..10), zaokrąglona w górę do pełnej liczby
        /// </summary>
        public int Ocena { get; set; }
    }
}

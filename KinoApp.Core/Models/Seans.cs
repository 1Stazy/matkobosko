using System;

namespace KinoApp.Core.Models
{
    public class Seans
    {
        public int Id { get; set; }

        public int FilmId { get; set; }
        public Film Film { get; set; } = null!;

        public int SalaId { get; set; }
        public Sala Sala { get; set; } = null!;

        public DateTime DataCzas { get; set; }

        /// <summary>
        /// Długość seansu jako TimeSpan. Jeśli obiekt Film jest dostępny, zwróci TimeSpan na podstawie pola CzasTrwaniaMin.
        /// Jeśli Film jest null (np. w niektórych scenariuszach projektowania/serializacji), zwróci TimeSpan.Zero.
        /// </summary>
        public TimeSpan Dlugosc => TimeSpan.FromMinutes(Film?.CzasTrwaniaMin ?? 0);
    }
}

namespace KinoApp.Core.Models
{
    public class Miejsce
    {
        public int Id { get; set; }
        public int SalaId { get; set; }
        public int Rzad { get; set; }
        public int Kolumna { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}

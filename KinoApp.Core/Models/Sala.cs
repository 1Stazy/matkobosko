using System.Collections.Generic;

namespace KinoApp.Core.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nazwa { get; set; } = "";
        public int Kolumny { get; set; } = 12; // stale przyjęte
        public int Rzedow { get; set; } = 18;  // stale przyjęte

        // Miejsca nie są edytowalne w UI, ale przechowujemy ich model logiczny
        public List<Miejsce> Miejsca { get; set; } = new List<Miejsce>();
    }
}

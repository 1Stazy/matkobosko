namespace KinoApp.Core.Models
{
    public class Uzytkownik
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string HasloHash { get; set; } = ""; // proste w MVP (hashowanie poza zakresem)
        public string Rola { get; set; } = "Pracownik"; // Admin/Pracownik
    }
}

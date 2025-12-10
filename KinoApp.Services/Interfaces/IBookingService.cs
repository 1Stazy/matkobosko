using System.Collections.Generic;
using System.Threading.Tasks;

namespace KinoApp.Services.Interfaces
{
    /// <summary>
    /// Usługa zajmująca się rezerwacjami i sprzedażą biletów (kontrakty używają typów int zgodnie z modelami w Core).
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Rezerwuje miejsca dla danego seansu. Zwraca Id utworzonej rezerwacji (int).
        /// </summary>
        Task<int> ReserveSeatsAsync(int showId, IEnumerable<(int rzad, int kolumna)> seats, string reservedBy);

        /// <summary>
        /// Kupuje miejsca (tworzy bilety). Zwraca listę Id wygenerowanych biletów.
        /// </summary>
        Task<IEnumerable<int>> PurchaseSeatsAsync(int showId, IEnumerable<(int rzad, int kolumna)> seats, string purchasedBy);

        /// <summary>
        /// Anuluje rezerwację o podanym Id.
        /// </summary>
        Task CancelReservationAsync(int reservationId);
    }
}

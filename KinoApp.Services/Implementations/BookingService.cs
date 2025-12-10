using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KinoApp.Core.Models;
using KinoApp.Infrastructure.Data;
using KinoApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinoApp.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _db;

        public BookingService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Rezerwuje miejsca dla danego seansu. Zwraca Id rezerwacji (int).
        /// W implementacji MVP tworzymy nową Rezerwacja i kopie obiektów Miejsce z podanych rzędów/kolumn.
        /// </summary>
        public async Task<int> ReserveSeatsAsync(int showId, IEnumerable<(int rzad, int kolumna)> seats, string reservedBy)
        {
            var seans = await _db.Seanse.Include(s => s.Sala).FirstOrDefaultAsync(s => s.Id == showId);
            if (seans == null) throw new ArgumentException("Seans nie istnieje", nameof(showId));

            var reservation = new Rezerwacja
            {
                SeansId = seans.Id,
                DataRezerwacji = DateTime.UtcNow,
                Status = "Zarezerwowano",
                UzytkownikId = 0 // jeśli chcesz mapować reservedBy na UzytkownikId, dodaj mechanizm; MVP trzyma string w innym miejscu
            };

            // dla MVP: tworzymy Miejsce obiekty powiązane z rezerwacją (kopie pozycji w sali)
            foreach (var (r, k) in seats)
            {
                var m = new Miejsce
                {
                    SalaId = seans.SalaId,
                    Rzad = r,
                    Kolumna = k,
                    IsAvailable = false // oznaczamy jako zajęte w kontekście rezerwacji
                };
                reservation.Miejsca.Add(m);
            }

            await _db.Rezerwacje.AddAsync(reservation);
            await _db.SaveChangesAsync();

            return reservation.Id;
        }

        /// <summary>
        /// Kupuje miejsca bezpośrednio (tworzy bilety). Zwraca listę Id wygenerowanych biletów.
        /// Jeśli miejsca były wcześniej zarezerwowane, powiązujemy sprzedaż z rezerwacją (jeśli istnieje).
        /// </summary>
        public async Task<IEnumerable<int>> PurchaseSeatsAsync(int showId, IEnumerable<(int rzad, int kolumna)> seats, string purchasedBy)
        {
            var seans = await _db.Seanse.Include(s => s.Film).FirstOrDefaultAsync(s => s.Id == showId);
            if (seans == null) throw new ArgumentException("Seans nie istnieje", nameof(showId));

            var ticketIds = new List<int>();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // w MVP nie mamy rozbudowanego mechanizmu konfliktów; zakładamy, że miejsca są wolne
                var reservation = new Rezerwacja
                {
                    SeansId = seans.Id,
                    DataRezerwacji = DateTime.UtcNow,
                    Status = "Sprzedane",
                    UzytkownikId = 0
                };
                await _db.Rezerwacje.AddAsync(reservation);
                await _db.SaveChangesAsync();

                foreach (var (r, k) in seats)
                {
                    // utwórz Miejsce (powiązane z rezerwacją)
                    var m = new Miejsce
                    {
                        SalaId = seans.SalaId,
                        Rzad = r,
                        Kolumna = k,
                        IsAvailable = false
                    };
                    reservation.Miejsca.Add(m);

                    var ticket = new Bilet
                    {
                        RezerwacjaId = reservation.Id,
                        DataSprzedazy = DateTime.UtcNow,
                        Cena = 0m, // ustaw cenę zgodnie z zasadami (tutaj 0 jako placeholder)
                        Numer = Guid.NewGuid().ToString().ToUpperInvariant()
                    };
                    await _db.Bilety.AddAsync(ticket);
                    await _db.SaveChangesAsync();
                    ticketIds.Add(ticket.Id);
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return ticketIds;
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _db.Rezerwacje
                .Include(r => r.Miejsca)
                .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null) return;

            // zwolnij miejsca (w naszym modelu po prostu usuwamy miejsca powiązane z rezerwacją)
            _db.Miejsca.RemoveRange(reservation.Miejsca);
            _db.Rezerwacje.Remove(reservation);
            await _db.SaveChangesAsync();
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KinoApp.Core.Models;
using KinoApp.Infrastructure.Data;
using KinoApp.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KinoApp.UI.ViewModels
{
    public partial class SeatSelectionViewModel : ObservableObject
    {
        public ObservableCollection<SeatCellViewModel> SeatViewModels { get; } = new();
        public string Naglowek { get; set; } = "";

        private readonly Seans _seans;
        private readonly AppDbContext _db;

        public IAsyncRelayCommand ReserveCommand { get; }
        public IAsyncRelayCommand PrintTicketCommand { get; }

        // Domyślna cena biletu (MVP)
        private const decimal CenaJednostkowa = 20m;

        public SeatSelectionViewModel(Seans seans, AppDbContext db)
        {
            _seans = seans ?? throw new ArgumentNullException(nameof(seans));
            _db = db ?? throw new ArgumentNullException(nameof(db));
            Naglowek = $"{seans.Film.Tytul} - {seans.DataCzas:g} - {seans.Sala.Nazwa}";

            LoadSeats();

            ReserveCommand = new AsyncRelayCommand(ReserveSelectedAsync);
            PrintTicketCommand = new AsyncRelayCommand(PrintTicketAsync);
        }

        private void LoadSeats()
        {
            SeatViewModels.Clear();

            var miejsca = _db.Miejsca
                .Where(m => m.SalaId == _seans.SalaId)
                .OrderBy(m => m.Rzad)
                .ThenBy(m => m.Kolumna)
                .ToList();

            foreach (var m in miejsca)
            {
                SeatViewModels.Add(new SeatCellViewModel
                {
                    MiejsceId = m.Id,
                    Rzad = m.Rzad,
                    Kolumna = m.Kolumna,
                    IsAvailable = m.IsAvailable
                });
            }
        }

        private async Task ReserveSelectedAsync()
        {
            try
            {
                var selected = SeatViewModels.Where(s => s.IsSelected).ToList();

                if (!selected.Any())
                {
                    MessageBox.Show("Brak wybranego miejsca. Wybierz przynajmniej jedno miejsce do rezerwacji.", "Rezerwacja", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                using var transaction = await _db.Database.BeginTransactionAsync();

                var miejscaEntities = new System.Collections.Generic.List<Miejsce>();
                foreach (var s in selected)
                {
                    var m = await _db.Miejsca.FindAsync(s.MiejsceId);
                    if (m == null)
                    {
                        await transaction.RollbackAsync();
                        MessageBox.Show($"Miejsce o Id={s.MiejsceId} nie zostało znalezione w bazie.", "Błąd rezerwacji", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!m.IsAvailable)
                    {
                        await transaction.RollbackAsync();
                        MessageBox.Show($"Miejsce {m.Rzad}-{m.Kolumna} jest już zajęte. Odśwież widok i spróbuj ponownie.", "Miejsce zajęte", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    m.IsAvailable = false;
                    miejscaEntities.Add(m);
                }

                var rezerwacja = new Rezerwacja
                {
                    SeansId = _seans.Id,
                    DataRezerwacji = DateTime.UtcNow,
                    Status = "Zarezerwowano",
                    Miejsca = miejscaEntities
                };

                _db.Rezerwacje.Add(rezerwacja);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                MessageBox.Show("Zarezerwowano miejsca.", "Rezerwacja", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSeats();
            }
            catch (Exception ex)
            {
                var msg = $"Błąd podczas rezerwacji: {ex.GetType().Name}: {ex.Message}";
                MessageBox.Show(msg + "\n\nSzczegóły zapisano do pliku logu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    var log = $"[{DateTime.Now:O}] {ex}\n\n";
                    var path = Path.Combine(AppContext.BaseDirectory, "kino_error.log");
                    File.AppendAllText(path, log);
                }
                catch { }
            }
        }

        private async Task PrintTicketAsync()
        {
            try
            {
                var selected = SeatViewModels.Where(s => s.IsSelected).ToList();
                if (!selected.Any())
                {
                    MessageBox.Show("Wybierz przynajmniej jedno miejsce, aby wydrukować bilet.", "Drukowanie biletu", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                using var transaction = await _db.Database.BeginTransactionAsync();

                var miejscaEntities = new System.Collections.Generic.List<Miejsce>();
                foreach (var s in selected)
                {
                    var m = await _db.Miejsca.FindAsync(s.MiejsceId);
                    if (m == null) { await transaction.RollbackAsync(); MessageBox.Show($"Miejsce {s.MiejsceId} nie znalezione."); return; }
                    if (!m.IsAvailable) { await transaction.RollbackAsync(); MessageBox.Show($"Miejsce {m.Rzad}-{m.Kolumna} zajęte."); return; }
                    m.IsAvailable = false;
                    miejscaEntities.Add(m);
                }

                var rezerwacja = new Rezerwacja
                {
                    SeansId = _seans.Id,
                    DataRezerwacji = DateTime.UtcNow,
                    Status = "Sprzedane",
                    Miejsca = miejscaEntities
                };

                _db.Rezerwacje.Add(rezerwacja);
                await _db.SaveChangesAsync();

                var bilet = new Bilet
                {
                    RezerwacjaId = rezerwacja.Id,
                    DataSprzedazy = DateTime.UtcNow,
                    Cena = selected.Count * CenaJednostkowa,
                    Numer = Guid.NewGuid().ToString("N").ToUpperInvariant()
                };

                _db.Bilety.Add(bilet);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                // get pdf service
                var pdfService = App.AppHost?.Services.GetService(typeof(IPdfService)) as IPdfService;
                if (pdfService == null)
                {
                    MessageBox.Show("Usługa PDF niedostępna.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var rezerwacjaFull = await _db.Rezerwacje
                    .Include(r => r.Seans)
                        .ThenInclude(s => s.Film)
                    .Include(r => r.Seans)
                        .ThenInclude(s => s.Sala)
                    .Include(r => r.Miejsca)
                    .FirstOrDefaultAsync(r => r.Id == rezerwacja.Id);

                var pdfBytes = pdfService.GenerateTicketPdf(bilet, rezerwacjaFull);

                // folder Tickets
                var ticketsDir = Path.Combine(AppContext.BaseDirectory, "Tickets");
                if (!Directory.Exists(ticketsDir)) Directory.CreateDirectory(ticketsDir);

                var fileName = $"ticket_{bilet.Numer}.pdf";
                var fullPath = Path.Combine(ticketsDir, fileName);
                await File.WriteAllBytesAsync(fullPath, pdfBytes);

                // Otwórz wbudowany viewer
                var viewer = new Views.PdfViewerWindow(fullPath);
                viewer.Show();

                LoadSeats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas generowania biletu: {ex.Message}");
            }
        }

    }
}

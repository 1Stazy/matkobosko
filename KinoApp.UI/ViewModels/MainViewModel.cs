using KinoApp.Core.Models;
using KinoApp.Infrastructure.Data;
using KinoApp.UI.Views.Placeholders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KinoApp.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AppDbContext? _db;

        public ObservableCollection<Film> DzisiejszeFilmy { get; set; } = new();
        public ObservableCollection<TileViewModel> Tiles { get; set; } = new();

        public string DzisiejszaData => DateTime.Now.ToString("dddd, dd MMMM yyyy");

        // Nawigacja - komendy dla wszystkich modułów
        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateFilmsCommand { get; }
        public ICommand NavigateScheduleCommand { get; }
        public ICommand NavigateReservationsCommand { get; }
        public ICommand NavigateSalesCommand { get; }
        public ICommand NavigateUsersCommand { get; }

        // Content area
        private object? _currentContent;
        public object? CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(AppDbContext? db)
        {
            _db = db;

            // Komendy nawigacyjne
            NavigateDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
            NavigateFilmsCommand = new RelayCommand(_ => NavigateToFilms());
            NavigateScheduleCommand = new RelayCommand(_ => NavigateToSchedule());
            NavigateReservationsCommand = new RelayCommand(_ => NavigateToReservations());
            NavigateSalesCommand = new RelayCommand(_ => NavigateToSales());
            NavigateUsersCommand = new RelayCommand(_ => NavigateToUsers());

            // Inicjalizacja danych
            LoadMovies();
            LoadTiles();

            // domyślny widok
            NavigateToDashboard();
        }

        private void NavigateToFilms()
        {
            try
            {
                // Spróbuj utworzyć konkretny widok jeśli jest dostępny w projekcie
                var view = new Views.FilmsView();
                CurrentContent = view;
                return;
            }
            catch { /* brak FilmsView w projekcie -> fallback */ }

            // fallback: prosty TextBlock (bez konieczności tworzenia placeholderu)
            CurrentContent = CreateFallbackView("Widok Filmy (brak FilmsView)");
        }

        private void NavigateToDashboard()
        {
            try
            {
                var view = new DashboardView();
                CurrentContent = view;
                return;
            }
            catch { }

            CurrentContent = CreateFallbackView("Dashboard (brak DashboardView)");
        }

        private void NavigateToSchedule()
        {
            try
            {
                var view = new ScheduleView();
                CurrentContent = view;
                return;
            }
            catch { }

            CurrentContent = CreateFallbackView("Harmonogram (brak ScheduleView)");
        }

        private void NavigateToReservations()
        {
            try
            {
                var view = new ReservationsView();
                CurrentContent = view;
                return;
            }
            catch { }

            CurrentContent = CreateFallbackView("Rezerwacje (brak ReservationsView)");
        }

        private void NavigateToSales()
        {
            try
            {
                var view = new SalesView();
                CurrentContent = view;
                return;
            }
            catch { }

            CurrentContent = CreateFallbackView("Sprzedaż (brak SalesView)");
        }

        private void NavigateToUsers()
        {
            try
            {
                var view = new UsersView();
                CurrentContent = view;
                return;
            }
            catch { }

            CurrentContent = CreateFallbackView("Użytkownicy (brak UsersView)");
        }

        // Helper: tworzy prosty kontroler UI jako fallback (TextBlock)
        private FrameworkElement CreateFallbackView(string message)
        {
            var tb = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 22,
                Margin = new Thickness(10)
            };
            return tb;
        }

        private void LoadMovies()
        {
            if (_db == null)
                return;

            var today = DateTime.Today;

            var filmy = _db.Seanse
                .Include(s => s.Film)
                .Where(s => s.DataCzas.Date == today)
                .Select(s => s.Film)
                .Distinct()
                .ToList();

            DzisiejszeFilmy.Clear();
            foreach (var f in filmy)
                DzisiejszeFilmy.Add(f);
        }

        private void LoadTiles()
        {
            Tiles.Clear();

            Tiles.Add(new TileViewModel("Dzisiejsze seanse",
                _db?.Seanse.Count(s => s.DataCzas.Date == DateTime.Today).ToString() ?? "--",
                "Łącznie w harmonogramie"));

            Tiles.Add(new TileViewModel("Rezerwacje",
                _db?.Rezerwacje.Count().ToString() ?? "--",
                "Aktywne rezerwacje"));

            Tiles.Add(new TileViewModel("Najbliższy seans",
                GetTimeToNearestSeans(),
                "Czas do najbliższego"));

            Tiles.Add(new TileViewModel("Sprzedane bilety",
                _db?.Bilety.Count().ToString() ?? "--",
                "Wszystkie bilety"));
        }

        private string GetTimeToNearestSeans()
        {
            if (_db == null) return "--";

            var now = DateTime.Now;
            var next = _db.Seanse
                .Where(s => s.DataCzas > now)
                .OrderBy(s => s.DataCzas)
                .FirstOrDefault();

            if (next == null)
                return "--";

            var diff = next.DataCzas - now;
            return $"{(int)diff.TotalMinutes} min";
        }
    }

    public class TileViewModel
    {
        public string Naglowek { get; set; }
        public string Wartosc { get; set; }
        public string Opis { get; set; }

        public TileViewModel(string n, string w, string o)
        {
            Naglowek = n;
            Wartosc = w;
            Opis = o;
        }
    }

    // Prosty RelayCommand (jeśli już masz, możesz pominąć - ale upewnij się, że jest identyczny)
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value!;
            remove => CommandManager.RequerySuggested -= value!;
        }
    }
}

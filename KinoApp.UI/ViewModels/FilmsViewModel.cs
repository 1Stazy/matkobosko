using KinoApp.Infrastructure.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KinoApp.UI.ViewModels
{
    public class FilmsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;

        public ObservableCollection<FilmDto> Films { get; set; } = new();

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadFilms();
            }
        }

        public FilmsViewModel(AppDbContext db)
        {
            _db = db;
            LoadFilms();
        }

        private void LoadFilms()
        {
            Films.Clear();

            var query = _db.Films.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(f => f.Tytul.ToLower().Contains(SearchText.ToLower()));

            foreach (var film in query)
            {
                Films.Add(new FilmDto
                {
                    Tytul = film.Tytul,
                    Gatunek = film.Gatunek,
                    PlakatPath = $"Resources/Posters/{film.PlakatPath}"
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // DTO uproszczony pod UI
        public class FilmDto
        {
            public string Tytul { get; set; } ="";
            public string Gatunek { get; set; } = "";
            public string PlakatPath { get; set; } = "";
        }
    }
}

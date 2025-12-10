using KinoApp.Infrastructure.Data;
using KinoApp.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KinoApp.UI.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AppDbContext _db;

        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ICommand LoginCommand { get; }

        public LoginViewModel(AppDbContext db)
        {
            _db = db;
            LoginCommand = new RelayCommand(async _ => await ExecuteLoginAsync());
        }

        private async Task ExecuteLoginAsync()
        {
            // prosty sync check - możesz tu wstawić hashowanie porównań, itp.
            try
            {
                var user = _db.Uzytkownicy.FirstOrDefault(u => u.Login == Login && u.HasloHash == Password);

                if (user == null)
                {
                    MessageBox.Show("Niepoprawny login lub hasło", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // --- SUKCES: Otwieramy okno główne z DI i zamykamy okno logowania ---
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // Pobieramy MainView z DI (musisz wcześniej zarejestrować MainView w App.xaml.cs)
                        var main = App.AppHost?.Services.GetService(typeof(Views.MainView)) as Views.MainView;

                        if (main == null)
                        {
                            // fallback: utwórz ręcznie (ale lepiej zarejestruj w DI)
                            main = new Views.MainView();
                        }

                        // Ustaw main jako główne okno aplikacji i pokaż je
                        Application.Current.MainWindow = main;
                        main.Show();

                        // Znajdź okno logowania i zamknij je (zakładamy typ LoginView)
                        var loginWindow = Application.Current.Windows
                            .OfType<Window>()
                            .FirstOrDefault(w => w.GetType().Name == "LoginView" || w.Title == "Logowanie");

                        // Jeśli nie uda się znaleźć na nazwę, zamykamy aktualne okno główne (jeśli to ono)
                        if (loginWindow != null && loginWindow != main)
                        {
                            loginWindow.Close();
                        }
                        else
                        {
                            // spróbuj zamknąć pierwsze okno, które nie jest main
                            var other = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w != main);
                            other?.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        // w razie błędu pokazujemy info i nie kończymy procesu
                        MessageBox.Show($"Błąd podczas otwierania głównego okna: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd logowania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

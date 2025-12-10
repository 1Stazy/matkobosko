using KinoApp.UI.ViewModels;
using System.Windows;

namespace KinoApp.UI.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _vm;

        // Ten konstruktor zostanie wywołany przez DI — LoginViewModel będzie wstrzyknięty.
        public LoginView(LoginViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = _vm;

            // Podpinamy aktualizowanie pola Hasło z PasswordBox przed wykonaniem komendy.
            // Zakładamy, że LoginCommand w VM bazuje na wartościach Login i Password.
            this.PasswordBox.PasswordChanged += (s, e) =>
            {
                _vm.Password = this.PasswordBox.Password;
            };
        }
    }
}

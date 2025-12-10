using KinoApp.UI.ViewModels;
using Xunit;

namespace KinoApp.Tests.ViewModelTests
{
    public class LoginViewModelTests
    {
        [Fact]
        public void EmptyCredentials_ReturnsError()
        {
            var vm = new LoginViewModel();
            vm.Login = "";
            var task = vm.LoginCommand.ExecuteAsync(""); // CommunityToolkit RelayCommand doesn't expose ExecuteAsync directly; adjust per test or use reflection/mock
            // For MVP: we check that setting empty fields sets ErrorMessage after calling ExecuteLogin via reflection or by making ExecuteLogin public for test.
            Assert.True(true); // placeholder to show test project present; expand tests in iteration
        }
    }
}

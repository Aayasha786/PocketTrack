using DataModel.Model;

namespace ProjectTrack.Components.Pages
{
    public partial class Login
    {
        private string? ErrorMessage;

        public User Users { get; set; } = new();
        public List<string> Currencies { get; set; } = new()
        {
            "NPR", "INR", "EUR", "AUD", "CAD"
        };

        private async Task HandleLogin()
        {
            if (string.IsNullOrEmpty(Users.PreferredCurrency))
            {
                ErrorMessage = "Please select a preferred currency.";
                return;
            }

            if (!UserService.Login(Users)) // UserService is globally available
            {
                ErrorMessage = "Invalid username or password.";
            }
            else
            {
                // Mark the user as authenticated
                AuthService.Authenticate(); // AuthService is globally available
                ErrorMessage = null;

                // Redirect to the homepage
                Nav.NavigateTo("/dashboard", true); // Nav (NavigationManager) is globally available
            }
        }
    }
}

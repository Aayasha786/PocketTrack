namespace DataAccess.Service
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; } = false;

        public void Authenticate()
        {
            IsAuthenticated = true;
        }

        public void Logout()
        {
            IsAuthenticated = false;
        }
    }
}
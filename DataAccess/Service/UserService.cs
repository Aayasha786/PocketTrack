using DataModel.Abstractions;
using DataModel.Model;
using DataAccess.Service.Interface;

namespace ProjectTrack.Service
{
    public class UserService : UserBase, IUserInterface
    {

        private List<User> _users;

        public const string SeedUsername = "admin";
        public const string SeedPassword = "password";

        public UserService()
        {
            //Load the list of users from the JSON file
            _users = LoadUsers();

            //If no users are present, add a default admin user and save to the file.
            if (!_users.Any())
            {
                _users.Add(new User { Username = SeedUsername, Password = SeedPassword });
                SaveUsers(_users);

            }

        }

        //Returns true if the user is authenticated,false otherwise.
        public bool Login(User user)
        {
            //Validate input for null or empty values.
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return false; //Invalid input
            }
            //Check if the username and password match any user in the list.
            return _users.Any(u => u.Username == user.Username && u.Password == user.Password);
            //u => u(lamda equation)
        }
    }
}

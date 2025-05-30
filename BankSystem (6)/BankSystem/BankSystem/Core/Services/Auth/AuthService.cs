using BankSystem.Core.Models.Users;
using BankSystem.Core.Repositories;

namespace BankSystem.Core.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private UserBase? _currentUser;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserBase? Login(string login, string password)
        {
            var user = _userRepository.GetByLogin(login);
            if (user == null || !user.VerifyPassword(password))
                return null;

            _currentUser = user;
            return user;
        }

        public void Logout() => _currentUser = null;
    }
}
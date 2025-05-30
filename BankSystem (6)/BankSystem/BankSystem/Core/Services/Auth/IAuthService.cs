using BankSystem.Core.Models.Users;

namespace BankSystem.Core.Services.Auth
{
    public interface IAuthService
    {
        UserBase? Login(string login, string password);
        void Logout();
    }
}
using BankSystem.Core.Models.Users;
using System.Collections.Generic;

namespace BankSystem.Core.Repositories
{
    public interface IUserRepository
    {
        UserBase? GetByLogin(string login);
        void AddUser(UserBase user);
        void UpdateUser(string login, UserBase newUser);
        IEnumerable<UserBase> GetAllUsers();
    }
}
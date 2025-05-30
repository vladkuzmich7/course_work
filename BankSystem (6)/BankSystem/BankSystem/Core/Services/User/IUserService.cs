using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;
using System;
using System.Collections.Generic;

namespace BankSystem.Core.Services.User
{
    public interface IUserService
    {
        Account CreateAccount(UserBase user, string currency, AccountType type);
        UserBase? GetUser(string login);
        void AddUser(UserBase user);
        void ChangeUserRole(string login);
        IEnumerable<UserBase> GetAllUsers();
        Card CreateCard(UserBase user, string accountNumber);
    }
}
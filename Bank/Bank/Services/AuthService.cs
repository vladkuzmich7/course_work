using BankingLibrary.Models;
using System.Collections.Generic;

namespace BankingLibrary.Services
{
    public class AuthService
    {
        public User Authenticate(string login, string password, List<User> users)
        {
            return users.Find(u => u.Login == login && u.Password == password);
        }

        public bool IsLoginExists(string login, List<User> users)
        {
            return users.Exists(u => u.Login == login);
        }
        public void UpdateUserData(User user, string firstName, string lastName, string email)
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
        }

        public bool ChangePassword(User user, string currentPassword, string newPassword)
        {
            if (user.Password != currentPassword)
                return false;

            user.Password = newPassword;
            return true;
        }
    }
}
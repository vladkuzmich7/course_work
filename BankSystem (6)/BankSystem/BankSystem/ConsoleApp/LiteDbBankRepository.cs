using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;
using BankSystem.ConsoleApp.Menus;

namespace BankSystem.Infrastructure
{
    /// <summary>
    /// LiteDB репозиторий для хранения аккаунтов, пользователей, кредитов, депозитов, транзакций.
    /// </summary>
    /// 
    public class LiteDbBankRepository : IDisposable
    {
        private readonly LiteDatabase _db;

        public LiteDbBankRepository(string dbPath = "BankSystemLite.db")
        {
            _db = new LiteDatabase(dbPath);

            // Индексы и уникальные ключи, если нужно
            _db.GetCollection<Account>("accounts").EnsureIndex(x => x.AccountNumber, true);
            _db.GetCollection<UserBase>("users").EnsureIndex(x => x.Login, true);
            _db.GetCollection<Credit>("credits").EnsureIndex(x => x.AccountNumber, false);
            _db.GetCollection<Deposit>("deposits").EnsureIndex(x => x.Id, true);
            _db.GetCollection<Transaction>("transactions").EnsureIndex(x => x.Id, true);
        }

        // ==== ACCOUNTS ====
        public void SaveAccount(Account account)
        {
            if (string.IsNullOrWhiteSpace(account.AccountNumber))
                throw new ArgumentException("AccountNumber must not be null or empty!");

            _db.GetCollection<Account>("accounts").Upsert(account);
        }
        public class LiteDbUser
        {
            [BsonId]
            public string Login { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public DateTime BirthDate { get; set; }
            public string PassportSeries { get; set; }
            public string PassportNumber { get; set; }
        }
        public class LiteDbUserRepository
        {
            private readonly string _dbPath;

            public LiteDbUserRepository(string dbPath = "BankSystemLite.db")
            {
                _dbPath = dbPath;
            }

            public void AddUser(LiteDbUser user)
            {
                using (var db = new LiteDatabase(_dbPath))
                {
                    var users = db.GetCollection<LiteDbUser>("users");
                    if (users.Exists(u => u.Login == user.Login))
                        throw new Exception("Пользователь с таким логином уже существует!");
                    users.Insert(user);
                }
            }

            public LiteDbUser GetUser(string login)
            {
                using (var db = new LiteDatabase(_dbPath))
                {
                    var users = db.GetCollection<LiteDbUser>("users");
                    return users.FindOne(u => u.Login == login);
                }
            }

            public List<LiteDbUser> GetAllUsers()
            {
                using (var db = new LiteDatabase(_dbPath))
                {
                    var users = db.GetCollection<LiteDbUser>("users");
                    return users.FindAll().ToList();
                }
            }
        }

        public List<Account> GetAllAccounts() =>
            _db.GetCollection<Account>("accounts").FindAll().ToList();

        public Account? GetAccount(string accountNumber) =>
            _db.GetCollection<Account>("accounts").FindOne(a => a.AccountNumber == accountNumber);

        public void DeleteAccount(string accountNumber)
        {
            _db.GetCollection<Account>("accounts").Delete(accountNumber);
        }

        // ==== USERS ====
        public void SaveUser(UserBase user)
        {
            if (string.IsNullOrWhiteSpace(user.Login))
                throw new ArgumentException("Login must not be null or empty!");

            _db.GetCollection<UserBase>("users").Upsert(user);
        }

        public List<UserBase> GetAllUsers() =>
            _db.GetCollection<UserBase>("users").FindAll().ToList();

        public UserBase? GetUser(string login)
        {
            return _db.GetCollection<UserBase>("users").FindOne(u => u.Login == login);
        }

        public void DeleteUser(string login)
        {
            _db.GetCollection<UserBase>("users").Delete(login);
        }

        // ==== CREDITS ====
        public void SaveCredit(Credit credit)
        {
            _db.GetCollection<Credit>("credits").Upsert(credit);
        }

        public List<Credit> GetAllCredits() =>
            _db.GetCollection<Credit>("credits").FindAll().ToList();

        public List<Credit> GetCreditsForAccount(string accountNumber) =>
            _db.GetCollection<Credit>("credits").Find(c => c.AccountNumber == accountNumber).ToList();

        public void DeleteCredit(string accountNumber)
        {
            _db.GetCollection<Credit>("credits").DeleteMany(c => c.AccountNumber == accountNumber);
        }

        // ==== DEPOSITS ====
        public void SaveDeposit(Deposit deposit)
        {
            _db.GetCollection<Deposit>("deposits").Upsert(deposit);
        }

        public List<Deposit> GetAllDeposits() =>
            _db.GetCollection<Deposit>("deposits").FindAll().ToList();

        public List<Deposit> GetDepositsForAccount(string accountNumber) =>
            _db.GetCollection<Deposit>("deposits").Find(d => d.AccountNumber == accountNumber).ToList();

        public void DeleteDeposit(Guid id)
        {
            _db.GetCollection<Deposit>("deposits").Delete(id);
        }

        // ==== TRANSACTIONS ====
        public void SaveTransaction(Transaction transaction)
        {
            _db.GetCollection<Transaction>("transactions").Insert(transaction);
        }

        public List<Transaction> GetAllTransactions() =>
            _db.GetCollection<Transaction>("transactions").FindAll().ToList();

        public List<Transaction> GetTransactionsByCard(string cardNumber) =>
            _db.GetCollection<Transaction>("transactions")
                .Find(t => t.SenderCardNumber == cardNumber || t.ReceiverCardNumber == cardNumber)
                .ToList();

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
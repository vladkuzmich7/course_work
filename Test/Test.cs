using BankingLibrary.Models;
using BankingLibrary.Services;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CardServiceTests
    {
        private CardService _cardService;
        private User _testUser;
        private Account _testAccount;

        [TestInitialize]
        public void Setup()
        {
            _cardService = new CardService();
            _testUser = new User { Login = "testUser" };
            _testAccount = new Account { Id = "acc1", Currency = "RUB", Balance = 1000 };
            _testUser.Accounts.Add(_testAccount);
        }

        [TestMethod]
        public void CreateCard_AddsCardToUser()
        {
            // Act
            var card = _cardService.CreateCard(_testUser, _testAccount);

            // Assert
            Assert.AreEqual(1, _testUser.Cards.Count);
            Assert.AreEqual(card, _testUser.Cards[0]);
            Assert.AreEqual(_testAccount, card.Account);
        }

        [TestMethod]
        public void FindCardByNumber_ReturnsCorrectCard()
        {
            // Arrange
            var card = _cardService.CreateCard(_testUser, _testAccount);
            var users = new List<User> { _testUser };

            // Act
            var foundCard = _cardService.FindCardByNumber(card.Number, users);

            // Assert
            Assert.AreEqual(card, foundCard);
        }

        [TestMethod]
        public void ChangeAccount_UpdatesCardAccount()
        {
            // Arrange
            var card = _cardService.CreateCard(_testUser, _testAccount);
            var newAccount = new Account { Id = "acc2", Currency = "USD" };
            _testUser.Accounts.Add(newAccount);

            // Act
            _cardService.ChangeAccount(card, newAccount);

            // Assert
            Assert.AreEqual(newAccount, card.Account);
            Assert.IsTrue(newAccount.Cards.Contains(card));
            Assert.IsFalse(_testAccount.Cards.Contains(card));
        }
    }
    [TestClass]
    public class TransactionServiceTests
    {
        private TransactionService _transactionService;
        private CardService _cardService;
        private User _user1, _user2;
        private List<User> _users;

        [TestInitialize]
        public void Setup()
        {
            _cardService = new CardService();
            _transactionService = new TransactionService(_cardService);

            _user1 = new User { Login = "user1" };
            _user2 = new User { Login = "user2" };

            var account1 = new Account { Id = "acc1", Currency = "RUB", Balance = 5000 };
            var account2 = new Account { Id = "acc2", Currency = "USD", Balance = 100 };

            _user1.Accounts.Add(account1);
            _user2.Accounts.Add(account2);

            _cardService.CreateCard(_user1, account1);
            _cardService.CreateCard(_user2, account2);

            _users = new List<User> { _user1, _user2 };
        }

        [TestMethod]
        public void CreateCardTransaction_TransfersMoneyBetweenCards()
        {
            // Arrange
            var fromCard = _user1.Cards[0];
            var toCard = _user2.Cards[0];
            decimal amount = 1000;

            // Act
            var transaction = _transactionService.CreateCardTransaction(
                _users,
                fromCard.Number,
                toCard.Number,
                amount);

            // Assert
            Assert.AreEqual(3900, _user1.Accounts[0].Balance); // 5000 - 1000
            Assert.AreEqual(113, _user2.Accounts[0].Balance); // 100 + (1000 * 0.013)
            Assert.AreEqual(amount, transaction.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateCardTransaction_ThrowsWhenNotEnoughFunds()
        {
            // Arrange
            var fromCard = _user1.Cards[0];
            var toCard = _user2.Cards[0];
            decimal amount = 6000; // Больше чем есть на счете

            // Act
            _transactionService.CreateCardTransaction(
                _users,
                fromCard.Number,
                toCard.Number,
                amount);
        }

        [TestMethod]
        public void ShowTransactionsHistory_DisplaysCorrectInfo()
        {
            // Arrange
            var transaction = new Transaction
            {
                Id = "test1",
                FromCard = "1111",
                ToCard = "2222",
                Amount = 100,
                Currency = "RUB",
                Date = DateTime.Now,
                Status = "Completed"
            };
            _user1.Transactions.Add(transaction);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            _transactionService.ShowTransactionsHistory(_user1);

            // Assert
            var output = sw.ToString();
            Assert.IsTrue(output.Contains("1111"));
            Assert.IsTrue(output.Contains("2222"));
            Assert.IsTrue(output.Contains("100 RUB"));
        }
    }
    [TestClass]
    public class AuthServiceTests
    {
        private AuthService _authService;
        private List<User> _users;

        [TestInitialize]
        public void Setup()
        {
            _authService = new AuthService();
            _users = new List<User>
        {
            new User { Login = "user1", Password = "pass1" },
            new User { Login = "user2", Password = "pass2" }
        };
        }

        [TestMethod]
        public void Authenticate_ReturnsUserForCorrectCredentials()
        {
            // Act
            var user = _authService.Authenticate("user1", "pass1", _users);

            // Assert
            Assert.AreEqual(_users[0], user);
        }

        [TestMethod]
        public void Authenticate_ReturnsNullForWrongPassword()
        {
            // Act
            var user = _authService.Authenticate("user1", "wrongpass", _users);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public void IsLoginExists_DetectsExistingLogin()
        {
            // Act
            bool exists = _authService.IsLoginExists("user1", _users);

            // Assert
            Assert.IsTrue(exists);
        }
    }
    [TestClass]
    public class CreditServiceTests
    {
        private CreditService _creditService;
        private User _user;

        [TestInitialize]
        public void Setup()
        {
            _creditService = new CreditService();
            _user = new User();
            _user.Accounts.Add(new Account { Id = "acc1", Currency = "RUB", Balance = 0 });
        }

        [TestMethod]
        public void TakeCredit_AddsMoneyToAccount()
        {
            // Arrange
            decimal amount = 10000;
            decimal percent = 10;
            int months = 12;

            // Act
            var credit = _creditService.TakeCredit(_user, "acc1", amount, percent, months);

            // Assert
            Assert.AreEqual(amount, _user.Accounts[0].Balance);
            Assert.AreEqual(1, _user.Credits.Count);
            Assert.AreEqual(11000, credit.TotalLoan); // 10000 + 10%
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TakeCredit_ThrowsForInvalidAccount()
        {
            // Act
            _creditService.TakeCredit(_user, "invalid_acc", 10000, 10, 12);
        }
    }
    [TestClass]
    public class ProgramIntegrationTests
    {
        [TestMethod]
        public void FullUserFlow_RegistersLogsInAndMakesTransaction()
        {
            // Arrange
            var users = new List<User>();
            var authService = new AuthService();

            // Регистрация
            var newUser = new User
            {
                Login = "testuser",
                Password = "testpass",
                FirstName = "Test",
                LastName = "User"
            };
            users.Add(newUser);

            // Создание счета и карты
            var accountService = new AccountService();
            var account = accountService.CreateAccount(newUser, "RUB");
            var cardService = new CardService();
            var card = cardService.CreateCard(newUser, account);

            // Act - попытка входа
            var loggedInUser = authService.Authenticate("testuser", "testpass", users);

            // Assert
            Assert.IsNotNull(loggedInUser);
            Assert.AreEqual(1, loggedInUser.Cards.Count);
            Assert.AreEqual("RUB", loggedInUser.Accounts[0].Currency);
        }
    }
}

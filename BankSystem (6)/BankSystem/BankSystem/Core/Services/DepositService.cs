using System;
using System.Collections.Generic;
using System.Linq;
using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;
using BankSystem.Core.Repositories;
using BankSystem.Core.Notifications;

public interface IObserver
{
    void Update(string message);
}

namespace BankSystem.Core.Services
{
    public class DepositService
    {
        private readonly List<IObserver> _observers = new();

        private readonly DepositRepository _repository;
        public IEnumerable<Deposit> GetDepositsForUser(UserBase user)
        {
            var userAccountNumbers = user.Accounts.Select(a => a.AccountNumber).ToHashSet();
            return _repository.GetAll().Where(d => userAccountNumbers.Contains(d.AccountNumber));
        }
        public DepositService(DepositRepository repository)
        {
            _repository = repository;
        }
        public void RegisterObserver(IObserver observer)
        {
            _observers.Add(observer);
        }


        // Добавьте этот приватный метод!
        private void NotifyObservers(string message)
        {
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
        }

        public Deposit CreateDeposit(string accountNumber, decimal amount, decimal rate, string currency)
        {
            var deposit = new Deposit(accountNumber, amount, rate, currency, true);
            { // Вклад сначала ожидает подтверждения админом
            };
            _repository.Add(deposit);
            // После создания вклада:
            NotifyObservers($"Новый вклад ожидает подтверждения: пользователь {accountNumber}, сумма {amount} {currency}");
            return deposit;
        }

        public IEnumerable<Deposit> GetDeposits(string accountNumber) =>
            _repository.GetByAccount(accountNumber);

        public void AccumulateInterest(Guid depositId, int months = 1)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            deposit.AccumulateInterest(months);
        }

        public void Withdraw(Guid depositId, decimal amount)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            deposit.Withdraw(amount);
        }

        public decimal TransferAndClose(Guid depositId)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            return deposit.TransferAndClose();
        }

        public void Block(Guid depositId)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            // Здесь можно реализовать логику блокировки вклада
        }

        public void Unblock(Guid depositId)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            // Здесь можно реализовать логику разблокировки вклада
        }

        public void Freeze(Guid depositId)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            // Здесь можно реализовать логику заморозки вклада
        }

        public void Unfreeze(Guid depositId)
        {
            var deposit = _repository.GetById(depositId)
                ?? throw new InvalidOperationException("Вклад не найден");
            // Здесь можно реализовать логику разморозки вклада
        }

        public IEnumerable<Deposit> GetAll() => _repository.GetAll();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using BankSystem.Core.Models;

namespace BankSystem.Core.Repositories
{
    public class DepositRepository
    {
        private readonly List<Deposit> _deposits = new();

        public void Add(Deposit deposit) => _deposits.Add(deposit);

        public Deposit? GetById(Guid id) => _deposits.FirstOrDefault(d => d.Id == id);

        public IEnumerable<Deposit> GetByAccount(string accountNumber) =>
            _deposits.Where(d => d.AccountNumber == accountNumber);

        public IEnumerable<Deposit> GetAll() => _deposits;
    }
}
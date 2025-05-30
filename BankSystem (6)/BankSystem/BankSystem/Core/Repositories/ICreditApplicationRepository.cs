using System;
using System.Collections.Generic;
using BankSystem.Core.Models;

namespace BankSystem.Core.Repositories
{
    public interface ICreditApplicationRepository
    {
        void Add(CreditApplication application);
        CreditApplication? GetById(Guid id);
        IEnumerable<CreditApplication> GetAll();
        void Update(CreditApplication application);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using BankSystem.Core.Models;

namespace BankSystem.Core.Repositories
{
    public class CreditApplicationRepository : ICreditApplicationRepository
    {
        private readonly List<CreditApplication> _applications = new();

        public void Add(CreditApplication application) => _applications.Add(application);

        public CreditApplication? GetById(Guid id) => _applications.FirstOrDefault(x => x.Id == id);

        public IEnumerable<CreditApplication> GetAll() => _applications.ToList();

        public void Update(CreditApplication application)
        {
            var idx = _applications.FindIndex(a => a.Id == application.Id);
            if (idx >= 0)
                _applications[idx] = application;
        }
    }
}
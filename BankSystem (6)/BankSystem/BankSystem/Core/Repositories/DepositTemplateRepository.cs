using System;
using System.Collections.Generic;
using System.Linq;
using BankSystem.Core.Models;

namespace BankSystem.Core.Repositories
{
    public class DepositTemplateRepository
    {
        private readonly List<DepositTemplate> _templates = new();

        public void Add(DepositTemplate template) => _templates.Add(template);
        public IEnumerable<DepositTemplate> GetAll() => _templates;
        public DepositTemplate? GetById(Guid id) => _templates.FirstOrDefault(t => t.Id == id);
    }
}
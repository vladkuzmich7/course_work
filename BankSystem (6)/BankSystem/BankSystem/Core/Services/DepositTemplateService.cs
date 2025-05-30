using System;
using System.Collections.Generic;
using BankSystem.Core.Models;
using BankSystem.Core.Repositories;

namespace BankSystem.Core.Services
{
    public class DepositTemplateService
    {
        private readonly DepositTemplateRepository _repo;

        public DepositTemplateService(DepositTemplateRepository repo)
        {
            _repo = repo;
        }

        public void AddTemplate(DepositTemplate template) => _repo.Add(template);
        public IEnumerable<DepositTemplate> GetAllTemplates() => _repo.GetAll();
        public DepositTemplate? GetById(Guid id) => _repo.GetById(id);
    }
}
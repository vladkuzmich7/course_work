using System.Collections.Generic;
using BankSystem.Core.Models;
using BankSystem.Core.Repositories;

namespace BankSystem.Core.Services
{
    public class CreditTypeService : ICreditTypeService
    {
        private readonly ICreditTypeRepository _repository;

        public CreditTypeService(ICreditTypeRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<CreditType> GetAllCreditTypes() => _repository.GetAll();

        public void AddCreditType(CreditType type) => _repository.Add(type);
    }
}
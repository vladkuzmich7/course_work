using System.Collections.Generic;
using BankSystem.Core.Models;

namespace BankSystem.Core.Repositories
{
    public class CreditTypeRepository : ICreditTypeRepository
    {
        private readonly List<CreditType> _types = new();

        public IEnumerable<CreditType> GetAll() => _types;

        public void Add(CreditType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            _types.Add(type);
        }
    }
}
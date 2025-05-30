using System.Collections.Generic;
using BankSystem.Core.Models;

namespace BankSystem.Core.Repositories
{
    public interface ICreditTypeRepository
    {
        IEnumerable<CreditType> GetAll();
        void Add(CreditType type);
    }
}
using System.Collections.Generic;
using BankSystem.Core.Models;

namespace BankSystem.Core.Services
{
    public interface ICreditTypeService
    {
        IEnumerable<CreditType> GetAllCreditTypes();
        void AddCreditType(CreditType type);
    }
}
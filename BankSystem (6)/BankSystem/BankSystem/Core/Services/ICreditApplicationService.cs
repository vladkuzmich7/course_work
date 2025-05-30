using System;
using System.Collections.Generic;
using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;

namespace BankSystem.Core.Services
{
    public interface ICreditApplicationService
    {
        void SubmitApplication(CreditApplication application);
        IEnumerable<CreditApplication> GetAllApplications();
        IEnumerable<CreditApplication> GetApplicationsByStatus(CreditApplicationStatus status);
        IEnumerable<CreditApplication> GetApplicationsByUser(UserBase user);
        void ApproveApplication(Guid id, UserBase approver);
        void RejectApplication(Guid id, UserBase approver);
    }
}
using System;
using BankSystem.Core.Models.Users;

namespace BankSystem.Core.Models
{
    public enum CreditApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class CreditApplication
    {
        public Guid Id { get; }
        public UserBase Applicant { get; }
        public CreditType CreditType { get; }
        public string AccountNumber { get; }
        public decimal Sum { get; }
        public int Months { get; }
        public CreditApplicationStatus Status { get; private set; }
        public UserBase? ApprovedBy { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? DecisionAt { get; private set; }

        public CreditApplication(UserBase applicant, CreditType creditType, string accountNumber, decimal sum, int months)
        {
            Id = Guid.NewGuid();
            Applicant = applicant ?? throw new ArgumentNullException(nameof(applicant));
            CreditType = creditType ?? throw new ArgumentNullException(nameof(creditType));
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Sum = sum;
            Months = months;
            Status = CreditApplicationStatus.Pending;
            CreatedAt = DateTime.Now;
        }

        public void Approve(UserBase approver)
        {
            if (Status != CreditApplicationStatus.Pending)
                throw new InvalidOperationException("Заявка уже рассмотрена.");
            Status = CreditApplicationStatus.Approved;
            ApprovedBy = approver;
            DecisionAt = DateTime.Now;
        }

        public void Reject(UserBase approver)
        {
            if (Status != CreditApplicationStatus.Pending)
                throw new InvalidOperationException("Заявка уже рассмотрена.");
            Status = CreditApplicationStatus.Rejected;
            ApprovedBy = approver;
            DecisionAt = DateTime.Now;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using BankSystem.Core.Models;
using BankSystem.Core.Notifications; // Добавить пространство имён
using BankSystem.Core.Models.Users;
using BankSystem.Core.Repositories;

namespace BankSystem.Core.Services
{
    public class CreditApplicationService : ICreditApplicationService, INotificationObservable
    {
        private readonly ICreditApplicationRepository _repository;
        private readonly List<INotificationObserver> _observers = new();

        public CreditApplicationService(ICreditApplicationRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<CreditApplication> GetAllApplications() => _repository.GetAll();
        public void RegisterObserver(INotificationObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void RemoveObserver(INotificationObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers(string message)
        {
            foreach (var observer in _observers)
                observer.Update(message);
        }

        public void SubmitApplication(CreditApplication application)
        {
            _repository.Add(application);
            NotifyObservers($"Поступила новая заявка на кредит от {application.Applicant.Login} на сумму {application.Sum} {application.CreditType.Currency}");
        }
        public IEnumerable<CreditApplication> GetApplicationsByStatus(CreditApplicationStatus status)
            => _repository.GetAll().Where(a => a.Status == status);

        public IEnumerable<CreditApplication> GetApplicationsByUser(UserBase user)
            => _repository.GetAll().Where(a => a.Applicant.Login == user.Login);

        public void ApproveApplication(Guid id, UserBase approver)
        {
            var app = _repository.GetById(id) ?? throw new KeyNotFoundException();
            app.Approve(approver);
            _repository.Update(app);

            // Credit создается только после одобрения!
            var credit = new Credit(app.AccountNumber, app.Sum, app.CreditType.DefaultPercent, app.Months);
            app.Applicant.Credits.Add(credit);

            // Закидываем деньги на счет:
            var account = app.Applicant.Accounts.FirstOrDefault(a => a.AccountNumber == app.AccountNumber);
            if (account != null)
                account.Balance += app.Sum;
        }

        public void RejectApplication(Guid id, UserBase approver)
        {
            var app = _repository.GetById(id) ?? throw new KeyNotFoundException();
            app.Reject(approver);
            _repository.Update(app);
        }
    }
}
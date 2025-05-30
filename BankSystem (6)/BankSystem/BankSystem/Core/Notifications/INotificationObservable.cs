namespace BankSystem.Core.Notifications
{
    public interface INotificationObservable
    {
        void RegisterObserver(INotificationObserver observer);
        void RemoveObserver(INotificationObserver observer);
        void NotifyObservers(string message);
    }
}
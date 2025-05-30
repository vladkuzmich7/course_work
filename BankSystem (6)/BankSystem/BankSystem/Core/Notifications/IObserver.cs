namespace BankSystem.Core.Notifications
{

    public interface INotificationObserver
    {
        void Update(string message);
    }
}
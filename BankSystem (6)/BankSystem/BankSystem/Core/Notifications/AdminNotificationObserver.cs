using BankSystem.Core.Notifications;
using BankSystem.Core.Models.Users;

public class AdminNotificationObserver : INotificationObserver
{
    private readonly AdminUser _admin;
    public AdminNotificationObserver(AdminUser admin)
    {
        _admin = admin;
    }

    public void Update(string message)
    {
        _admin.AddNotification(new Notification(message));
        // Можно оставить Console.WriteLine для мгновенного вывода, если хотите
    }
}
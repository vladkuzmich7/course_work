namespace BankSystem.Core.Models.Users
{
    public class AdminUser : UserBase
    {
        public AdminUser(string login, string password, string firstName, string lastName,
                       string email, string phone, DateTime birthDate,
                       string passportSeries, string passportNumber)
            : base(login, password, firstName, lastName, email, phone,
                  birthDate, passportSeries, passportNumber)
        { }

        public override string RoleName => "Admin";

        public override void DisplayPermissions()
        {
            Console.WriteLine("Администратор может:");
            Console.WriteLine("- Управлять всеми пользователями");
            Console.WriteLine("- Назначать роли");
            Console.WriteLine("- Изменять системные настройки");
        }
        private readonly List<Notification> _notifications = new();
        public IEnumerable<Notification> Notifications => _notifications;

        public void AddNotification(Notification notification)
            => _notifications.Add(notification);

        public void ShowNotifications()
        {
            Console.Clear();
            Console.WriteLine("=== Уведомления администратора ===");
            if (_notifications.Count == 0)
                Console.WriteLine("Нет уведомлений.");
            else
                foreach (var n in _notifications.OrderByDescending(x => x.Date))
                    Console.WriteLine(n);
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }
}
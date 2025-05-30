namespace BankSystem.Core.Models.Users
{
    public class EmployeeUser : UserBase
    {
        public EmployeeUser(string login, string password, string firstName, string lastName,
                          string email, string phone, DateTime birthDate,
                          string passportSeries, string passportNumber)
            : base(login, password, firstName, lastName, email, phone,
                  birthDate, passportSeries, passportNumber)
        { }

        public override string RoleName => "Employee";

        public override void DisplayPermissions()
        {
            Console.WriteLine("Сотрудник может:");
            Console.WriteLine("- Просматривать все счета");
            Console.WriteLine("- Одобрять кредитные заявки");
            Console.WriteLine("- Редактировать данные пользователей");
        }
    }
}
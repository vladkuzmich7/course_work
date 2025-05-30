namespace BankSystem.Core.Models.Users
{
    public class RegularUser : UserBase
    {
        public RegularUser(string login, string password, string firstName, string lastName,
                         string email, string phone, DateTime birthDate,
                         string passportSeries, string passportNumber)
            : base(login, password, firstName, lastName, email, phone,
                  birthDate, passportSeries, passportNumber)
        { }

        public override string RoleName => "RegularUser";

        public override void DisplayPermissions()
        {
            Console.WriteLine("Обычный пользователь может:");
            Console.WriteLine("- Просматривать свои счета");
            Console.WriteLine("- Совершать переводы");
            Console.WriteLine("- Подавать заявки на кредиты");
        }
    }
}
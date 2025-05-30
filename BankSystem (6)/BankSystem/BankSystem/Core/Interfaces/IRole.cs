namespace BankSystem.Core.Interfaces
{
    public interface IRole
    {
        string RoleName { get; }
        void DisplayPermissions();
    }
}
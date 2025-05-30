public class Notification
{
    public string Message { get; }
    public DateTime Date { get; }

    public Notification(string message)
    {
        Message = message;
        Date = DateTime.Now;
    }

    public override string ToString() => $"[{Date:dd.MM.yyyy HH:mm}] {Message}";
}
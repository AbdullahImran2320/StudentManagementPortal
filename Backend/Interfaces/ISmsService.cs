namespace StudentAPI.Interfaces
{
    public interface ISmsService
    {
        Task SendWelcomeSmsAsync(string toPhoneNumber, string userName);
    }
}
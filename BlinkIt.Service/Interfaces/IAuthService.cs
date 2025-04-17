namespace BlinkIt.Service.Interfaces;

public interface IAuthService
{
    public Task<(bool Success, string Message, string Token)> CreateNewUser(string mobileNumber, string password);
}
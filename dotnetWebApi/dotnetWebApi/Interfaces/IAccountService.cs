using dotnetWebApi.Entities;

namespace dotnetWebApi.Interfaces;

public interface IAccountService
{
    Task<(bool success, string message)> RegisterAsync(string userName, string firstName, string password);
    Task<(bool success, string message, User? user)> LoginAsync(string userName, string password);
}
using dotnetWebApi.Entities;
using dotnetWebApi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace dotnetWebApi.Services;

public class AccountService(AccountRepository accountRepository)
{
    public async Task<(bool success, string message)> RegisterAsync(string userName, string firstName, string password)
    {
        var existingUser = await accountRepository.GetByUserName(userName);
        if (existingUser != null) return (false, "User with such username is already exists");
        var user = new User
        {
            UserName = userName,
            FirstName = firstName,
            Role = "Visitor"
        };
        var passHash = new PasswordHasher<User>().HashPassword(user, password);
        user.PasswordHash = passHash;
        await accountRepository.AddAsync(user);
        return (true, "User is registered");
    }

    public async Task<(bool success, string message, User? user)> LoginAsync(string userName, string password)
    {
        var user = await accountRepository.GetByUserName(userName);
        if (user == null) return (false, "Invalid username or/and password", null);
        var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password);
        return passwordVerification == PasswordVerificationResult.Success
            ? (true, "Login succeed", user)
            : (false, "Invalid username or/and password", null);
    }
}
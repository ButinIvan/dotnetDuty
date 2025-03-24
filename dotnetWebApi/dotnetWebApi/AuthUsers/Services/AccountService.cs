using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace dotnetWebApi.AuthUsers.Services;

public class AccountService(IAccountRepository accountRepository)
{
    public async Task<(bool success, string message)> RegisterAsync(string userName, string firstName, string password)
    {
        // Full user entity is fetched. .Any(x => x...) would be enough
        var existingUser = await accountRepository.GetByUserNameAsync(userName);
        if (existingUser != null) return (false, "User with such username is already exists");
        var user = new User(userName, firstName, "User");
        // Could've used DI (dependency injection) here
        var passHash = new PasswordHasher<User>().HashPassword(user, password);
        user.PasswordHash = passHash;
        await accountRepository.AddAsync(user);
        return (true, "User is registered");
    }

    // Nice application of railway programming, but I'd encapsulate it all (isSuccess, msg, data) in a "Result" object
    public async Task<(bool success, string message, User? user)> LoginAsync(string userName, string password)
    {
        var user = await accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, "Invalid username or/and password", null);
        // Also could've used DI (dependency injection) here
        var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password);
        return passwordVerification == PasswordVerificationResult.Success
            ? (true, "Login succeed", user)
            : (false, "Invalid username or/and password", null);
    }
}
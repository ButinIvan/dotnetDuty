using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace dotnetWebApi.AuthUsers.Services;

public class PasswordHashService :IPasswordHasher
{
    public string HashPassword(User user, string password)
    {
        return new PasswordHasher<User>().HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string password)
    {
        return new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, password);
    }

    public async Task<(bool success, string message, User? user)> VerifyResultStatus(PasswordVerificationResult result,
        User user)
    {
        return result == PasswordVerificationResult.Success
            ? (true, "Login succeed", user)
            : (false, "Invalid username or/and password", null);
    }
}
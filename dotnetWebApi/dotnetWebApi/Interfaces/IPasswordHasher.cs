using dotnetWebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace dotnetWebApi.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(User user, string password);
    PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string password);

    Task<(bool success, string message, User? user)> VerifyResultStatus(PasswordVerificationResult result,
        User user);
}
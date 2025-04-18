﻿using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;

namespace dotnetWebApi.AuthUsers.Services;

public class AccountService(IAccountRepository accountRepository, IPasswordHasher hasher) :IAccountService
{
    public async Task<(bool success, string message)> RegisterAsync(string userName, string firstName, string password)
    {
        var existingUser = await accountRepository.GetByUserNameAsync(userName);
        if (existingUser != null) return (false, "User with such username is already exists");
        var user = new User(userName, firstName, "User");
        var passHash = hasher.HashPassword(user, password);
        user.PasswordHash = passHash;
        await accountRepository.AddAsync(user);
        return (true, "User is registered");
    }

    public async Task<(bool success, string message, User? user)> LoginAsync(string userName, string password)
    {
        var user = await accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, "Invalid username or/and password", null);
        var passwordVerification = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return await hasher.VerifyResultStatus(passwordVerification, user);
    }
}
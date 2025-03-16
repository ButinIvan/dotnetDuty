using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using dotnetWebApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace dotnetWebApi.AuthUsers.Repositories;

public class AccountRepository(ApplicationDbContext dbContext) : IAccountRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}
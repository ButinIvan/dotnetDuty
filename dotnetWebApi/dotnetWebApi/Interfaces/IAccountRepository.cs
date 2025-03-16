using dotnetWebApi.Entities;

namespace dotnetWebApi.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(User user);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
}
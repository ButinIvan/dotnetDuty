using dotnetWebApi.Entities;

namespace dotnetWebApi.AuthUsers.Repositories;

public class AccountRepository
{
    private static IDictionary<string?, User> _users = new Dictionary<string?, User>();

    public async Task AddAsync(User user)
    {
        _users[user.UserName] = user;
    }

    public async Task<User?> GetByUserName(string userName)
    {
        return _users.TryGetValue(userName, out var user) ? user : null;
    }
}
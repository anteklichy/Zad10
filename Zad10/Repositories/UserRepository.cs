using Microsoft.EntityFrameworkCore;
using Zad10.Context;
using Zad10.Models;

namespace Zad10.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByLogin(string login)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Login == login);
    }

    public async Task SaveRefreshToken(int userId, string refreshToken)
    {
        var user = await _context.Users.FindAsync(userId);
        user.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByRefreshToken(string refreshToken)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}

public interface IUserRepository
{
    Task AddUser(User user);
    Task<User> GetUserByLogin(string login);
    Task SaveRefreshToken(int userId, string refreshToken);
    Task<User> GetUserByRefreshToken(string refreshToken);
}
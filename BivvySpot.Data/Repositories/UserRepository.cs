using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BivvySpot.Data.Repositories;

public class UserRepository(BivvySpotContext dbContext) : IUserRepository
{
    public Task<User?> FindByIdentityAsync(string provider, string subject, CancellationToken ct) =>
        dbContext.Users.SingleOrDefaultAsync(u =>
            u.AuthProvider == provider &&
            u.AuthSubject == subject &&
            u.DeletedDate == null, ct);

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        dbContext.Users.SingleOrDefaultAsync(u => u.Email == email && u.DeletedDate == null, ct);

    public Task AddAsync(User user, CancellationToken ct)
    {
        dbContext.Users.Add(user);
        return Task.CompletedTask;
    }
    
    public Task<bool> UsernameExistsAsync(string username, CancellationToken ct) =>
        dbContext.Users.AnyAsync(u => u.Username == username && u.DeletedDate == null, ct);

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}
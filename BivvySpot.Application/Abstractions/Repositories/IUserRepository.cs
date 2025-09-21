using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> FindByIdentityAsync(string provider, string subject, CancellationToken ct);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
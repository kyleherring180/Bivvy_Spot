using BivvySpot.Application.Abstractions.Repositories;
using BivvySpot.Application.Abstractions.Services;
using BivvySpot.Model.Dtos;
using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Services;

public class AccountService(IUserRepository userRepository) : IAccountService
{
    public async Task<AccountProfileResponse> RegisterOrUpsertAsync(AuthContext auth, CancellationToken ct)
    {
        var user = await ResolveUserAsync(auth, ct)
                   ?? await CreateUserAsync(auth, ct);

        // Light refresh
        user.UpdateProfile(auth.DisplayName, null, null);
        if (!string.IsNullOrWhiteSpace(auth.Email)) user.UpdateEmail(auth.Email!);
        if (!string.IsNullOrWhiteSpace(auth.Provider) && !string.IsNullOrWhiteSpace(auth.Subject))
            user.LinkIdentity(auth.Provider!, auth.Subject!);

        await userRepository.SaveChangesAsync(ct);
        return ToResponse(user);
    }

    public async Task<AccountProfileResponse?> GetCurrentProfileAsync(AuthContext auth, CancellationToken ct)
    {
        var user = await ResolveUserAsync(auth, ct);
        return user is null ? null : ToResponse(user);
    }

    public async Task UpdateCurrentProfileAsync(AuthContext auth, UpdateAccountProfileRequest req, CancellationToken ct)
    {
        var user = await ResolveUserAsync(auth, ct) ?? throw new KeyNotFoundException();
        user.UpdateProfile(req.Username, req.FirstName, req.LastName);
        await userRepository.SaveChangesAsync(ct);
    }

    private async Task<User?> ResolveUserAsync(AuthContext auth, CancellationToken ct)
        => (!string.IsNullOrWhiteSpace(auth.Provider) && !string.IsNullOrWhiteSpace(auth.Subject))
            ? await userRepository.FindByIdentityAsync(auth.Provider!, auth.Subject!, ct)
            : (!string.IsNullOrWhiteSpace(auth.Email) ? await userRepository.FindByEmailAsync(auth.Email!, ct) : null);

    private async Task<User> CreateUserAsync(AuthContext auth, CancellationToken ct)
    {
        var user = new User(auth.DisplayName ?? $"user-{Guid.NewGuid():N}", "", "", auth.Email ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(auth.Provider) && !string.IsNullOrWhiteSpace(auth.Subject))
            user.LinkIdentity(auth.Provider!, auth.Subject!);
        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);
        return user;
    }

    private static AccountProfileResponse ToResponse(User u)
        => new(u.Id, u.Username, u.Email, u.FirstName, u.LastName);
}
using BivvySpot.Model.Dtos;

namespace BivvySpot.Application.Abstractions.Services;

public interface IAccountService
{
    Task<AccountProfileResponse> RegisterOrUpsertAsync(AuthContext auth, CancellationToken ct);
    Task<AccountProfileResponse?> GetCurrentProfileAsync(AuthContext auth, CancellationToken ct);
    Task UpdateCurrentProfileAsync(AuthContext auth, UpdateAccountProfileRequest req, CancellationToken ct);
}
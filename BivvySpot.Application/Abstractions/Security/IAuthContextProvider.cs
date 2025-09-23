using BivvySpot.Model.Dtos;

namespace BivvySpot.Application.Abstractions.Security;

public interface IAuthContextProvider
{
    AuthContext GetCurrent();
}
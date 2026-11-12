using ApiContracts;

namespace Blazor_App.Services;

public interface IUserService
{
    Task<UserDto> AddUserAsync(CreateUserDto request);
}
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        VerifyUserNameIsAvailable(request.UserName, null);

        User user = new() { UserName = request.UserName, Password = request.Password };
        User created = await userRepo.AddAsync(user);

        UserDto dto = ToDto(created);
        return Created($"/users/{dto.Id}", dto);
    }

    // PUT /users/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        User existing = await userRepo.GetSingleAsync(id);   // throws NotFoundException -> 404
        VerifyUserNameIsAvailable(request.UserName, id);

        existing.UserName = request.UserName;
        existing.Password = request.Password;
        await userRepo.UpdateAsync(existing);

        return NoContent();
    }

    // GET /users/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        User user = await userRepo.GetSingleAsync(id);
        return Ok(ToDto(user));
    }

    // GET /users?userNameContains=an
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? userNameContains)
    {
        IQueryable<User> users = userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(userNameContains))
        {
            users = users.Where(u =>
                u.UserName.Contains(userNameContains, StringComparison.OrdinalIgnoreCase));
        }

        List<UserDto> result = users.Select(u => ToDto(u)).ToList();
        return Ok(result);
    }

    // DELETE /users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        await userRepo.GetSingleAsync(id);   // 404 if it doesn't exist
        await userRepo.DeleteAsync(id);
        return NoContent();
    }

    private void VerifyUserNameIsAvailable(string userName, int? ignoreId)
    {
        bool taken = userRepo.GetMany().Any(u =>
            u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && u.Id != ignoreId);

        if (taken)
        {
            throw new ArgumentException($"Username '{userName}' is already taken.");
        }
    }

    private static UserDto ToDto(User u) => new() { Id = u.Id, UserName = u.UserName };
}
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /auth/login
    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequestDto request)
    {
        User? user = userRepo.GetMany()
            .FirstOrDefault(u =>
                u.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase)
                && u.Password == request.Password);

        if (user is null)
        {
            return Unauthorized("invalid username or password");
        }

        return Ok(new UserDto { Id = user.Id, UserName = user.UserName });
    }
}
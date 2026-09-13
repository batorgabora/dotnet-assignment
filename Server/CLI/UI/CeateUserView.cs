using Entities;
using RepositoryContracts;

namespace CLI;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.Write("username: ");
        string username = Console.ReadLine() ?? "";

        bool usernameTaken = userRepository.GetMany().Any(u => u.UserName == username);
        if (usernameTaken)
        {
            Console.WriteLine("username is already taken.");
            return;
        }

        Console.Write("password: ");
        string password = Console.ReadLine() ?? "";

        User user = new User
        {
            UserName = username,
            Password = password
        };

        User created = await userRepository.AddAsync(user);
        Console.WriteLine($"created user with id {created.Id}");
    }
}
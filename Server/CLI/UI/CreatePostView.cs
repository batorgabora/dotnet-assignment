using Entities;
using RepositoryContracts;

namespace CLI;

public class CreatePostView
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public CreatePostView(IUserRepository userRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    public async Task RunAsync()
    {
        Console.Write("title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("body: ");
        string body = Console.ReadLine() ?? "";

        Console.Write("user id: ");
        int userId = int.Parse(Console.ReadLine() ?? "0");

        bool userExists = userRepository.GetMany().Any(u => u.Id == userId);
        if (!userExists)
        {
            Console.WriteLine($"no user with id {userId} exists.");
            return;
        }

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post created = await postRepository.AddAsync(post);
        Console.WriteLine($"created post with id {created.Id}");
    }
}
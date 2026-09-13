using Entities;
using RepositoryContracts;

namespace CLI;

public class AddCommentView
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public AddCommentView(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        Console.Write("post id: ");
        int postId = int.Parse(Console.ReadLine() ?? "0");

        bool postExists = postRepository.GetMany().Any(p => p.Id == postId);
        if (!postExists)
        {
            Console.WriteLine($"no post with id {postId} exists.");
            return;
        }

        Console.Write("user id: ");
        int userId = int.Parse(Console.ReadLine() ?? "0");

        bool userExists = userRepository.GetMany().Any(u => u.Id == userId);
        if (!userExists)
        {
            Console.WriteLine($"no user with id {userId} exists.");
            return;
        }

        Console.Write("body: ");
        string body = Console.ReadLine() ?? "";

        Comment comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Body = body
        };

        Comment created = await commentRepository.AddAsync(comment);
        Console.WriteLine($"created comment with id {created.Id}");
    }
}
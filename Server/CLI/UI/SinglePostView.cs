using Entities;
using RepositoryContracts;

namespace CLI;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        Console.Write("post id: ");
        int postId = int.Parse(Console.ReadLine() ?? "0");

        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("post not found.");
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"---  {post.Title}  ---");
        Console.WriteLine($"'{post.Body}'");

        var comments = commentRepository.GetMany().Where(c => c.PostId == postId);
        foreach (Comment comment in comments)
        {
            Console.WriteLine($"\t- {comment.Body}  (by {comment.UserId})");
        }
        
        Console.ResetColor();
    }
}
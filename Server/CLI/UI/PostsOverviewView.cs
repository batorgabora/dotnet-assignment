using Entities;
using RepositoryContracts;

namespace CLI;

public class PostsOverviewView
{
    private readonly IPostRepository postRepository;

    public PostsOverviewView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public void Run()
    {
        Console.WriteLine();
        Console.WriteLine("=== posts ===");
        foreach (Post post in postRepository.GetMany())
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }
}
using Entities;
using RepositoryContracts;
using InMemoryRepositories;

namespace CLI;

public class Program
{
    public static async Task Main(string[] args)
    {
        IUserRepository userRepository = new UserInMemoryRepository();
        IPostRepository postRepository = new PostInMemoryRepository();
        ICommentRepository commentRepository = new CommentInMemoryRepository();

        CliApp app = new CliApp(userRepository, postRepository, commentRepository);
        await app.RunAsync();
    }
}
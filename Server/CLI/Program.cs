using Entities;
using RepositoryContracts;
using FileRepositories;

namespace CLI;

public class Program
{
    public static async Task Main(string[] args)
    {
        IUserRepository userRepository = new UserFileRepository();
        IPostRepository postRepository = new PostFileRepository();
        ICommentRepository commentRepository = new CommentFileRepository();

        CliApp app = new CliApp(userRepository, postRepository, commentRepository);
        await app.RunAsync();
    }
}
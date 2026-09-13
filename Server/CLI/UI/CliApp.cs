using RepositoryContracts;

namespace CLI;

public class CliApp
{
    private readonly CreateUserView createUserView;
    private readonly CreatePostView createPostView;
    private readonly AddCommentView addCommentView;
    private readonly PostsOverviewView postsOverviewView;
    private readonly SinglePostView singlePostView;

    public CliApp(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        createUserView = new CreateUserView(userRepository);
        createPostView = new CreatePostView(userRepository, postRepository);
        addCommentView = new AddCommentView(userRepository, postRepository, commentRepository);
        postsOverviewView = new PostsOverviewView(postRepository);
        singlePostView = new SinglePostView(postRepository, commentRepository);
    }

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("==== chatter ====");
            Console.ResetColor();
            Console.WriteLine("1. create user");
            Console.WriteLine("2. create post");
            Console.WriteLine("3. add comment to post");
            Console.WriteLine("4. view posts overview");
            Console.WriteLine("5. view single post");
            Console.WriteLine("0. exit");
            Console.Write("choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await createUserView.RunAsync();
                    break;
                case "2":
                    await createPostView.RunAsync();
                    break;
                case "3":
                    await addCommentView.RunAsync();
                    break;
                case "4":
                    postsOverviewView.Run();
                    break;
                case "5":
                    await singlePostView.RunAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("invalid option.");
                    break;
            }
        }
    }
}
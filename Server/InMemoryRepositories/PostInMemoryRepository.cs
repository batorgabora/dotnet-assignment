using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new List<Post>();

    public PostInMemoryRepository()
    {
        posts.Add(new Post { Id = 1, Title = "welcome to chatter", Body = "yk u can just chat", UserId = 1 });
        posts.Add(new Post { Id = 2, Title = "hii", Body = "puszi", UserId = 2 });
        posts.Add(new Post { Id = 3, Title = "least favorite C# features", Body = "hi Allan", UserId = 3 });
    }

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max(x => x.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(x => x.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"post with ID '{post.Id}' not found :((");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(x => x.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"post with ID '{id}' not found :((");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(x => x.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"post with ID '{id}' not found :((");
        }

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}
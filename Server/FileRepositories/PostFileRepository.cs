using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            List<Post> seedPosts = new List<Post>
            {
                new Post { Id = 1, Title = "what's cooking", Body = "post numero uno", UserId = 1 },
                new Post { Id = 2, Title = "this is getting tedious", Body = "dunno what to write here oh hi allan", UserId = 2 },
                new Post { Id = 3, Title = "one two three", Body = "four five and i stop here", UserId = 3 }
            };
            File.WriteAllText(filePath, JsonSerializer.Serialize(seedPosts));
        }
    }

    private async Task<List<Post>> LoadAsync()
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
    }

    private async Task SaveAsync(List<Post> posts)
    {
        string postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

    public async Task<Post> AddAsync(Post post)
    {
        List<Post> posts = await LoadAsync();

        post.Id = posts.Any()
            ? posts.Max(x => x.Id) + 1
            : 1;
        posts.Add(post);

        await SaveAsync(posts);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await LoadAsync();

        Post? existingPost = posts.SingleOrDefault(x => x.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        await SaveAsync(posts);
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await LoadAsync();

        Post? postToRemove = posts.SingleOrDefault(x => x.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        await SaveAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await LoadAsync();

        Post? post = posts.SingleOrDefault(x => x.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"post with ID '{id}' not found");
        }

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string postsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts.AsQueryable();
    }
}
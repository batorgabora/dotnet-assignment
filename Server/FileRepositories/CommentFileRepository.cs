using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            List<Comment> seedComments = new List<Comment>
            {
                new Comment { Id = 1, Body = "puszi!", PostId = 1, UserId = 2 },
                new Comment { Id = 2, Body = "yeah fair.", PostId = 1, UserId = 3 },
                new Comment { Id = 3, Body = "hia allan!", PostId = 2, UserId = 1 }
            };
            File.WriteAllText(filePath, JsonSerializer.Serialize(seedComments));
        }
    }

    private async Task<List<Comment>> LoadAsync()
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
    }

    private async Task SaveAsync(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await LoadAsync();

        comment.Id = comments.Any()
            ? comments.Max(x => x.Id) + 1
            : 1;
        comments.Add(comment);

        await SaveAsync(comments);
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadAsync();

        Comment? existingComment = comments.SingleOrDefault(x => x.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        await SaveAsync(comments);
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadAsync();

        Comment? commentToRemove = comments.SingleOrDefault(x => x.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        await SaveAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadAsync();

        Comment? comment = comments.SingleOrDefault(x => x.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        string commentsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments.AsQueryable();
    }
}
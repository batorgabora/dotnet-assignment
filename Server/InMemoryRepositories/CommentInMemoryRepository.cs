using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new List<Comment>();

    public CommentInMemoryRepository()
    {
        comments.Add(new Comment { Id = 1, Body = "tight!", PostId = 1, UserId = 2 });
        comments.Add(new Comment { Id = 2, Body = "yooo!", PostId = 1, UserId = 3 });
        comments.Add(new Comment { Id = 3, Body = "hope you're doing fine", PostId = 3, UserId = 1 });
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(x => x.Id) + 1
            : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(x => x.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"comment with ID '{comment.Id}' not found  :‹");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(x => x.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"comment with ID '{id}' not found  :‹");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(x => x.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"comment with ID '{id}' not found  :‹");
        }

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}
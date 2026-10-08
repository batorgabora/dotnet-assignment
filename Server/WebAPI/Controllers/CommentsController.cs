using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public CommentsController(ICommentRepository commentRepo, IPostRepository postRepo,
        IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        await postRepo.GetSingleAsync(request.PostId);   // post must exist
        await userRepo.GetSingleAsync(request.UserId);   // user must exist

        Comment comment = new()
        {
            Body = request.Body,
            PostId = request.PostId,
            UserId = request.UserId
        };
        Comment created = await commentRepo.AddAsync(comment);

        CommentDto dto = await ToDtoAsync(created);
        return Created($"/comments/{dto.Id}", dto);
    }

    // PUT /comments/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateComment([FromRoute] int id,
        [FromBody] UpdateCommentDto request)
    {
        Comment existing = await commentRepo.GetSingleAsync(id);
        existing.Body = request.Body;
        await commentRepo.UpdateAsync(existing);
        return NoContent();
    }

    // GET /comments/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment([FromRoute] int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);
        return Ok(await ToDtoAsync(comment));
    }

    // GET /comments?userId=1&userName=anna&postId=3
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepo.GetMany();

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingIds = userRepo.GetMany()
                .Where(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();
            comments = comments.Where(c => matchingIds.Contains(c.UserId));
        }

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        List<CommentDto> result = new();
        foreach (Comment c in comments.ToList())
        {
            result.Add(await ToDtoAsync(c));
        }
        return Ok(result);
    }

    // DELETE /comments/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        await commentRepo.GetSingleAsync(id);
        await commentRepo.DeleteAsync(id);
        return NoContent();
    }

    private async Task<CommentDto> ToDtoAsync(Comment c)
    {
        User author = await userRepo.GetSingleAsync(c.UserId);
        return new CommentDto
        {
            Id = c.Id,
            Body = c.Body,
            PostId = c.PostId,
            UserId = c.UserId,
            AuthorUserName = author.UserName
        };
    }
}
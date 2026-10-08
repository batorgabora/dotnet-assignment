using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo,
        ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        await userRepo.GetSingleAsync(request.UserId);   // author must exist, else 404

        Post post = new() { Title = request.Title, Body = request.Body, UserId = request.UserId };
        Post created = await postRepo.AddAsync(post);

        PostDto dto = await ToDtoAsync(created, false);
        return Created($"/posts/{dto.Id}", dto);
    }

    // PUT /posts/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        Post existing = await postRepo.GetSingleAsync(id);
        existing.Title = request.Title;
        existing.Body = request.Body;
        await postRepo.UpdateAsync(existing);
        return NoContent();
    }

    // GET /posts/5?includeComments=true
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost([FromRoute] int id,
        [FromQuery] bool includeComments = false)
    {
        Post post = await postRepo.GetSingleAsync(id);
        return Ok(await ToDtoAsync(post, includeComments));
    }

    // GET /posts?titleContains=x&userId=1&userName=anna&top=10
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? top)
    {
        IQueryable<Post> posts = postRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            posts = posts.Where(p =>
                p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingIds = userRepo.GetMany()
                .Where(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();
            posts = posts.Where(p => matchingIds.Contains(p.UserId));
        }

        if (top.HasValue && top.Value > 0)
        {
            posts = posts.Take(top.Value);
        }

        List<PostDto> result = new();
        foreach (Post p in posts.ToList())
        {
            result.Add(await ToDtoAsync(p, false));
        }
        return Ok(result);
    }

    // GET /posts/5/comments  (nested route for a post's comments)
    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost([FromRoute] int postId)
    {
        await postRepo.GetSingleAsync(postId);   // 404 if post missing
        return Ok(GetCommentDtos(postId));
    }

    // DELETE /posts/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        await postRepo.GetSingleAsync(id);

        // remove the post's comments first so none are orphaned
        List<int> commentIds = commentRepo.GetMany().Where(c => c.PostId == id)
            .Select(c => c.Id).ToList();
        foreach (int commentId in commentIds)
        {
            await commentRepo.DeleteAsync(commentId);
        }

        await postRepo.DeleteAsync(id);
        return NoContent();
    }

    private async Task<PostDto> ToDtoAsync(Post post, bool includeComments)
    {
        User author = await userRepo.GetSingleAsync(post.UserId);
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorUserName = author.UserName,
            Comments = includeComments ? GetCommentDtos(post.Id) : null
        };
    }

    private List<CommentDto> GetCommentDtos(int postId)
    {
        Dictionary<int, string> names = userRepo.GetMany().ToDictionary(u => u.Id, u => u.UserName);
        return commentRepo.GetMany()
            .Where(c => c.PostId == postId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                PostId = c.PostId,
                UserId = c.UserId,
                AuthorUserName = names.GetValueOrDefault(c.UserId, "unknown")
            })
            .ToList();
    }
}
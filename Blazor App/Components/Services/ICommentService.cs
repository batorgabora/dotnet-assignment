using ApiContracts;

namespace Blazor_App.Services;

public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(CreateCommentDto request);
}
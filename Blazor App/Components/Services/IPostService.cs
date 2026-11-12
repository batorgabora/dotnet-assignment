using ApiContracts;

namespace Blazor_App.Services;

public interface IPostService
{
    Task<PostDto> AddPostAsync(CreatePostDto request);
    Task<List<PostDto>> GetPostsAsync();
    Task<PostDto> GetPostAsync(int id, bool includeComments = false);
}
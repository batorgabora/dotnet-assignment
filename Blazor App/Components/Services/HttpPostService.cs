using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace Blazor_App.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDto>(response, jsonOptions)!;
    }

    public async Task<List<PostDto>> GetPostsAsync()
    {
        HttpResponseMessage httpResponse = await client.GetAsync("posts");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<List<PostDto>>(response, jsonOptions)!;
    }

    public async Task<PostDto> GetPostAsync(int id, bool includeComments = false)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{id}?includeComments={includeComments}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDto>(response, jsonOptions)!;
    }
}
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor_App.Services;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient client;
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private ClaimsPrincipal currentUser = new(new ClaimsIdentity());

    public SimpleAuthProvider(HttpClient client)
    {
        this.client = client;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(currentUser));
    }

    public async Task<bool> LoginAsync(string userName, string password)
    {
        LoginRequestDto request = new() { UserName = userName, Password = password };
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("auth/login", request);

        if (!httpResponse.IsSuccessStatusCode)
        {
            return false;
        }

        string response = await httpResponse.Content.ReadAsStringAsync();
        UserDto user = JsonSerializer.Deserialize<UserDto>(response, jsonOptions)!;

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        ClaimsIdentity identity = new(claims, "apiauth");
        currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
    }

    public void Logout()
    {
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public int? GetCurrentUserId()
    {
        string? idClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return idClaim is null ? null : int.Parse(idClaim);
    }
}
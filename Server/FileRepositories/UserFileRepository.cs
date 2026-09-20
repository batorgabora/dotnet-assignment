using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            List<User> seedUsers = new List<User>
            {
                new User { Id = 1, UserName = "picasso", Password = "12345" },
                new User { Id = 2, UserName = "monet", Password = "12345" },
                new User { Id = 3, UserName = "magritte", Password = "12345" }
            };
            File.WriteAllText(filePath, JsonSerializer.Serialize(seedUsers));
        }
    }

    private async Task<List<User>> LoadAsync()
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
    }

    private async Task SaveAsync(List<User> users)
    {
        string usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await LoadAsync();

        user.Id = users.Any()
            ? users.Max(x => x.Id) + 1
            : 1;
        users.Add(user);

        await SaveAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadAsync();

        User? existingUser = users.SingleOrDefault(x => x.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"user with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        await SaveAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadAsync();

        User? userToRemove = users.SingleOrDefault(x => x.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"user with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        await SaveAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadAsync();

        User? user = users.SingleOrDefault(x => x.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        return user;
    }

    public IQueryable<User> GetMany()
    {
        string usersAsJson = File.ReadAllTextAsync(filePath).Result;
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        return users.AsQueryable();
    }
}
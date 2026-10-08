namespace Entities;

// thrown by the repositories when an entity with the requested ID doesn't exist.
// lets the Web API's exception middleware return 404 Not Found instead of a generic 500.
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
namespace ApiContracts;

public class CreateCommentDto
{
    public required string Body { get; set; }
    public required int PostId { get; set; }
    public required int UserId { get; set; }
}

public class UpdateCommentDto
{
    public required string Body { get; set; }
}

public class CommentDto
{
    public required int Id { get; set; }
    public required string Body { get; set; }
    public required int PostId { get; set; }
    public required int UserId { get; set; }
    public required string AuthorUserName { get; set; }
}
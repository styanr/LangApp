namespace LangApp.Infrastructure.EF.Models.Posts;

public class PostCommentReadModel
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
}

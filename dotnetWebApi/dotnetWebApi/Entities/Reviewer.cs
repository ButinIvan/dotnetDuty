namespace dotnetWebApi.Entities;

public class Reviewer
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role {get; private set;}

    public Document Document { get; private set; }
    public User User { get; private set; }
    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    private Reviewer()
    {
    }
    
    public Reviewer(Guid documentId, Guid userId, string role)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        UserId = userId;
        Role = role;
    }
}
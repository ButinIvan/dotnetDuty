namespace dotnetWebApi.Entities;

public class Reviewer
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Role {get; private set;}

    public Document Document { get; private set; }
    public User User { get; private set; }
    public Comment Comment { get; private set; }

    private Reviewer()
    {
    }
    
    public Reviewer(Guid documentId, Guid ownerId, string role)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        OwnerId = ownerId;
        Role = role;
    }
}
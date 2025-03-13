namespace dotnetWebApi.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsClosedToComment { get; private set; }
    public DateTime LastEdited { get; private set; }
    public string S3Path { get; private set; }
    
    public User Owner { get; private set; }
    public ICollection<User> Reviewers { get; private set; } = new List<User>();

    public Document()
    {
    }

    public Document(string title, Guid ownerId, string s3Path)
    {
        Id = Guid.NewGuid();
        Title = title;
        OwnerId = ownerId;
        IsFinished = false;
        IsClosedToComment = true;
        LastEdited = DateTime.UtcNow;
        S3Path = s3Path;
    }
    
    public void UpdateS3Path(string newS3Path)
    {
        S3Path = newS3Path;
        LastEdited = DateTime.UtcNow;
    }

    public void UpdateLastEdited()
    {
        LastEdited = DateTime.UtcNow;
    }

    public void SetFinished()
    {
        IsFinished = true;
    }

    public void ClosedToComment()
    {
        IsClosedToComment = true;
    }
}
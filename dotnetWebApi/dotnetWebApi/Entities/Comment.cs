namespace dotnetWebApi.Entities;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public string S3Path { get; private set; }
    public DateTime LastModified { get; private set; }
    
    public Document Document { get; private set; }
    public Reviewer Reviewer { get; private set; }

    private Comment()
    {
    }

    public Comment(Guid documentId, Guid reviewerId, string s3Path)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        ReviewerId = reviewerId;
        S3Path = s3Path;
        LastModified = DateTime.UtcNow;
    }

    public void UpdateLastModifiedDate()
    {
        LastModified = DateTime.UtcNow;
    }
}
namespace dotnetWebApi.Entities;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    public Guid DocumentId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public string S3Path { get; private set; }
    public DateTime LastModified { get; private set; }
    
    // Could use "virtual" keyword here to establish one-to-many 
    public Document Document { get; private set; }
    public Reviewer Reviewer { get; private set; }
    public Comment? ParentComment { get; private set; }
    public ICollection<Comment> Replies { get; private set; } = [];

    private Comment()
    {
    }

    public Comment(Guid documentId, Guid reviewerId, string s3Path, Guid? parentCommentId = null)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        ReviewerId = reviewerId;
        S3Path = s3Path;
        LastModified = DateTime.UtcNow;
        ParentCommentId = parentCommentId;
    }

    public void SetParentComment(Guid commentId)
    {
        ParentCommentId = commentId;
    }

    public void UpdateLastModifiedDate()
    {
        LastModified = DateTime.UtcNow;
    }
}
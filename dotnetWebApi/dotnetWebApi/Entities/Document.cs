namespace dotnetWebApi.Entities;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public Guid OwnerId { get; set; }
    public string Body { get; set; }
    public bool IsFinished { get; set; }
    public string S3Path { get; set; }
    
    public User Owner { get; set; }
    public IEnumerable<User> Reviewer { get; set; }
}
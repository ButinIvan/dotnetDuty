namespace dotnetWebApi.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string Role { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<Document> ReviewDocuments { get; set; } = new List<Document>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
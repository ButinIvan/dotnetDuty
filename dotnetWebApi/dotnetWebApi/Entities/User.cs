namespace dotnetWebApi.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserName { get; private set; }
    public string FirstName { get; private set; }
    public string Role { get; private set; }
    public string PasswordHash { get; set; }
    public ICollection<Document> OwnedDocuments { get; set; } = new List<Document>();
    public ICollection<Reviewer> ReviewAssignments { get; set; } = new List<Reviewer>();

    private User()
    {
    }

    public User(string userName, string firstName, string role)
    {
        UserName = userName;
        FirstName = firstName;
        Role = role;
    }
}
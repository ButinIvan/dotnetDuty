namespace BusinessLogic;

public class AuthSettings
{
    public required TimeSpan Expires { get; set; }
    public required string SecretKey { get; set; }
}
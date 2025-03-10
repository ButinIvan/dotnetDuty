namespace dotnetWebApi.Services;

public class EnvService
{
    public string GetVariable(string envItem, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(envItem) ?? defaultValue;
    }
}
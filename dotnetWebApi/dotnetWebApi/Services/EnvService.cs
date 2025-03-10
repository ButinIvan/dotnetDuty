namespace dotnetWebApi.Services;

public class EnvService
{
    public string GetVariable(string envKey, string defaultValue = null)
    {
        return Environment.GetEnvironmentVariable(envKey) ?? defaultValue;
    }
}
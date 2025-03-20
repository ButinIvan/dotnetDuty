namespace dotnetWebApi.Middlewares;

public class SwaggerAuthMiddleware
{
    private readonly RequestDelegate _next;

    public SwaggerAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") && 
            context.Response.Headers.ContainsKey("Authorization"))
        {
            var token = context.Response.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                context.Response.Cookies.Append("SwaggerAuthToken", token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(1)
                });
            }
        }

        await _next(context);
    }
}
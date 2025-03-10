using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace dotnetWebApi.Extensions;

public static class ControllerExtension
{
    public static Guid GetUserId(this ControllerBase controller)
    {
        var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }
}
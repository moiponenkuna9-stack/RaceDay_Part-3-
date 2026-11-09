using System.Security.Claims;

namespace RaceDay.Middleware;

public sealed class SessionPrincipalMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.Session.GetInt32("UserId");
        var role = context.Session.GetString("Role");
        if (userId.HasValue && !string.IsNullOrWhiteSpace(role))
        {
            var identity = new ClaimsIdentity("RaceDaySession");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            context.User = new ClaimsPrincipal(identity);
        }
        await next(context);
    }
}

using api.Services;

namespace api.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, UserService userService)
    {
        await userService.SetupUser(context.Request.Headers.Authorization);
        userService.Subject = "wtg;";
        Console.WriteLine("setup lol");
        await _next.Invoke(context);
    }
}
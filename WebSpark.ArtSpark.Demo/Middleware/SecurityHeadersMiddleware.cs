namespace WebSpark.ArtSpark.Demo.Middleware;

/// <summary>
/// Security headers middleware for enhanced protection
/// Adds security headers to all responses for better security posture
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

        // Content Security Policy for enhanced XSS protection
        var csp = "default-src 'self'; " +
                 "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                 "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                 "img-src 'self' data: https:; " +
                 "font-src 'self' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                 "connect-src 'self' https://api.artic.edu;";

        context.Response.Headers.Append("Content-Security-Policy", csp);

        await _next(context);
    }
}

/// <summary>
/// Extension methods for registering security middleware
/// </summary>
public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}

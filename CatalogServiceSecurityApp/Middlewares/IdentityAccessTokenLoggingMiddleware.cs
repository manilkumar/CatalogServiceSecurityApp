namespace CatalogServiceSecurityApp.Middlewares
{
    public class IdentityAccessTokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IdentityAccessTokenLoggingMiddleware> _logger;

        public IdentityAccessTokenLoggingMiddleware(RequestDelegate next, ILogger<IdentityAccessTokenLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Assuming the access token is in the Authorization header
            var accessToken = context.Request.Headers["Authorization"];

            // Log the access token details
            if (!string.IsNullOrEmpty(accessToken))
            {
                _logger.LogInformation($"Identity Access Token: {accessToken}");
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}

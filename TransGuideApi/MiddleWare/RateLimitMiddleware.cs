using System.Net;

namespace StudentProgressTracker.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private readonly Dictionary<string, UserRateLimit> _rateLimits;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _rateLimits = new Dictionary<string, UserRateLimit>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);

            if (!IsRequestAllowed(clientId))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers.Add("X-RateLimit-Limit", "100");
                context.Response.Headers.Add("X-RateLimit-Remaining", "0");
                context.Response.Headers.Add("X-RateLimit-Reset", GetResetTime().ToString());

                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            AddRateLimitHeaders(context, clientId);
            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Try to get user ID from JWT token first
            var userId = context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
                return $"user_{userId}";

            // Fall back to IP address
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private bool IsRequestAllowed(string clientId)
        {
            var now = DateTime.UtcNow;

            if (!_rateLimits.ContainsKey(clientId))
            {
                _rateLimits[clientId] = new UserRateLimit
                {
                    RequestCount = 1,
                    WindowStart = now
                };
                return true;
            }

            var rateLimit = _rateLimits[clientId];

            // Reset window if it's been more than 1 minute
            if (now - rateLimit.WindowStart > TimeSpan.FromMinutes(1))
            {
                rateLimit.RequestCount = 1;
                rateLimit.WindowStart = now;
                return true;
            }

            // Check if under limit
            if (rateLimit.RequestCount < 100)
            {
                rateLimit.RequestCount++;
                return true;
            }

            return false;
        }

        private void AddRateLimitHeaders(HttpContext context, string clientId)
        {
            var rateLimit = _rateLimits.GetValueOrDefault(clientId);
            if (rateLimit != null)
            {
                var remaining = Math.Max(0, 100 - rateLimit.RequestCount);
                context.Response.Headers.Add("X-RateLimit-Limit", "100");
                context.Response.Headers.Add("X-RateLimit-Remaining", remaining.ToString());
                context.Response.Headers.Add("X-RateLimit-Reset", GetResetTime(rateLimit.WindowStart).ToString());
            }
        }

        private long GetResetTime(DateTime? windowStart = null)
        {
            var resetTime = (windowStart ?? DateTime.UtcNow).AddMinutes(1);
            return ((DateTimeOffset)resetTime).ToUnixTimeSeconds();
        }
    }

    public class UserRateLimit
    {
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; }
    }
}
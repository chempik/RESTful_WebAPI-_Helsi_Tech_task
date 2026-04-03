using Application.Services;

namespace RESTful_WebAPI__Helsi_Tech_task.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.Request.Headers["X-UserId"].FirstOrDefault();
            // ExceptionHandlingMiddleware will intercept it and return 401 Unauthorized.
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID is required.");
            return userId;
        }
    }
}

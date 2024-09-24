namespace PalettaPolizeiPro.Services.Users
{
    public class ClientIpService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientIpService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Items["ClientIP"]?.ToString();
        }
    }

}

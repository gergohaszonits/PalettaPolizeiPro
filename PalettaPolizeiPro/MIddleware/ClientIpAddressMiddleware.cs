namespace PalettaPolizeiPro.MIddleware
{
    public class ClientIpAddressMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientIpAddressMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extract client IP address
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            // Store the IP address in the HttpContext.Items or any other service
            if (clientIp != null)
            {
                context.Items["ClientIP"] = clientIp;
            }

            await _next(context);
        }
    }

}

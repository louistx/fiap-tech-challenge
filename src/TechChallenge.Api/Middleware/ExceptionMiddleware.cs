using System.Text.Json;
using TechChallenge.Infrastructure.IoC.Exceptions;

namespace TechChallenge.Api.Middleware
{
    public class ExceptionMiddleware
    {
        #region Properties

        private readonly RequestDelegate _req;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        #endregion

        #region Constructor

        public ExceptionMiddleware(RequestDelegate req, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _req = req;
            _logger = logger;
            _env = env;
        }

        #endregion

        #region Implementation

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _req(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                int statusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()
                    ? new ExceptionConfiguration(statusCode.ToString(), ex.Message, ex.StackTrace?.ToString() ?? string.Empty)
                    : new ExceptionConfiguration(statusCode.ToString(), "Ocorreu um erro ao processar a sua requisição. Por favor, entre em contato conosco e comunique o ocorrido.", ex.Message, ex.StackTrace?.ToString() ?? string.Empty);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);
            }
        }

        #endregion
    }
}
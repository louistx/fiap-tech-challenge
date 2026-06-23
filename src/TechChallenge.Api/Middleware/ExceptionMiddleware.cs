using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
                    ValidationException => StatusCodes.Status400BadRequest,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";

                if (ex is ValidationException validationException)
                {
                    var errors = validationException.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.ErrorMessage).ToArray());

                    var problemDetails = new ValidationProblemDetails(errors)
                    {
                        Status = statusCode,
                        Title = "Ocorreram erros de validação."
                    };

                    await httpContext.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }

                var response = _env.IsDevelopment()
                    ? new ExceptionConfiguration(statusCode.ToString(), ex.Message, ex.StackTrace ?? string.Empty)
                    : new ExceptionConfiguration(statusCode.ToString(), "Ocorreu um erro ao processar a sua requisição. Por favor, entre em contato conosco e comunique o ocorrido.", ex.Message, ex.StackTrace ?? string.Empty);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);
            }
        }

        #endregion
    }
}

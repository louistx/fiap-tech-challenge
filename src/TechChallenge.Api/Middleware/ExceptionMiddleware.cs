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

        #endregion

        #region Constructor

        public ExceptionMiddleware(RequestDelegate req, ILogger<ExceptionMiddleware> logger)
        {
            _req = req;
            _logger = logger;
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
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
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
                    problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

                    await httpContext.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = GetTitle(statusCode),
                    Detail = ShouldExposeMessage(ex)
                        ? ex.Message
                        : "Ocorreu um erro ao processar a sua requisição. Por favor, entre em contato conosco e comunique o ocorrido."
                };
                problem.Extensions["traceId"] = httpContext.TraceIdentifier;

                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }

        private static string GetTitle(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "Requisição inválida.",
            StatusCodes.Status401Unauthorized => "Não autorizado.",
            StatusCodes.Status404NotFound => "Recurso não encontrado.",
            _ => "Erro interno."
        };

        private static bool ShouldExposeMessage(Exception ex) => ex switch
        {
            NotFoundException => true,
            BadRequestException => true,
            UnauthorizedException => true,
            UnauthorizedAccessException => true,
            KeyNotFoundException => true,
            InvalidOperationException => true,
            _ => false
        };

        #endregion
    }
}

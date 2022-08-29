using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using System.Net;
using System.Text.Json;

namespace ScheduleApi.Extensions {
    public class ExceptionHandlingMiddleware {
        private readonly RequestDelegate _next;
        public bool AllowStatusCode404Response { get; set; } = true;


        public ExceptionHandlingMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext) {
            try {
                await _next(httpContext);
            } catch (Exception ex) {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ErrorResponse {
                Success = false
            };
            switch (exception) {
                case AppException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message;
                    break;
                default:
                    if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate")) {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorResponse.Message = "one or more unique keys already exists in database";
                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Internal Server errors." + exception.Message;
                    break;
            }
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}

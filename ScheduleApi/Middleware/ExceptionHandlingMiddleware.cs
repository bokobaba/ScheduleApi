using Microsoft.EntityFrameworkCore;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace ScheduleApi.Extensions {
    public class ExceptionHandlingMiddleware {
        private readonly RequestDelegate _next;

        private static readonly string EXCEPTION_FOREIGN_KEY = "FOREIGN KEY";
        private static readonly string EXCEPTION_DUPLICATE = "duplicate";

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
                    switch (ex.type) {
                        case ExceptionType.FORBIDDEN:
                            response.StatusCode = StatusCodes.Status403Forbidden;
                            errorResponse.Message = ex.Message;
                            break;
                        default:
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            errorResponse.Message = ex.Message;
                            break;
                    }
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message;
                    break;
                case TaskCanceledException ex:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Message = "Connection Timeout";
                    break;
                case DbUpdateException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;

                    if (ex.InnerException != null) {
                        if (ex.InnerException.Message.Contains(EXCEPTION_FOREIGN_KEY)) {
                            object entity = ex.Entries[0].Entity;
                            PropertyInfo? prop = entity.GetType().GetProperty("EmployeeId");
                            if (prop != null) {
                                var id = prop.GetValue(entity);
                                errorResponse.Message = string.Format(
                                    "employee with id: {0} does not exist", id);
                                break;
                            }
                        }

                        if (ex.InnerException.Message.Contains(EXCEPTION_DUPLICATE)) {
                            string t = ex.Entries[0].Entity.GetType().Name;
                            errorResponse.Message = string.Format("{0} already exists", t);
                            break;
                        }
                    }

                    errorResponse.Message = "invalid request";
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Internal Server errors.";
                    break;
            }
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}

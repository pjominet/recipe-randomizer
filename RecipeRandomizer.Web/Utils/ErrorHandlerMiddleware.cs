using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RecipeRandomizer.Business.Utils.Exceptions;

namespace RecipeRandomizer.Web.Utils
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
#if DEBUG
                // write exception stack trace to console in debug
                await Console.Error.WriteLineAsync(error.ToString());
#endif

                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    BadRequestException e => (int) HttpStatusCode.BadRequest,
                    KeyNotFoundException e => (int) HttpStatusCode.NotFound,
                    _ => (int) HttpStatusCode.InternalServerError
                };

                var result = JsonSerializer.Serialize(new {message = error.Message});
                await response.WriteAsync(result);
            }
        }
    }
}

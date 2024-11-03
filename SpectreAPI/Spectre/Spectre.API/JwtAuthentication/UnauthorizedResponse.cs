using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace Spectre.API.JwtAuthentication
{
    public class UnauthorizedResponse
    {
        private static void writeErrorResponse(HttpContext Context, params string[] Errors)
        {
            Context.Response.ContentType = "application/json";
            using (var writer = new Utf8JsonWriter(Context.Response.Body))
            {
                writer.WriteStartObject();
                writer.WriteString("Message", "Authorization has been denied for this request.");
                writer.WriteEndObject();
                writer.Flush();
            }
        }

        private readonly RequestDelegate _request;

        public UnauthorizedResponse(RequestDelegate RequestDelegate)
        {
            if (RequestDelegate == null)
            {
                throw new ArgumentNullException(nameof(RequestDelegate)
                    , nameof(RequestDelegate) + " is required");
            }

            _request = RequestDelegate;
        }

        public async Task InvokeAsync(HttpContext Context)
        {
            if (Context == null)
            {
                throw new ArgumentNullException(nameof(Context)
                    , nameof(Context) + " is required");
            }

            await _request(Context);

            if (Context.Response.StatusCode == 401)
            {
                writeErrorResponse(Context, "Please log in");
            }
        }
    }
}

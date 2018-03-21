using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using System;

namespace Boondocks.Auth.WebApi.ActionResults
{
    /// <summary>
    /// Custom action result for returning JWT signed authentication token.
    /// </summary>
    public class OkJwtAuthResult : OkObjectResult
    {
        public string Token { get; }

        public OkJwtAuthResult(string token) : base(StatusCodes.Status200OK)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public OkJwtAuthResult(string token, object result): this(token)
        {
            Value = result ?? throw new ArgumentNullException(nameof(token));
        }

        // Invoked by the HTTP response pipeline.
        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("X-Custom-Token", new StringValues(Token));
            return base.ExecuteResultAsync(context);
        }
    }
}
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Concept.Logging;
using Ledger.WebApi.RequestHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ledger.WebApi.Filters
{
    public class RequestContextFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestContext = (IRequestContext)context.HttpContext.RequestServices.GetService(typeof(IRequestContext));

            requestContext.TelemetryEvent = CreateTelemetryEvent(context);

            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                HandleAuthenticatedUser(requestContext, user);
            }
            else
            {
                HandleAnonymousUser(requestContext, user);
            }

            await next().ConfigureAwait(false);
        }

        private static TelemetryEvent CreateTelemetryEvent(ActionContext context)
        {
            var telemetryEvent = new TelemetryEvent("WebRequest");
            
            telemetryEvent["EventId"] = Guid.NewGuid();
            telemetryEvent["Http"] = new
            {
                Method = context.HttpContext.Request.Method,
                Path = context.HttpContext.Request.Path.Value,
                StatusCode = context.HttpContext.Response.StatusCode,
            };
            telemetryEvent["Timestamp"] = DateTime.UtcNow;

            return telemetryEvent;
        }

        private static void HandleAnonymousUser(IRequestContext requestContext, ClaimsPrincipal user)
        {
            requestContext.IsAuthenticated = false;
        }

        private static void HandleAuthenticatedUser(IRequestContext requestContext, ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.SingleOrDefault(x => x.Type == BuildUserJwtTokenRequestHandler.UserIdClaimName);

            if (userIdClaim == null)
            {
                throw new InvalidOperationException($"Expecting a claim type '{BuildUserJwtTokenRequestHandler.UserIdClaimName}' for an authenticated user.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new ArgumentOutOfRangeException($"The claim '{BuildUserJwtTokenRequestHandler.UserIdClaimName}' was not a valid UUID: '{userIdClaim.Value}'.");
            }

            requestContext.IsAuthenticated = true;
            requestContext.UserId = userId;
        }
    }
}
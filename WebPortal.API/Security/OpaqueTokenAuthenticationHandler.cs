using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebPortal.API.Security
{
    public sealed class OpaqueTokenAuthenticationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.GetRequestContext().Principal = new ClaimsPrincipal(new ClaimsIdentity());

            var authorization = request.Headers.Authorization;

            if (authorization != null &&
                string.Equals(authorization.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(authorization.Parameter))
            {
                AuthenticationTicket ticket = MemoryCacheTokenStore.Instance.Retrieve(authorization.Parameter);

                if (ticket != null &&
                    ticket.Identity != null &&
                    ticket.Identity.IsAuthenticated)
                {
                    request.GetRequestContext().Principal = new ClaimsPrincipal(ticket.Identity);
                }
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer"));
            }

            return response;
        }
    }
}
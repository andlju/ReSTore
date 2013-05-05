using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ReSTore.Web.Controllers.BasketControllers
{
    public class BasketIdHandler : DelegatingHandler
    {
        public static string BasketIdToken = "basket-id";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string basketId;

            // Try to get the basket ID from the request; otherwise create a new ID.
            var cookie = request.Headers.GetCookies(BasketIdToken).FirstOrDefault();
            if (cookie == null)
            {
                basketId = Guid.NewGuid().ToString();
            }
            else
            {
                basketId = cookie[BasketIdToken].Value;
                try
                {
                    Guid guid = Guid.Parse(basketId);
                }
                catch (FormatException)
                {
                    // Bad session ID. Create a new one.
                    basketId = Guid.NewGuid().ToString();
                }
            }

            // Store the session ID in the request property bag.
            request.Properties[BasketIdToken] = basketId;

            // Continue processing the HTTP request.
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // Set the session ID as a cookie in the response message.
            response.Headers.AddCookies(new CookieHeaderValue[]
                {
                    new CookieHeaderValue(BasketIdToken, basketId)
                });

            return response;
        }
    }
}
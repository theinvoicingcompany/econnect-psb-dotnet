using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Models;

namespace EConnect.Psb.Client
{
    public static class PsbClientResponseExtensions
    {
        private static async Task<TResponseBody> ReadBody<TResponseBody>(this HttpResponseMessage response, CancellationToken cancellation)
        {
            try
            {
                var body = await response.Content.ReadFromJsonAsync<TResponseBody>(cancellationToken: cancellation);
                if (body == null)
                    throw new EConnectException($"PSB{response.StatusCode}.JSON01", "Response body is empty.");

                return body;
            }
            catch (JsonException ex)
            {
                var text = await response.Content.ReadAsStringAsync();
                if (text.Length > 1000)
                    text = text.Substring(0, 1000);

                throw new EConnectException($"PSB{(int)response.StatusCode}.JSON02", $"'{response.ReasonPhrase}', unexpected JSON '{ex.Message}'. The message from the server: '{text}'.");
            }
        }

        public static async Task ThrowError(this HttpResponseMessage response, CancellationToken cancellation)
        {
            var exception = await response.ReadBody<EConnectException>(cancellation);
            throw exception;
        }

        public static async Task<TResponseBody> Read<TResponseBody>(this HttpResponseMessage response,
            CancellationToken cancellation)
        {
            if (!response.IsSuccessStatusCode)
                await response.ThrowError(cancellation);

            return await response.ReadBody<TResponseBody>(cancellation);
        }
    }
}

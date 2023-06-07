using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EConnect.Psb.Client.Extensions;
using EConnect.Psb.Models;

namespace EConnect.Psb.Api
{
    public class PsbPeppolLookupApi : IPsbPeppolLookupApi, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _psbProd = new("https://psb.econnect.eu");
        private readonly Uri _psbAccp = new("https://accp-psb.econnect.eu");

        public PsbPeppolLookupApi() : this(false)
        {
        }

        public PsbPeppolLookupApi(bool usePeppolTest)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = usePeppolTest ? _psbAccp : _psbProd,
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
        }

        private async Task<string> CreateRequestUri(
            List<string> partyIds,
            string? preferredDocumentTypeId = null,
            List<string>? documentTypeIds = null,
            string? documentFamily = null,
            bool? isCredit = null)
        {
            var requestUri = "/api/v1/peppol/deliveryOption";
            var query = new List<KeyValuePair<string,string>>();

            query.AddRange(partyIds.Select(partyId =>
                new KeyValuePair<string, string>("partyIds", partyId)));

            if (!string.IsNullOrEmpty(preferredDocumentTypeId))
                query.Add(new KeyValuePair<string, string>("preferredDocumentTypeId", preferredDocumentTypeId!));

            if(documentTypeIds != null) 
                query.AddRange(documentTypeIds.Select(documentTypeId => 
                    new KeyValuePair<string, string>("documentTypeIds", documentTypeId)));

            if (!string.IsNullOrEmpty(documentFamily))
                query.Add(new KeyValuePair<string, string>("documentFamily", documentFamily!));

            if (isCredit != null)
                query.Add(new KeyValuePair<string, string>("isCredit", isCredit.ToString()));
        
            var queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(queryString))
                requestUri += "?" + queryString;

            return requestUri;
        }

        protected virtual async Task<DeliveryOption[]> GetDeliveryOptions(
           string requestUri,
           CancellationToken cancellation)
        {
            var res = await _httpClient.GetAsync(requestUri, cancellation).ConfigureAwait(false);

            var deliveryOptions = await res.Read<DeliveryOption[]>(cancellation).ConfigureAwait(false);
           
            return deliveryOptions;
        }

        public async Task<DeliveryOption[]> GetDeliveryOptions(
            List<string> partyIds,
            string? preferredDocumentTypeId = null,
            List<string>? documentTypeIds = null,
            string? documentFamily = null,
            bool? isCredit = null,
            CancellationToken cancellation = default)
        {
            var requestUri = await CreateRequestUri(partyIds, preferredDocumentTypeId,
                documentTypeIds, documentFamily, isCredit);

            return await GetDeliveryOptions(requestUri, cancellation);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

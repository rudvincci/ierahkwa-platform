using System.Net;
using System.Net.Http.Headers;
using Mamey.Http;
using Mamey.Mifos.Commands;
using Mamey.Mifos.Exceptions;
using Newtonsoft.Json;

namespace Mamey.Mifos
{
    public class MifosApiClient : IMifosApiClient
    {
        private readonly IHttpClient _httpClient;
        private readonly MifosOptions _options;
        private string _url;
        private string _bearerToken;

        public MifosApiClient(IHttpClient httpClient, MifosOptions options)
        {
            _options = options;
            _httpClient = httpClient;
            var headers = new Dictionary<string, string>();
            headers.Add("Fineract-Platform-TenantId", "Default");
            _httpClient.SetHeaders(headers);
            _url = options.HostUrl;
        }

        public async Task AuthenticateAsync()
        {
            var authUrl = string.Empty;
            switch (_options.AuthType)
            {
                case "basic":
                    _url = $"{_url}/authentication?username={_options.Username}&password={_options.Password}";


                    break;
                case "oauth":
                    _url = $"{_url}/fineract-provider/api/oauth/token";

                    break;
                default:
                    break;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, authUrl);
            var result = await _httpClient.SendAsync(request);

            if(!result.IsSuccessStatusCode)
            {
                switch(result.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new MifosBadRequestException();
                    case HttpStatusCode.Unauthorized:
                        throw new MifosUnauthorizedException();
                    default:
                        break;
                }
            }
            _bearerToken = await result.Content.ReadAsStringAsync();
        }

        public async Task<IMifosResult<U>> SendAsync<T, U>(IMifosRequest<T> request)
            where T : IMifosCommand
            where U : IMifosResponse
        {
            var jsonPayload = JsonConvert.SerializeObject(request.Command);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            var byteContent = new ByteArrayContent(buffer);
            if (string.IsNullOrWhiteSpace(_bearerToken))
            {
                await this.AuthenticateAsync();
            }
            byteContent.Headers.Add("Authorization", $"Basic {_bearerToken}");
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(request.Url, data: byteContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                return new MifosResult<U>(response.IsSuccessStatusCode,
                    JsonConvert.DeserializeObject<U>(responseContent), default);
            }
            return new MifosResult<U>(response.IsSuccessStatusCode, default,
                JsonConvert.DeserializeObject<MifosErrorResult>(responseContent));
        }
    }
}


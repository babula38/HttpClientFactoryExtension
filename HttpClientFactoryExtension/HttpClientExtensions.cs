using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpClientFactoryExtension
{
    public static class HttpClientExtensions
    {
        public static async ValueTask<TResult> GetContentAsJsonAsync<TResult>(this HttpClient client, string url)
        {
            HttpResponseMessage response = await client.GetAsync(new Uri(url));

            response.EnsureSuccessStatusCode();

            return await Deserilize<TResult>(response);
        }

        public static async ValueTask<TResponse> PostAsJsonAsync<TRequest, TResponse>(this HttpClient client, string relativeUrl, TRequest request)
        {
            StringContent jsonString = SerilizeToJson(request);

            HttpResponseMessage responseMessage = await client.PostAsync(relativeUrl, jsonString);

            return await Deserilize<TResponse>(responseMessage);
        }

        private static async ValueTask<TResult> Deserilize<TResult>(HttpResponseMessage responseMessage)
        {
            string response = await responseMessage.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResult>(response);
        }

        private static StringContent SerilizeToJson<TObject>(TObject value)
        {
            string jsonString = JsonSerializer.Serialize(value);

            var contentString = new StringContent(jsonString, Encoding.UTF8, "text/json");

            return contentString;
        }
    }
}

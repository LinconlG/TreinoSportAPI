using System.Net;

namespace TreinoSportAPI.Tests.Helpers {
    /// <summary>
    /// Fake HttpMessageHandler for mocking HttpClient in unit tests.
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response) {
            _response = response;
        }

        public static FakeHttpMessageHandler ReturnsJson(string json, HttpStatusCode statusCode = HttpStatusCode.OK) {
            var response = new HttpResponseMessage(statusCode) {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            return new FakeHttpMessageHandler(response);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return Task.FromResult(_response);
        }
    }
}

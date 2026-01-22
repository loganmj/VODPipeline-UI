namespace VODPipeline.UI.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<StatusDto?> GetStatusAsync()
            => await _http.GetFromJsonAsync<StatusDto>("api/status");

        public async Task<ConfigDto?> GetConfigAsync()
            => await _http.GetFromJsonAsync<ConfigDto>("api/config");

        // Add more endpoints as needed
    }

}

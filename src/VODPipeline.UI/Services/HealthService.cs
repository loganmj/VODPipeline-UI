using VODPipeline.UI.Data;

namespace VODPipeline.UI.Services
{
    public class HealthService
    {
        private readonly HttpClient _http;

        public HealthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<SystemHealthResponse?> GetHealthAsync()
        {
            return await _http.GetFromJsonAsync<SystemHealthResponse>("api/health");
        }
    }
}

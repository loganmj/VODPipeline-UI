using VODPipeline.UI.Data;

namespace VODPipeline.UI.Services
{
    public class ConfigService
    {
        private readonly HttpClient _http;

        public ConfigService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PipelineConfig?> GetConfigAsync()
        {
            return await _http.GetFromJsonAsync<PipelineConfig>("api/config");
        }

        public async Task<HttpResponseMessage> SaveConfigAsync(PipelineConfig config)
        {
            return await _http.PostAsJsonAsync("api/config", config);
        }
    }
}

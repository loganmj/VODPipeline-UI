using VODPipeline.UI.Data;

namespace VODPipeline.UI.Services
{
    public class VODAPIClient
    {
        private readonly HttpClient _http;

        public VODAPIClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<JobStatus?> GetStatusAsync()
        {
            return await _http.GetFromJsonAsync<JobStatus>("api/status");
        }

        public async Task<PipelineConfig?> GetConfigAsync()
        {
            return await _http.GetFromJsonAsync<PipelineConfig>("api/config");
        }

        // Add more endpoints as needed
    }

}

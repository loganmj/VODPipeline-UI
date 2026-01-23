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

        public async Task<List<JobHistoryItem>?> GetRecentJobsAsync(int count = 10)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be positive", nameof(count));
            }

            return await _http.GetFromJsonAsync<List<JobHistoryItem>>($"api/jobs/recent?count={count}");
        }

        // Add more endpoints as needed
    }

}

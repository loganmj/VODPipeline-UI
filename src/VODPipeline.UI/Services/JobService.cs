using VODPipeline.UI.Data;

namespace VODPipeline.UI.Services
{
    public class JobService
    {
        private readonly HttpClient _http;

        public JobService(HttpClient http)
        {
            _http = http;
        }

        public async Task<JobStatus?> GetStatusAsync()
        {
            return await _http.GetFromJsonAsync<JobStatus>("api/status");
        }

        public async Task<List<JobHistoryItem>?> GetJobsAsync(int count = 10)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be positive", nameof(count));
            }

            return await _http.GetFromJsonAsync<List<JobHistoryItem>>($"api/jobs/recent?count={count}");
        }

        public async Task<JobDetailInfo?> GetJobDetailAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Job ID cannot be null or empty", nameof(id));
            }

            return await _http.GetFromJsonAsync<JobDetailInfo>($"api/jobs/{id}");
        }

        public async Task<List<JobEvent>?> GetJobEventsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Job ID cannot be null or empty", nameof(id));
            }

            return await _http.GetFromJsonAsync<List<JobEvent>>($"api/jobs/{id}/events");
        }
    }
}

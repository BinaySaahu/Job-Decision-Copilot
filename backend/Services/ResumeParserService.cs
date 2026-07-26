using System.Net.Http.Json;
using System.Text.Json;
using JobDecisionEngine.Models;

namespace JobDecisionEngine.Services
{
    public class ResumeParserService : IResumeParserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ResumeParserService> _logger;

        public ResumeParserService(HttpClient httpClient, ILogger<ResumeParserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ParsedResume?> ParseResumeTextAsync(string resumeText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(resumeText))
            {
                throw new ArgumentException("Resume text cannot be empty.", nameof(resumeText));
            }

            var request = new { resume_text = resumeText };
            _logger.LogInformation("Sending resume text to AI parser.");

            var response = await _httpClient.PostAsJsonAsync("parse-resume", request, cancellationToken);
            Console.WriteLine($"Response status code: {response.StatusCode}");
            if(!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to parse resume. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to parse resume. Status Code: {response.StatusCode}, Response: {errorContent}");
                // return null;
            }

            var parsedResume = await response.Content.ReadFromJsonAsync<ParsedResume>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }, cancellationToken);

            if (parsedResume == null)
            {
                throw new InvalidOperationException("AI parser returned an invalid or empty response.");
            }

            return parsedResume;
        }
    }
}

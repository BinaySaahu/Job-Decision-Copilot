using JobDecisionEngine.Models;

namespace JobDecisionEngine.Services
{
    public interface IResumeParserService
    {
        Task<ParsedResume?> ParseResumeTextAsync(string resumeText, CancellationToken cancellationToken = default);
    }
}

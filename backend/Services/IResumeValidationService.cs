using JobDecisionEngine.Models;

namespace JobDecisionEngine.Services
{
    public interface IResumeValidationService
    {
        Task<ParsedResume> ValidateAndNormalizeAsync(ParsedResume parsedResume, CancellationToken cancellationToken = default);
    }
}

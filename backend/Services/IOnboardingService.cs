using JobDecisionEngine.DTOs;
using JobDecisionEngine.Models;

namespace JobDecisionEngine.Services
{
    public interface IOnboardingService
    {
        Task<OnboardingResponse?> SaveOnboardingAsync(OnboardingRequest request);
        Task<OnboardingResponse?> GetOnboardingProfileAsync(int userId);
    }
}

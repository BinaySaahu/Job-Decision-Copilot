using JobDecisionEngine.DTOs;
using JobDecisionEngine.Models;

namespace JobDecisionEngine.Services
{
    public interface IOnboardingService
    {
        Task<bool?> SaveOnboardingAsync(OnboardingRequest request);
        Task<OnboardingResponse?> GetOnboardingProfileAsync(int userId);
    }
}

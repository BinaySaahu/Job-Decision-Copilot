using Microsoft.AspNetCore.Http;

namespace JobDecisionEngine.DTOs
{
    public class OnboardingRequest
    {
        public int UserId { get; set; }
        public string? ExperienceYears { get; set; }
        public List<string>? InterestedRoles { get; set; }
        public string? EmploymentType { get; set; }
        public IFormFile? Resume { get; set; }
    }
}

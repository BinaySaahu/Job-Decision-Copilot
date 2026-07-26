using Microsoft.AspNetCore.Http;
using JobDecisionEngine.Models;

namespace JobDecisionEngine.DTOs
{
    public class OnboardingResponse
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public bool ISOnboarded { get; set; }
        public bool IsActive { get; set; }
        public string? ExperienceYears { get; set; }
        public string? ResumeUrl { get; set; }
        public ResumeParseStatus ParseStatus { get; set; }
        public string? ParsedResumeJson { get; set; }
        public List<string>? Warnings { get; set; }
        public string? LastParseError { get; set; }
        public List<string>? InterestedRoles { get; set; }
        public string? EmploymentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

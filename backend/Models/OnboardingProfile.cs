using System;

namespace JobDecisionEngine.Models
{
    public class OnboardingProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ExperienceYears { get; set; }
        public string? ResumeUrl { get; set; }
        public List<string>? InterestedRoles { get; set; }
        public string? EmploymentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
    }
}

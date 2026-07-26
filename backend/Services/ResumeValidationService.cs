using JobDecisionEngine.Models;

namespace JobDecisionEngine.Services
{
    public class ResumeValidationService : IResumeValidationService
    {
        private static readonly Dictionary<string, string> SkillNormalization = new(StringComparer.OrdinalIgnoreCase)
        {
            { "spring", "Spring Boot" },
            { "spring boot", "Spring Boot" },
            { "java", "Java" },
            { "sql", "SQL" },
            { "docker", "Docker" },
            { "aws", "AWS" },
            { "c#", "C#" },
            { "dotnet", ".NET" }
        };

        private static readonly Dictionary<string, string> RoleNormalization = new(StringComparer.OrdinalIgnoreCase)
        {
            { "backend developer", "Backend Developer" },
            { "software engineer", "Software Engineer" },
            { "full stack developer", "Full Stack Developer" },
            { "devops engineer", "DevOps Engineer" }
        };

        public Task<ParsedResume> ValidateAndNormalizeAsync(ParsedResume parsedResume, CancellationToken cancellationToken = default)
        {
            if (parsedResume == null)
            {
                throw new ArgumentNullException(nameof(parsedResume));
            }

            if (parsedResume.Skills == null || !parsedResume.Skills.Any())
            {
                throw new InvalidOperationException("Parsed resume must include at least one skill.");
            }

            if (parsedResume.Experience == null)
            {
                throw new InvalidOperationException("Parsed resume must include years of experience.");
            }

            parsedResume.Skills = NormalizeList(parsedResume.Skills, SkillNormalization);
            parsedResume.Roles = NormalizeList(parsedResume.Roles, RoleNormalization);
            parsedResume.Education = NormalizeList(parsedResume.Education);
            parsedResume.Certifications = NormalizeList(parsedResume.Certifications);

            return Task.FromResult(parsedResume);
        }

        private static List<string>? NormalizeList(List<string>? values, Dictionary<string, string>? normalization = null)
        {
            if (values == null || !values.Any())
            {
                return values;
            }

            var normalized = values
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Select(x => normalization != null && normalization.TryGetValue(x, out var canonical) ? canonical : x)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return normalized.Any() ? normalized : null;
        }
    }
}

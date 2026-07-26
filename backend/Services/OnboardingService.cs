using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JobDecisionEngine.Data;
using JobDecisionEngine.DTOs;
using JobDecisionEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace JobDecisionEngine.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IS3Service _s3Service;
        private readonly IResumeParserService _resumeParserService;

        private readonly ILogger<OnboardingService> _logger;

        public OnboardingService(
            AppDbContext context,
            IConfiguration configuration,
            IS3Service s3Service,
            IResumeParserService resumeParserService,
            ILogger<OnboardingService> logger)
        {
            _context = context;
            _configuration = configuration;
            _s3Service = s3Service;
            _resumeParserService = resumeParserService;
            _logger = logger;
        }

        public async Task<OnboardingResponse?> SaveOnboardingAsync(OnboardingRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                return null;
            }

            string? resumeUrl = null;
            string? resumeKey = null;

            if (request.Resume == null || request.Resume.Length <= 0)
            {
                _logger.LogInformation("No resume file provided for user ID {UserId}.", request.UserId);
                return null;
            }
            
                _logger.LogInformation("Uploading resume for user ID {UserId}.", request.UserId);
                resumeUrl = await _s3Service.UploadFileAsync(request.Resume, request.UserId.ToString());
                resumeKey = new Uri(resumeUrl).AbsolutePath.TrimStart('/');

            var profile = await _context.OnboardingProfiles
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (profile == null)
            {
                profile = new OnboardingProfile
                {
                    UserId = request.UserId,
                    ExperienceYears = request.ExperienceYears,
                    ResumeUrl = resumeUrl,
                    InterestedRoles = request.InterestedRoles,
                    EmploymentType = request.EmploymentType,
                    ParseStatus = string.IsNullOrWhiteSpace(resumeUrl) ? ResumeParseStatus.NotStarted : ResumeParseStatus.Pending
                };

                _context.OnboardingProfiles.Add(profile);
            }
            else
            {
                profile.ExperienceYears = request.ExperienceYears;
                profile.ResumeUrl = string.IsNullOrWhiteSpace(resumeUrl) ? profile.ResumeUrl : resumeUrl;
                profile.InterestedRoles = request.InterestedRoles;
                profile.EmploymentType = request.EmploymentType;
                profile.UpdatedAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(resumeUrl))
                {
                    profile.ParseStatus = ResumeParseStatus.Pending;
                }
            }

            user.IsOnBoarded = true;
            user.UpdatedAt = DateTime.UtcNow;
            // await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(resumeKey))
            {
                try
                {
                    var resumeText = await _s3Service.ExtractTextFromS3Async(resumeKey);
                    var parsedResume = await _resumeParserService.ParseResumeTextAsync(resumeText);


                    profile.ParsedResumeJson = JsonSerializer.Serialize(parsedResume, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    });
                    profile.ParseStatus = ResumeParseStatus.Completed;
                    profile.ParsedAt = DateTime.UtcNow;
                    profile.LastParseError = null;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully parsed resume for user ID {UserId}.", request.UserId);
                    return new OnboardingResponse
                    {
                        UserId = user.Id,
                        UserName = user.FullName,
                        UserEmail = user.Email,
                        ISOnboarded = user.IsOnBoarded,
                        IsActive = user.IsActive,
                        ExperienceYears = profile.ExperienceYears,
                        ResumeUrl = profile.ResumeUrl,
                        ParseStatus = profile.ParseStatus,
                        ParsedResumeJson = profile.ParsedResumeJson,
                        Warnings = ExtractWarnings(profile.ParsedResumeJson),
                        LastParseError = profile.LastParseError,
                        InterestedRoles = profile.InterestedRoles,
                        EmploymentType = profile.EmploymentType,
                        CreatedAt = profile.CreatedAt,
                        UpdatedAt = profile.UpdatedAt
                    };
                }
                catch (Exception ex)
                {
                    // Console.WriteLine($"Error parsing resume: {ex.Message}");
                    return null;
                    // profile.ParseStatus = ResumeParseStatus.Failed;
                    // profile.LastParseError = ex.Message;
                    
                }


            }
            _logger.LogError("Could not save resume correctly in S3 for user ID {UserId}.", request.UserId);
            return null;
        }

        public async Task<OnboardingResponse?> GetOnboardingProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            var onBoarding = await _context.OnboardingProfiles.FirstOrDefaultAsync(x => x.UserId == userId);

            return new OnboardingResponse
            {
                UserId = user.Id,
                UserName = user.FullName,
                UserEmail = user.Email,
                ISOnboarded = user.IsOnBoarded,
                IsActive = user.IsActive,
                ExperienceYears = onBoarding?.ExperienceYears,
                ResumeUrl = onBoarding?.ResumeUrl,
                ParseStatus = onBoarding?.ParseStatus ?? ResumeParseStatus.NotStarted,
                ParsedResumeJson = onBoarding?.ParsedResumeJson,
                Warnings = ExtractWarnings(onBoarding?.ParsedResumeJson),
                LastParseError = onBoarding?.LastParseError,
                InterestedRoles = onBoarding?.InterestedRoles,
                EmploymentType = onBoarding?.EmploymentType,
                CreatedAt = onBoarding?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = onBoarding?.UpdatedAt ?? DateTime.UtcNow
            };
        }

        private static List<string>? ExtractWarnings(string? parsedResumeJson)
        {
            if (string.IsNullOrWhiteSpace(parsedResumeJson))
            {
                return null;
            }

            try
            {
                using var document = JsonDocument.Parse(parsedResumeJson);
                if (document.RootElement.TryGetProperty("warnings", out var warningsElement) && warningsElement.ValueKind == JsonValueKind.Array)
                {
                    var warnings = new List<string>();
                    foreach (var warning in warningsElement.EnumerateArray())
                    {
                        if (warning.ValueKind == JsonValueKind.String)
                        {
                            warnings.Add(warning.GetString()!);
                        }
                    }

                    return warnings.Count > 0 ? warnings : null;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}

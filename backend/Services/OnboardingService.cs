using System;
using System.IO;
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

        public OnboardingService(AppDbContext context, IConfiguration configuration, IS3Service s3Service)
        {
            _context = context;
            _configuration = configuration;
            _s3Service = s3Service;
        }

        public async Task<bool?> SaveOnboardingAsync(OnboardingRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return null;
            }

            string? resumeUrl = null;

            if (request.Resume != null && request.Resume.Length > 0)
            {

                var bucketName = _configuration["Aws:S3:BucketName"];
                var baseUrl = _configuration["Aws:S3:BaseUrl"];

                if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(baseUrl))
                {
                    throw new InvalidOperationException("AWS S3 configuration is missing.");
                }

                resumeUrl = await _s3Service.UploadFileAsync(request.Resume, $"resumes_{request.UserId}");
            }

            var existing = await _context.OnboardingProfiles
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (existing == null)
            {
                var profile = new OnboardingProfile
                {
                    UserId = request.UserId,
                    ExperienceYears = request.ExperienceYears,
                    ResumeUrl = resumeUrl,
                    InterestedRoles = request.InterestedRoles,
                    EmploymentType = request.EmploymentType,
                };

                _context.OnboardingProfiles.Add(profile);
            }
            else
            {
                existing.ExperienceYears = request.ExperienceYears;
                existing.ResumeUrl = string.IsNullOrWhiteSpace(resumeUrl) ? existing.ResumeUrl : resumeUrl;
                existing.InterestedRoles = request.InterestedRoles;
                existing.EmploymentType = request.EmploymentType;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            user.IsOnBoarded = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            // var onBoarding = await _context.OnboardingProfiles.FirstOrDefaultAsync(x => x.UserId == request.UserId);
            // return new OnboardingResponse
            // {
            //     UserId = user.Id,
            //     UserName = user.FullName,
            //     UserEmail = user.Email,
            //     ISOnboarded = user.IsOnBoarded,
            //     IsActive = user.IsActive,
            //     ExperienceYears = onBoarding?.ExperienceYears,
            //     ResumeUrl = onBoarding?.ResumeUrl,
            //     InterestedRoles = onBoarding?.InterestedRoles,
            //     EmploymentType = onBoarding?.EmploymentType,
            //     CreatedAt = onBoarding?.CreatedAt ?? DateTime.UtcNow,
            //     UpdatedAt = onBoarding?.UpdatedAt ?? DateTime.UtcNow
            // };
            return true;
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
                InterestedRoles = onBoarding?.InterestedRoles,
                EmploymentType = onBoarding?.EmploymentType,
                CreatedAt = onBoarding?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = onBoarding?.UpdatedAt ?? DateTime.UtcNow
            };
        }
    }
}

using JobDecisionEngine.DTOs;
using JobDecisionEngine.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobDecisionEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] OnboardingRequest request)
        {
            if (request.UserId <= 0)
            {
                return BadRequest(new { message = "A valid user id is required." });
            }

            try
            {
                var rslt = await _onboardingService.SaveOnboardingAsync(request);

                if (rslt == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(new
                {
                    message = "Onboarding completed successfully.",
                    // profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getDetails/{userId}")]
        public async Task<IActionResult> Get([FromRoute] int userId)
        {
            var profile = await _onboardingService.GetOnboardingProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { message = "Onboarding profile not found." });
            }

            return Ok(profile);
        }
    }
}

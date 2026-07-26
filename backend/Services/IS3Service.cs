
namespace JobDecisionEngine.Services
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string fileUrl);
        Task<string> ExtractTextFromS3Async(string key);
    }
    
}
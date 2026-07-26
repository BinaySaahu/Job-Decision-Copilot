using Amazon.S3;
using Amazon.S3.Model;
using UglyToad.PdfPig;
using System.Text;

namespace JobDecisionEngine.Services
{

    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public S3Service(
            IAmazonS3 s3Client,
            IConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }


        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            var bucketName = _configuration["Aws:S3:BucketName"];
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new InvalidOperationException("AWS S3 bucket configuration is missing.");
            }
            
            var fileName = $"resumes/{folder}/{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";

            using var stream = file.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(request);

            return $"https://{bucketName}.s3.amazonaws.com/{fileName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var bucketName = _configuration["Aws:S3:BucketName"];
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new InvalidOperationException("AWS S3 bucket configuration is missing.");
            }

            var key = new Uri(fileUrl).AbsolutePath.TrimStart('/');

            await _s3Client.DeleteObjectAsync(bucketName, key);
        }

        public async Task<string> ExtractTextFromS3Async(string key)
        {
            var bucketName = _configuration["Aws:S3:BucketName"];
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new InvalidOperationException("AWS S3 bucket configuration is missing.");
            }

            var response = await _s3Client.GetObjectAsync(bucketName, key);

            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);

            using var document = PdfDocument.Open(memoryStream.ToArray());

            var text = new StringBuilder();
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }

            return text.ToString();
        }
    }
}
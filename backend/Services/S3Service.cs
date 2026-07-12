using Amazon.S3;
using Amazon.S3.Model;

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


        public async Task<string> UploadFileAsync(
            IFormFile file,
            string folder)
        {
            var bucketName = _configuration["AWS:S3:BucketName"];

            var fileName =
                $"{folder}/{Guid.NewGuid():N}_{file.FileName}";


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
            var bucketName = _configuration["AWS:BucketName"];

            var key = new Uri(fileUrl).AbsolutePath.TrimStart('/');


            await _s3Client.DeleteObjectAsync(
                bucketName,
                key
            );
        }
    }
}
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;

namespace Utils
{
    public partial class Startup
    {
        public static void UseAmazonS3(WebApplicationBuilder builder)
        {
            // Register the IAmazonS3 client with the configuration from appsettings
            builder.Services.AddSingleton<IAmazonS3>(provider =>
            {
                var accessKey = builder.Configuration["AWS:AccessKey"];
                var secretKey = builder.Configuration["AWS:SecretKey"];
                var region = builder.Configuration["AWS:Region"];

                if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(region))
                {
                    throw new InvalidOperationException("AWS S3 configuration is missing or incomplete.");
                }

                return new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
            });

            // Register the S3Helper service
            builder.Services.AddScoped<AmazonS3Helper>();
        }
    }

    public class AmazonS3Helper
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AmazonS3Helper(IConfiguration configuration, IAmazonS3 s3Client)
        {
            _bucketName = configuration["AWS:BucketName"] ?? throw new ArgumentNullException("AWS:BucketName");
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var newFileName = $"{fileNameWithoutExtension}_{timestamp}{fileExtension}";

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = newFileName,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return newFileName;
        }

        public async Task<List<object>> UploadFilesAsync(List<IFormFile> files)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("File list cannot be null or empty.");

            var results = new List<object>();
            var transferUtility = new TransferUtility(_s3Client);

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    results.Add(new { FileName = file.FileName, Status = "Failed: Empty file." });
                    continue;
                }

                try
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var newFileName = $"{fileNameWithoutExtension}_{timestamp}{fileExtension}";

                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = memoryStream,
                        Key = newFileName,
                        BucketName = _bucketName,
                        ContentType = file.ContentType
                    };

                    await transferUtility.UploadAsync(uploadRequest);
                    results.Add(new { FileName = newFileName, Status = "Success" });
                }
                catch (Exception ex)
                {
                    results.Add(new { FileName = file.FileName, Status = $"Failed: {ex.Message}" });
                }
            }

            return results;
        }

        public async Task<FileContentResult> GetFileImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty.");

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                using var response = await _s3Client.GetObjectAsync(request);
                using var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // แปลงข้อมูลเป็น byte array สำหรับการส่งเป็นไฟล์
                var fileBytes = memoryStream.ToArray();
                var contentType = response.Headers["Content-Type"] ?? "application/octet-stream";

                return new FileContentResult(fileBytes, contentType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw new Exception($"Error getting file from S3: {ex.Message}");
            }
        }





        public async Task<IActionResult> GetFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return new BadRequestObjectResult(new { Message = "File name cannot be null or empty." });

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                var response = await _s3Client.GetObjectAsync(request);
                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var contentType = response.Headers["Content-Type"] ?? "application/octet-stream";

                // คืนค่าเป็นไฟล์โดยตรง
                return new FileStreamResult(memoryStream, contentType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult(new { Message = "File not found in S3 bucket." });
                }
                return new ObjectResult(new { Message = $"Error getting file from S3: {ex.Message}" }) { StatusCode = 500 };
            }
        }


        public async Task<List<object>> GetFilesAsync(List<string> fileNames)
        {
            if (fileNames == null || !fileNames.Any())
                throw new ArgumentException("File name list cannot be null or empty.");

            var results = new List<object>();

            foreach (var fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    results.Add(new { FileName = fileName, Status = "Failed: Empty or invalid file name." });
                    continue;
                }

                try
                {
                    var stream = await GetFileAsync(fileName);
                    if (stream != null)
                    {
                        results.Add(new { FileName = fileName, Status = "Success", Stream = stream });
                    }
                    else
                    {
                        results.Add(new { FileName = fileName, Status = "Failed: File not found." });
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new { FileName = fileName, Status = $"Failed: {ex.Message}" });
                }
            }

            return results;
        }

        public class GetFileUrlAsync_Result
        {

            public string? url { get; set; }
            public string? message { get; set; }
            public bool status { get; set; }

        }
        public async Task<GetFileUrlAsync_Result> GetFileUrlAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty.");

            try
            {
                // ตรวจสอบว่าไฟล์มีอยู่จริงใน S3 หรือไม่
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                await _s3Client.GetObjectMetadataAsync(request);


                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Expires = DateTime.UtcNow.AddHours(12)
                };

                var preSignedUrl = _s3Client.GetPreSignedURL(urlRequest);

                // คืนค่าในรูปแบบ JSON
                return new GetFileUrlAsync_Result
                {
                    status = true,
                    url = preSignedUrl
                };
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new GetFileUrlAsync_Result
                {
                    status = false,
                    message = "File not found in S3 bucket."
                };
            }
            catch (Exception ex)
            {
                return new GetFileUrlAsync_Result
                {
                    status = false,
                    message = $"Error getting file from S3: {ex.Message}"
                };
            }
        }


        public async Task<bool> DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty.");

            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception ex)
            {
                // Log Amazon S3-specific errors if needed
                throw new Exception($"Error deleting file from S3: {ex.Message}");
            }
        }

        public async Task<List<object>> DeleteFilesAsync(List<string> fileNames)
        {
            if (fileNames == null || !fileNames.Any())
                throw new ArgumentException("File name list cannot be null or empty.");

            var results = new List<object>();

            foreach (var fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    results.Add(new { FileName = fileName, Status = "Failed: Empty or invalid file name." });
                    continue;
                }

                try
                {
                    var success = await DeleteFileAsync(fileName);
                    results.Add(new { FileName = fileName, Status = success ? "Success" : "Failed" });
                }
                catch (Exception ex)
                {
                    results.Add(new { FileName = fileName, Status = $"Failed: {ex.Message}" });
                }
            }

            return results;
        }


    }
}

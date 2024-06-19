using Amazon.S3;
using Amazon.S3.Model;
using DataShareCore.Common.Enum;

namespace WebApplication1.Service.IS3Service;

public class S3Service: IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = ConfigEnum.S3BucketName;

    public S3Service(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task UploadFileAsync(string keyName, Stream inputStream)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName,
            InputStream = inputStream
        };

        await _s3Client.PutObjectAsync(putRequest);
    }
    
    public async Task UploadTextAsync(string fileName, string content)
    {

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            ContentBody = content,
            ContentType = "text/plain" // Set content type appropriately
        };

        await _s3Client.PutObjectAsync(putRequest);
    }

    public async Task<Stream> DownloadFileAsync( string keyName)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName
        };

        var response = await _s3Client.GetObjectAsync(getRequest);
        return response.ResponseStream;
    }
    
    public async Task DeleteFileAsync(string keyName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }
}
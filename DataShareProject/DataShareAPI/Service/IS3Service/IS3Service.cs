namespace WebApplication1.Service.IS3Service;

public interface IS3Service
{
    Task UploadFileAsync(string keyName, Stream inputStream);
    Task UploadTextAsync(string fileName, string content);
    Task<Stream> DownloadFileAsync(string keyName);
    Task<bool> IsExists(string fileKey);
    Task DeleteFileAsync(string keyName); 
}
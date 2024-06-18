using DataShareCore.Models;

namespace WebApplication1.Service.FileStoreService;

public interface ITextStoreService
{
    Task<TextStore> UploadText(TextStore content);
    Task<TextStore> GetById(int id);

    //void DeleteFile(FileStore fileData);
    void DeleteFile(int fileId);
    //void DeleteFile(string name);

}
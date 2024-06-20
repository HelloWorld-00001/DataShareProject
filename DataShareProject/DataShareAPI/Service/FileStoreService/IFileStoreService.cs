using DataShareCore.Models;

namespace WebApplication1.Service.FileStoreService;

public interface IFileStoreService
{
    Task<FileStore> UploadFile(FileStore fileData);
    Task<FileStore> GetByName(string name);
    Task<FileStore> GetById(int id);

    //void DeleteFile(FileStore fileData);
    void DeleteFile(int fileId);
    //void DeleteFile(string name);

}
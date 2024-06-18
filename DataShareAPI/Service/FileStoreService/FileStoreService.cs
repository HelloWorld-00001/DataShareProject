using System.Collections;
using System.Security.Claims;
using DataShareCore.Dtos;
using DataShareCore.Mapper;
using DataShareCore.Models;
using DataShareData;
using DataShareData.Repository.AccountRepoFolder;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Service.FileStoreService;

public class FileStoreService: IFileStoreService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFileStoreRepo _dataContext;

    public FileStoreService(IHttpContextAccessor httpContextAccessor, IFileStoreRepo dataContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataContext = dataContext;
    }

    public async Task<FileStore> UploadFile(FileStore fileData)
    {
        return await _dataContext.CreateAsync(fileData);
    }

    public async Task<FileStore> GetByName(string name)
    {
        return await _dataContext.GetByName(name);
    }

    public async Task<FileStore> GetById(int id)
    {
        return await _dataContext.GetByIdAsync(id);
    }
    
    public  void DeleteFile(int fileId)
    {
         _dataContext.Delete(fileId);

    }
    

}
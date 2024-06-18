using System.Collections;
using System.Security.Claims;
using DataShareCore.Dtos;
using DataShareCore.Mapper;
using DataShareCore.Models;
using DataShareData;
using DataShareData.Repository.AccountRepoFolder;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Service.FileStoreService;

public class TextStoreService: ITextStoreService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITextStoreRepo _dataContext;

    public TextStoreService(IHttpContextAccessor httpContextAccessor, ITextStoreRepo dataContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataContext = dataContext;
    }

    public async Task<TextStore> UploadText(TextStore content)
    {
        return await _dataContext.CreateAsync(content);
    }

   

    public async Task<TextStore> GetById(int id)
    {
        return await _dataContext.GetByIdAsync(id);
    }
    
    public  void DeleteFile(int fileId)
    {
         _dataContext.Delete(fileId);

    }
    

}
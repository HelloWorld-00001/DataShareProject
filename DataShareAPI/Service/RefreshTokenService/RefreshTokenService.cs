using DataShareCore.Models;
using DataShareData.Repository.AccountRepoFolder;

namespace WebApplication1.Service.AccountService;

public class RefreshTokenService: IRefreshTokenService
{
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRefreshTokenRepo _dataContext;

    public RefreshTokenService(IHttpContextAccessor httpContextAccessor, IRefreshTokenRepo dataContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataContext = dataContext;
    }
    
    public async Task<RefreshToken> GetByUserId(int userId)
    {
        return await _dataContext.GetByUserId(userId);
    }

    public async Task<RefreshToken> AddOne(RefreshToken acc)
    {
        return await _dataContext.CreateAsync(acc);
    }

    public async Task<RefreshToken> GetById(int id)
    {
        return await _dataContext.GetByIdAsync(id);
    }

    public async Task<RefreshToken> UpdateToken(RefreshToken acc)
    {
        return await _dataContext.UpdateAsync(acc);
    }
    

    public void DeleteToken(int id)
    {
         _dataContext.Delete(id);
    }
}
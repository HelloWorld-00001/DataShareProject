using DataShareCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DataShareData.Repository.AccountRepoFolder;

public class RefreshTokenRepo: RepositoryBase<RefreshToken>, IRefreshTokenRepo
{
    readonly DataContext _dataContext;
    public RefreshTokenRepo(DataContext dbContext) : base(dbContext)
    {
        _dataContext = dbContext;
    }
    public async Task<RefreshToken> GetByUserId(int userId)
    {
        return await _dataContext.RefreshTokens.Where(e => e.userId == userId).FirstOrDefaultAsync();
    }

    public async Task<int> GetUserByToken(string token)
    {
        var userId = await _dataContext.RefreshTokens
            .Where(e => e.token == token)
            .Select(e => e.userId)
            .FirstOrDefaultAsync();

        return userId == default(int) ? -1 : userId;
    }

}
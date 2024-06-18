namespace DataShareData.Repository.AccountRepoFolder;

using DataShareCore.Models;


public interface IRefreshTokenRepo: IRepositoryBase<RefreshToken>
{
    Task<RefreshToken> GetByUserId(int userId);

}
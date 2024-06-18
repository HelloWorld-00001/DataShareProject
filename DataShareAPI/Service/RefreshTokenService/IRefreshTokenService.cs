using DataShareCore.Dtos;
using DataShareCore.Models;

namespace WebApplication1.Service.AccountService;

public interface IRefreshTokenService
{
    Task<RefreshToken> GetByUserId(int userId);
    Task<RefreshToken> AddOne(RefreshToken acc);
    Task<RefreshToken> GetById(int id);
    Task<RefreshToken> UpdateToken(RefreshToken acc);
    

    void DeleteToken(int id);

}
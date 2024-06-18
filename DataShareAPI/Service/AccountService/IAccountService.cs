using DataShareCore.Dtos;
using DataShareCore.Models;

namespace WebApplication1.Service.AccountService;

public interface IAccountService
{
    Task<Account> AddAccount(Account acc);
    Task<Account> GetByEmail(string username);
    Task<Account> GetById(int id);
    Task<Account> UpdateAccount(Account acc);
    
    Task<List<Account>> GetAllAccount();

    Task<int> GetNextAccountId();
}
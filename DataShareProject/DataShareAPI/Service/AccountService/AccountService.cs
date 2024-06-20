using System.Collections;
using System.Security.Claims;
using DataShareCore.Dtos;
using DataShareCore.Mapper;
using DataShareCore.Models;
using DataShareData;
using DataShareData.Repository.AccountRepoFolder;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Service.AccountService;

public class AccountService: IAccountService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccountRepo _dataContext;

    public AccountService(IHttpContextAccessor httpContextAccessor, IAccountRepo dataContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataContext = dataContext;
    }

    public async Task<Account> AddAccount(Account acc)
    {
        var res = await _dataContext.CreateAsync(acc);
        return res;
    }
    
    
    public async Task<List<Account>> GetAllAccount()
    {
        var accountList =  await _dataContext.GetAllAsync();
        return accountList.ToList();
    }


    public async Task<Account> GetByEmail(string email)
    {
        return await _dataContext.GetByEmail(email);
    }

    public async Task<Account> UpdateAccount(Account acc)
    {
        return await _dataContext.UpdateAsync(acc);
    }

    public async Task<Account> GetById(int id)
    {
        return await _dataContext.GetByIdAsync(id);
    }

    public async Task<int> GetNextAccountId()
    {
        return await _dataContext.GetNextAccountIdAsync();
    }
}
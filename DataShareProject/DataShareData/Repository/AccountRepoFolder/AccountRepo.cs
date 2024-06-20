using System.Text;
using DataShareCore.Models;

namespace DataShareData.Repository.AccountRepoFolder;

public class AccountRepo : RepositoryBase<Account>, IAccountRepo
{
    readonly DataContext _dataContext;
    public AccountRepo(DataContext dbContext) : base(dbContext)
    {
        _dataContext = dbContext;
    }

    public async Task<Account> GetByEmail(string email)
    {
        var account = _dataContext.Account.Where(e => e.email == email).FirstOrDefault();
        return account;
    }
    

    public async Task<int> GetNextAccountIdAsync()
    {
        return 0;
    }
}
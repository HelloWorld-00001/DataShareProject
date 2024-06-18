namespace DataShareData.Repository.AccountRepoFolder;

using DataShareCore.Models;


public interface IAccountRepo: IRepositoryBase<Account>
{
    Task<Account> GetByEmail(string email);
    Task<int> GetNextAccountIdAsync();
}
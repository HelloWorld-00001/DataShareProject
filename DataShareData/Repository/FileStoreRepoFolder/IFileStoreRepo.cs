namespace DataShareData.Repository.AccountRepoFolder;

using DataShareCore.Models;


public interface IFileStoreRepo: IRepositoryBase<FileStore>
{
    Task<FileStore> GetByName(string name);
    Task<int> GetNextAccountIdAsync();
}
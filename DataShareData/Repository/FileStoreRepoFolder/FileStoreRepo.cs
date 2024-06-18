using System.Text;
using DataShareCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DataShareData.Repository.AccountRepoFolder;

public class FileStoreRepo : RepositoryBase<FileStore>, IFileStoreRepo
{
    readonly DataContext _dataContext;
    public FileStoreRepo(DataContext dbContext) : base(dbContext)
    {
        _dataContext = dbContext;
    }

    public async Task<FileStore> GetByName(string name)
    {
        return await _dataContext.FileStores.Where(e => e.fileName == name).FirstOrDefaultAsync();
    }
    

    public async Task<int> GetNextAccountIdAsync()
    {
        return 0;
    }
}
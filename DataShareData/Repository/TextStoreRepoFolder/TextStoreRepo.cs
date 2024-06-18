using System.Text;
using DataShareCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DataShareData.Repository.AccountRepoFolder;

public class TextStoreRepo : RepositoryBase<TextStore>, ITextStoreRepo
{
    readonly DataContext _dataContext;
    public TextStoreRepo(DataContext dbContext) : base(dbContext)
    {
        _dataContext = dbContext;
    }
    
}
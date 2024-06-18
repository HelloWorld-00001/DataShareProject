using DataShareCore.Dtos;
using DataShareCore.Models;

namespace DataShareCore.Mapper;

public class AccountMapper
{
    public static Account AccountDtos2Account(AccountDtos accountDtos)
    {
        return new Account(accountDtos.email, accountDtos.password);
    }

    public static AccountDtos Account2AccountDtos(Account account)
    {
        return new AccountDtos(account.email, account.password);
    }
}
namespace DataShareCore.Dtos;

public class AccountDtos
{
  
        
    public string email { get; set; }
    public string password { get; set; }
    
    public AccountDtos(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
    
    
}
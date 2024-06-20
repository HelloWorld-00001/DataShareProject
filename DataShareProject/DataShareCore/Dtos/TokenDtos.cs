using System.ComponentModel.DataAnnotations;

namespace DataShareCore.Dtos;

public class TokenDtos
{
    [Required]
    public string accessToken { get; set; }
    [Required]
    public string refreshToken { get; set; }
}
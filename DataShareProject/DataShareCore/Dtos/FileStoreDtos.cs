using System.ComponentModel.DataAnnotations;

namespace DataShareCore.Dtos;
using Microsoft.AspNetCore.Http;

public class FileStoreDtos
{
    [Required]
    public IFormFile fileControl { get; set; }
    [Required]

    public bool autoDelete { get; set; }
}
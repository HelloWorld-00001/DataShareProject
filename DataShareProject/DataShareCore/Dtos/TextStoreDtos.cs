using System.ComponentModel.DataAnnotations;

namespace DataShareCore.Dtos;
using Microsoft.AspNetCore.Http;

public class TextStoreDtos
{
    [Required]
    public string content { get; set; }
    [Required]
    public bool autoDelete { get; set; }
}
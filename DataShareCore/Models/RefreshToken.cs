using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataShareCore.Models;

[Table("RefreshToken")]
public class RefreshToken: ModelBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    
    public string token { get; set; } = string.Empty;
    public DateTime createTime { get; set; } = DateTime.UtcNow;
    public DateTime expiredTime { get; set; }
    [Required]
    public int userId { get; set; }
    
}
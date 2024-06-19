using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DataShareCore.Models;

[Table("Account")]
public class Account : ModelBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    [Required]
    public string password { get; set; }
    [Required]
    public string email { get; set; }
    
    [Column("salt")]
    public string passwordSalt { get; set; }

    
}
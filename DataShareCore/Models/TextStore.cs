using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataShareCore.Models;

[Table("TextStore")]
public class TextStore: ModelBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public string content { get; set; }
    public DateTime createdAt { get; set; }
    public bool autoDelete { get; set; }
    public int owner { get; set; }
    
}
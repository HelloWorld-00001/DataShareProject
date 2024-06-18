using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataShareCore.Models;

[Table("FileStore")]
public class FileStore: ModelBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public string fileName { get; set; }
    public Byte[] fileData { get; set; }
    public DateTime uploadedAt { get; set; }
    public int fileSize { get; set; }
    public bool autoDelete { get; set; }
    public int owner { get; set; }
    
}
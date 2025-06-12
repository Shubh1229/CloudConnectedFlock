using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Models;

[Table("securityqa")]
public partial class Securityqa
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("securitykey")]
    public List<byte[]> Securitykey { get; set; } = null!;

    [Column("securityhasharray")]
    public List<byte[]> Securityhasharray { get; set; } = null!;

    public virtual ICollection<Useraccount> Useraccounts { get; set; } = new List<Useraccount>();
}
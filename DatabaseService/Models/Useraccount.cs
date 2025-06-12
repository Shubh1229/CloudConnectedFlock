using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Models;

[Table("useraccounts")]
public partial class Useraccount
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = null!;

    [Column("birthday")]
    public DateOnly Birthday { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("passwordkey")]
    public byte[] Passwordkey { get; set; } = null!;

    [Column("passwordhash")]
    public byte[] Passwordhash { get; set; } = null!;

    [Column("securityqaid")]
    public Guid Securityqaid { get; set; }

    public virtual Securityqa Securityqa { get; set; } = null!;

    public virtual Userprofile? Userprofile { get; set; }
}
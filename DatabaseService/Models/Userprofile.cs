using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Models;

[Table("userprofiles")]
public partial class Userprofile
{
    [Column("id")]
    public int Id { get; set; }

    [Column("accountid")]
    public Guid Accountid { get; set; }

    [Column("financeid")]
    public Guid Financeid { get; set; }

    [Column("firstname")]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    public string Lastname { get; set; } = null!;

    [Column("bio")]
    public string Bio { get; set; } = null!;

    [Column("personallinks")]
    public List<string> Personallinks { get; set; } = null!;

    [Column("resumefilepath")]
    public string? Resumefilepath { get; set; }

    [Column("profilepicturepath")]
    public string Profilepicturepath { get; set; } = null!;

    public virtual Useraccount Account { get; set; } = null!;
    public virtual Userfinance Finance { get; set; } = null!;
}
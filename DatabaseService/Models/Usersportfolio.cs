using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Models;

[Table("usersportfolio")]
public partial class Usersportfolio
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("value")]
    public double Value { get; set; }

    public virtual ICollection<Userfinance> Userfinances { get; set; } = new List<Userfinance>();
    public virtual ICollection<Stockdatum> Stocks { get; set; } = new List<Stockdatum>();
}
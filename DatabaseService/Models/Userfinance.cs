using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Models;

[Table("userfinances")]
public partial class Userfinance
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("annualcashflush")]
    public DateTime Annualcashflush { get; set; }

    [Column("liquidcash")]
    public double Liquidcash { get; set; }

    [Column("stockportfoliovalue")]
    public double Stockportfoliovalue { get; set; }

    [Column("stockportfolioid")]
    public Guid Stockportfolioid { get; set; }

    public virtual Usersportfolio Stockportfolio { get; set; } = null!;
    public virtual Userprofile? Userprofile { get; set; }
}
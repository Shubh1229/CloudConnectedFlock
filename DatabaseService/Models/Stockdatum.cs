using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Models;

[Table("stockdata")]
public partial class Stockdatum
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("currentvalue")]
    public double Currentvalue { get; set; }

    [Column("dailychange")]
    public double Dailychange { get; set; }

    [Column("historicvalues")]
    public List<double> Historicvalues { get; set; } = null!;

    public virtual ICollection<Usersportfolio> Portfolios { get; set; } = new List<Usersportfolio>();
}
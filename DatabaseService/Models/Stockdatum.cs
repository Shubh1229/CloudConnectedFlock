using System;
using System.Collections.Generic;

namespace DatabaseService.Models;

public partial class Stockdatum
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public double Currentvalue { get; set; }

    public double Dailychange { get; set; }

    public List<double> Historicvalues { get; set; } = null!;

    public virtual ICollection<Usersportfolio> Portfolios { get; set; } = new List<Usersportfolio>();
}

using System;
using System.Collections.Generic;

namespace DatabaseService.Models;

public partial class Userfinance
{
    public Guid Id { get; set; }

    public DateTime Annualcashflush { get; set; }

    public double Liquidcash { get; set; }

    public double Stockportfoliovalue { get; set; }

    public Guid Stockportfolioid { get; set; }

    public virtual Usersportfolio Stockportfolio { get; set; } = null!;

    public virtual Userprofile? Userprofile { get; set; }
}

using System;
using System.Collections.Generic;

namespace DatabaseService.Models;

public partial class Usersportfolio
{
    public Guid Id { get; set; }

    public double Value { get; set; }

    public virtual ICollection<Userfinance> Userfinances { get; set; } = new List<Userfinance>();

    public virtual ICollection<Stockdatum> Stocks { get; set; } = new List<Stockdatum>();
}

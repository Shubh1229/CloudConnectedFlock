using System;
using System.Collections.Generic;

namespace DatabaseService.Models;

public partial class Securityqa
{
    public Guid Id { get; set; }

    public byte[] Securitykey { get; set; } = null!;

    public List<byte[]> Securityhasharray { get; set; } = null!;

    public virtual ICollection<Useraccount> Useraccounts { get; set; } = new List<Useraccount>();
}

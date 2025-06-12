using System;
using System.Collections.Generic;

namespace DatabaseService.Models;

public partial class Useraccount
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public DateOnly Birthday { get; set; }

    public string Email { get; set; } = null!;

    public byte[] Passwordkey { get; set; } = null!;

    public byte[] Passwordhash { get; set; } = null!;

    public Guid Securityqaid { get; set; }

    public virtual Securityqa Securityqa { get; set; } = null!;

    public virtual Userprofile? Userprofile { get; set; }
}

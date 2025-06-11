using System;
using System.Collections.Generic;

namespace DatabaseService.Models;

public partial class Userprofile
{
    public int Id { get; set; }

    public Guid Accountid { get; set; }

    public Guid Financeid { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Bio { get; set; } = null!;

    public List<string> Personallinks { get; set; } = null!;

    public string? Resumefilepath { get; set; }

    public string Profilepicturepath { get; set; } = null!;

    public virtual Useraccount Account { get; set; } = null!;

    public virtual Userfinance Finance { get; set; } = null!;
}

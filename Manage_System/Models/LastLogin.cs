using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class LastLogin
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime? History { get; set; }

    public virtual User? User { get; set; }
}

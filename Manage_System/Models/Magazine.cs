﻿using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class Magazine
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public DateTime? ClosureDay { get; set; }

    public DateTime? FinalClosureDay { get; set; }

    public virtual ICollection<Contribution> Contributions { get; } = new List<Contribution>();
}

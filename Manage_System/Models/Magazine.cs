using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.models;

public partial class Magazine
{
    public int Id { get; set; }

    public string? Description { get; set; }
    public DateTime? StartYear { get; set; }
    public DateTime? CloseYear { get; set; }

    public virtual ICollection<Contribution> Contributions { get; } = new List<Contribution>();
}

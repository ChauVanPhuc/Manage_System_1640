using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.models;

public partial class Magazine
{
    public int Id { get; set; }

    public string? Description { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? ClosureDay { get; set; }

    public DateTime? FinalClosureDay { get; set; }

    public virtual ICollection<Contribution> Contributions { get; } = new List<Contribution>();
}

using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class ImgFile
{
    public int Id { get; set; }

    public string? Stype { get; set; }

    public string? Url { get; set; }

    public int? ContributionId { get; set; }

    public virtual Contribution? Contribution { get; set; }
}

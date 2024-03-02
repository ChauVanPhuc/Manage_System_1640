using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class Comment
{
    public int Id { get; set; }

    public int? ContributionId { get; set; }

    public int? UserId { get; set; }

    public string? CommentText { get; set; }

    public DateTime? CommentDate { get; set; }

    public virtual Contribution? Contribution { get; set; }

    public virtual User? User { get; set; }
}

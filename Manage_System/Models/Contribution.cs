using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class Contribution
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? Status { get; set; }

    public bool? Publics { get; set; }

    public int? MagazineId { get; set; }

    public string? ShortDescription { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ImgFile> ImgFiles { get; set; } = new List<ImgFile>();

    public virtual Magazine? Magazine { get; set; }

    public virtual User? User { get; set; }
}

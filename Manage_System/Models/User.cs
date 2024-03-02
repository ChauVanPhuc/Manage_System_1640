using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class User
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public int? FacultyId { get; set; }

    public DateTime? CreateDay { get; set; }

    public bool? Status { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Contribution> Contributions { get; } = new List<Contribution>();

    public virtual Faculty? Faculty { get; set; }

    public virtual Role? Role { get; set; }
}

using Manage_System.models;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Coordinator.ModelView
{
    public class CommentModelView
    {
        public int Id { get; set; }

        public int? ContributionId { get; set; }

        public int? UserId { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Comment")]
        [Display(Name = "Comment")]
        public string? CommentText { get; set; }

        public DateTime? CommentDate { get; set; }

        public virtual Contribution? Contribution { get; set; }

        public virtual User? User { get; set; }
    }
}

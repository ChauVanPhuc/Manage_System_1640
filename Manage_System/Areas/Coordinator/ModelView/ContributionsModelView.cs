using Manage_System.models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Coordinator.ModelView
{
    public class ContributionsModelView
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Title")]
        [Display(Name = " Title")]
        public string? Title { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter ShortDescription")]
        [Display(Name = " Short Description")]
        public string? ShortDescription { get; set; }

        [MaxLength(1500)]
        [Required(ErrorMessage = "Please, Enter Content")]
        [Display(Name = "Content")]
        public string? Content { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool? Status { get; set; }

        public string? Publics { get; set; }

        public int? MagazineId { get; set; }

        public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

        public virtual ICollection<ImgFile> ImgFiles { get; } = new List<ImgFile>();


        [NotMapped]
        public IFormFileCollection? ImgFile { get; set; }

        public virtual Magazine? Magazine { get; set; }

        public virtual User? User { get; set; }
    }
}

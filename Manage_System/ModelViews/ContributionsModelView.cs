using Manage_System.models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manage_System.ModelViews
{
    public class ContributionsModelView
    {
        public int? Id { get; set; }

        public int? UserId { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Title")]
        [Display(Name = " Title")]
        public string? Title { get; set; }

        [MaxLength(500)]
        [Required(ErrorMessage = "Please, Enter ShortDescription")]
        [Display(Name = " Short Description")]
        public string? ShortDescription { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string? Status { get; set; }

        public bool? Publics { get; set; }

        public int? MagazineId { get; set; }

        public virtual List<Comment>? Comments { get; set; }
        public string? Comment { get; set; }
        public virtual IEnumerable<ImgFile>? ImgFiles { get; set; }


        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Please, Choose File")]
        [NotMapped]
        public IFormFileCollection? ImgFile { get; set; }
        public string? Mess { get; set; }
        public virtual Magazine? Magazine { get; set; }
        public virtual User? User { get; set; }
        public virtual User? Coordinator { get; set; }

        public int? coordinatorId { get; set; }
        public virtual List<ChatViewModel>? chatViewModels { get; set; }
    }
}

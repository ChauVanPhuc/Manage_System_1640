﻿using Manage_System.models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

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

        public DateTime? SubmissionDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string? Status { get; set; }

        public bool? Publics { get; set; }

        public int? MagazineId { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public string? Comment { get; set; }
        public string? Mess { get; set; }
        public virtual IEnumerable<ImgFile> ImgFiles { get; set; } 
            

        [NotMapped]
        public IFormFileCollection? ImgFile { get; set; }

        public virtual Magazine? Magazine { get; set; }
        public virtual User? User { get; set; }
        public virtual User? Coordinator { get; set; }
    }
}

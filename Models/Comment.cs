using System.ComponentModel.DataAnnotations;

namespace Blog_MVC.Models
{
    public class Comment
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key (PK for BlogPost)
        public int BlogPostId { get; set; }
        
        // Foreign Key
        [Required]
        public string? AuthorId { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastUpdated { get; set; }

        public string? UpdateReason { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and max of {1} characters long.", MinimumLength = 2)]
        public string? Body { get; set; }
        
        // Navigation Properties
        public virtual BlogPost? BlogPost { get; set; }

        public virtual BlogUser? Author { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_MVC.Models
{
    public class BlogPost
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and max of {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastUpdated { get; set; }

        // Foreign Key (PK for Category)
        public int CategoryId { get; set; }
        public string? Slug { get; set; }
        public string? Abstract { get; set; }
        public bool IsDeleted { get; set; }

        [DisplayName("Published")]
        public bool IsPublished { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        // moves entire image file to Post & passes info to service
        [NotMapped]
        public IFormFile? BlogPostImage { get; set; }
        
        // Navigation Properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}

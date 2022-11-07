using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_MVC.Models
{
    public class Category
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(33, ErrorMessage = "The {0} must be at least {2} and max of {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and max of {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        // moves entire image file to Post & passes info to service
        [NotMapped]
        public IFormFile? CategoryImage { get; set; }

        // Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}

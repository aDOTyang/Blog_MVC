using System.ComponentModel.DataAnnotations;

namespace Blog_MVC.Models
{
    public class Tag
    {
        // Priuary Key
        public int Id { get; set; }

        [Required]
        [StringLength(33, ErrorMessage = "The {0} must be at least {2} and max of {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        // Navigation Properties
        // Hashset is smallest way to get info into database -> more efficient than List in this case
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}

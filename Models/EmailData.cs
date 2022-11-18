using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Blog_MVC.Models
{
    public class EmailData
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        public string? Message { get; set; }
    }
}

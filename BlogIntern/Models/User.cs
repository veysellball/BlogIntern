using System.ComponentModel.DataAnnotations;

namespace BlogIntern.Models
{

    public class User
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime InsertDate { get; set; } = DateTime.Now;


    }
}

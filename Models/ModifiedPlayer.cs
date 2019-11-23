using System.ComponentModel.DataAnnotations;

namespace UndoneAspNetCoreApi.Models
{
    public class ModifiedPlayer : IPlayer
    {
        public bool SetBanned { get; set; }
        public bool SetAdmin { get; set; }
        public bool IsAdmin { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,20}$",
         ErrorMessage = "Characters are not allowed.")] //only the alphabet is allowed (1-20 chars)
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9''-'\s]{1,20}$",
         ErrorMessage = "Characters are not allowed.")]
        public string Password { get; set; }
    }
}

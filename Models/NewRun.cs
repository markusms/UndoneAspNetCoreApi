using System;
using System.ComponentModel.DataAnnotations;
using UndoneAspNetCoreApi.Validation;

namespace UndoneAspNetCoreApi.Models
{
    public class NewRun : IRun
    {
        [Required]
        [Range(0, 99999)]
        public float TimeTaken { get; set; } //speedrun time

        [Required]
        [DateIsNotInFuture]
        public DateTime TimePosted { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9''-'\s]{1,20}$",
         ErrorMessage = "Characters are not allowed.")]
        public string Level { get; set; }
    }
}

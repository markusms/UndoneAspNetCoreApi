using System;
using System.ComponentModel.DataAnnotations;
using UndoneAspNetCoreApi.Models;

namespace UndoneAspNetCoreApi.Validation
{
    public class DateIsNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var newRun = (NewRun)validationContext.ObjectInstance;
            if (DateTime.Now.Subtract(newRun.TimePosted).TotalSeconds < 0)
                return new ValidationResult("Date is in the future!");
            return ValidationResult.Success;
        }
    }
}

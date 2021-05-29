using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MovieTheater.Validations
{
    public class FileWeightValidation : ValidationAttribute
    {
        private readonly int maxWeightInMb;

        public FileWeightValidation(int maxWeightInMb)
        {
            this.maxWeightInMb = maxWeightInMb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var formFile = value as IFormFile;
            if (formFile == null) return ValidationResult.Success;

            if (formFile.Length > maxWeightInMb * 1024 * 1024)
            {
                return new ValidationResult($"The weight of the file can't be higher than {maxWeightInMb}mb.");
            }

            return ValidationResult.Success;
        }
    }
}

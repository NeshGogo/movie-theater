using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] fileTypeValids;

        public FileTypeValidation(string[] fileTypeValids)
        {
            this.fileTypeValids = fileTypeValids;
        }

        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            if (fileTypeGroup == FileTypeGroup.image)
            {
                fileTypeValids = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var formFile = value as IFormFile;
            if (formFile == null) return ValidationResult.Success;

            if (!fileTypeValids.Contains(formFile.ContentType))
            {
                return new ValidationResult($"The file type is not valid, need to be one of this {string.Join(", ", fileTypeValids)}");
            }

            return ValidationResult.Success;
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.api.Attribute
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly string _errorMessage;
        public AllowedExtensionsAttribute(string[] extensions,string errorMessage="FILE_NOT_SUPPORT")
        {
            _extensions = extensions;
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var files = value as IFormFile[];
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (file != null)
                    {
                        if (!_extensions.Contains(extension.ToLower()))
                        {
                            return new ValidationResult(_errorMessage);
                        }
                    }
                }
               
            }
            return ValidationResult.Success;
        }

      
    }
}

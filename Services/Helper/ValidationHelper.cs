using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace Services.Helper
{
    public class ValidationHelper
    {
        public static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResult = new List<ValidationResult>();

            bool isValid =  Validator.TryValidateObject(obj, validationContext, validationResult);
            if (!isValid)
            {
                throw new ArgumentException(validationResult.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}

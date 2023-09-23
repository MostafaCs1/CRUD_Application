using System.ComponentModel.DataAnnotations;

namespace Services.Helper;

public class ValidationHelper
{
    public static void ModelValidation(object obj)
    {
        var validationContext = new ValidationContext(obj);
        var validationResult = new List<ValidationResult>();

        bool isValid =  Validator.TryValidateObject(obj, validationContext, validationResult, true);
        if (!isValid)
        {
            throw new ArgumentException(validationResult.FirstOrDefault()?.ErrorMessage);
        }
    }
}

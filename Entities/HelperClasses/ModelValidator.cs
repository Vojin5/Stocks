using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.HelperClasses
{
    public static class ModelValidator
    {
        public static bool IsValid(object? obj)
        {
            if (obj == null)
                return false;

            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext,results,true);
            return isValid;
        }
    }
}

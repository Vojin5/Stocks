using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CustomValidators
{
    internal class MinDateAttribute : ValidationAttribute
    {
        private readonly DateTime _minDate;

        public MinDateAttribute(string minDate)
        {
            _minDate = DateTime.Parse(minDate);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("Provided Date is null");
            }
            DateTime dateTime = (DateTime)value;
            if(dateTime >= _minDate)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(string.Format(ErrorMessage ?? "Date of {0} should be older than {1}", validationContext.MemberName, _minDate.Date.ToString()));
            }
            
        }
    }
}

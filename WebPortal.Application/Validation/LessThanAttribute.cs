using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebPortal.Application.Validation
{
    [AttributeUsage(
        AttributeTargets.Property,
        AllowMultiple = false)]
    public class LessThanAttribute : ValidationAttribute
    {
        public string OtherProperty { get; private set; }


        public LessThanAttribute(
            string otherProperty)
        {
            OtherProperty = otherProperty;
        }


        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {

            if (value == null)
            {
                return ValidationResult.Success;
            }


            PropertyInfo otherPropertyInfo = validationContext
                                                .ObjectType
                                                .GetProperty(OtherProperty);




            object otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            IComparable currentValue = value as IComparable;


            if (currentValue.CompareTo(
                    otherValue) < 0)
            {
                return ValidationResult.Success;
            }


            return new ValidationResult(
                FormatErrorMessage(
                    validationContext.DisplayName),
                new[]
                {
                    validationContext.MemberName
                });
        }
    }
}
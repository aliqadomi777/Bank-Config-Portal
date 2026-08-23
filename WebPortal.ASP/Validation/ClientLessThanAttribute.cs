using System.Collections.Generic;
using System.Web.Mvc;
using WebPortal.Application.Validation;

namespace WebPortal.ASP.Validation
{
    public class ClientLessThanAttribute : LessThanAttribute,
                 IClientValidatable
    {
        public ClientLessThanAttribute(string otherProperty) : base(otherProperty)
        {
        }


        public IEnumerable<ModelClientValidationRule>
            GetClientValidationRules(
                ModelMetadata metadata,
                ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "lessthan",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };


            rule.ValidationParameters.Add("other", OtherProperty);
            yield return rule;
        }
    }
}
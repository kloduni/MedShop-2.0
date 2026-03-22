using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace MedShop.ModelBinders
{
    /// <summary>
    /// Custom model binder for <see cref="decimal"/> that normalises both '.' and ',' to the
    /// current culture's decimal separator before parsing.  This is necessary because form
    /// inputs submitted from browsers in different locales may use either character as the
    /// decimal point, and the default binder rejects whichever is not the server's separator.
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);

            if (valueResult != ValueProviderResult.None && !string.IsNullOrEmpty(valueResult.FirstValue))
            {
                decimal actualValue = 0M;

                bool success = false;

                try
                {
                    string decimalValue = valueResult.FirstValue;
                    decimalValue = decimalValue.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    decimalValue = decimalValue.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    actualValue = Convert.ToDecimal(decimalValue, CultureInfo.CurrentCulture);
                    success = true;
                }
                catch (FormatException fe)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, fe, bindingContext.ModelMetadata);
                }

                if (success)
                {
                    bindingContext.Result = ModelBindingResult.Success(actualValue);
                }
            }

            return Task.CompletedTask;
        }
    }
}

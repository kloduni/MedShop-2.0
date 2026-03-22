using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MedShop.ModelBinders
{
    /// <summary>
    /// Registers <see cref="DecimalModelBinder"/> for all <see cref="decimal"/> and nullable
    /// <see cref="decimal"/> model properties.  Inserted at position 0 in the binder provider
    /// list so it takes precedence over the built-in binders.
    /// </summary>
    public class DecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Decimal) || context.Metadata.ModelType == typeof(Decimal?))
            {
                return new DecimalModelBinder();
            }

            return null;
        }
    }
}

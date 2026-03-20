namespace MedShop.Core.Exceptions
{
    public class MedShopException : ApplicationException
    {
        public MedShopException()
        {
        }

        public MedShopException(string errorMessage)
        : base(errorMessage)
        {
        }
    }
}

namespace MedShop.Core.Models.Admin
{
    public class UserServiceModel
    { 
        public string UserId { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public bool IsActive { get; init; }
    }
}

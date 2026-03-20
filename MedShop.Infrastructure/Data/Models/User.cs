using Microsoft.AspNetCore.Identity;

namespace MedShop.Infrastructure.Data.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = true;
        public ICollection<UserProduct> UsersProducts { get; set; } = new List<UserProduct>();
    }
}

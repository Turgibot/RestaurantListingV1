using Microsoft.AspNetCore.Identity;

namespace RestaurantListing.Data
{
    public class ApiUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsPaid { get; set; }

    }
}

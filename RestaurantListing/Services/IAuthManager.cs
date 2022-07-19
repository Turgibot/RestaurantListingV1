using RestaurantListing.DTOs;
using System.Threading.Tasks;

namespace RestaurantListing.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDTO userDTO);
        string CreateToken();
    }
}

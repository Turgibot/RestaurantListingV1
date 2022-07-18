using RestaurantListing.DTOs;
using System.ComponentModel.DataAnnotations;

namespace RestaurantListing.Core.DTOs
{
    public class CreateLocationDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country Name Is Too Long")]
        public string Country { get; set; }
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "CIty Name Is Too Long")]
        public string City { get; set; }
        [Required]
        public int RestaurantId { get; set; }

    }

    public class LocationDTO : CreateLocationDTO
    {
        public int Id { get; set; }
        public RestaurantDTO Restaurant { get; set; }
    }

    public class UpdateLocationDTO : CreateLocationDTO { }
}



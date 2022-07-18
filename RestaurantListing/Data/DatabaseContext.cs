using Microsoft.EntityFrameworkCore;

namespace RestaurantListing.Data
{
    public class DatabaseContext :DbContext
    {
        public DatabaseContext(DbContextOptions options): base(options){ }
        
        // list all tables = DbSets
        public DbSet<Restaurant> Restaurants{ get; set; }
        public DbSet<Location> Locations{ get; set; }
        public DbSet<Dish> Dishes{ get; set; }


    
    }
}

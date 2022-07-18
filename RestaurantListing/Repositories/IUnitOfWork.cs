using RestaurantListing.Data;
using System;
using System.Threading.Tasks;

namespace RestaurantListing.Repositories
{
    public interface IUnitOfWork:IDisposable
    {

        IGenericRepository<Restaurant> Restaurants { get; }
        IGenericRepository<Location> Locations { get; }
        IGenericRepository<Dish> Dishes { get; }

        Task Save();
    }
}

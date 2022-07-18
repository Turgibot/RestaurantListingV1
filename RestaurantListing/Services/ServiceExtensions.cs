using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RestaurantListing.Data;

namespace RestaurantListing.Services
{
    public static class ServiceExtensions
    {

        public static void AddIdentityConfiguration(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(
                op => {
                    op.Password.RequireLowercase = false;
                    op.User.RequireUniqueEmail = true;
                }
                );
            builder = new IdentityBuilder(builder.UserType,
                typeof(IdentityRole),
                services).AddRoles<IdentityRole>();

            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

        }
    }
}

using System;
using System.Collections.Generic;

namespace RestaurantListing.Data
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime VisitedOn { get; set; }

        //relations
        // a restaurant has one location
        public virtual Location Location { get; set; }

        // a restaurant has many dishes
        public virtual List<Dish> Dishes { get; set; }
    }
}

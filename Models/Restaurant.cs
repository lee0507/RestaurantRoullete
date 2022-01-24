namespace Swilago.Models
{
    public class Restaurants
    {
        public Restaurants()
        {
            RestaurantList = new List<Restaurant>();
        }

        public List<Restaurant> RestaurantList { get; set; }
    }

    public class Restaurant
    {
        public int id { get; set; }

        public string text { get; set; }
    }
}

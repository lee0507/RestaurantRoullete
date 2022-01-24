using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swilago.Data.Tables
{
    [Table("T_Restaurant")]
    public class TRestaurant
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; }
    }
}

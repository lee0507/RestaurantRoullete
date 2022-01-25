using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swilago.Data.Tables
{
    [Table("T_Statistics")]
    public class TStatistics
    {
        [Key]
        public int StatisticsId { get; set; }

        public string Email { get; set; }

        public string RouletteResult { get; set; }

        public string RouletteList { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

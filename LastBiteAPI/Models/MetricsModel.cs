using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastBiteAPI.Models
{
    [Table("Metrics")]
    public class MetricsModel
    {
        [Key]
        [Column("food_name")]
        public string FoodName { get; set; }

        [Column("frequency")]
        public int Frequency { get; set; }

        [Column("share_of_all_wasted")]
        public float ShareOfAllWasted { get; set; }

        [Column("share_of_this_wasted")]
        public float ShareOfThisWasted { get; set; }

        [Column("provided_count")]
        public int ProvidedCount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastBiteAPI.Models
{
    [Table("Scans")]
    public class ScansModel
    {
        [Key]
        [Column("uuid")]
        public Guid UUID { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("food_name")]
        public string FoodName { get; set; }
    }
}

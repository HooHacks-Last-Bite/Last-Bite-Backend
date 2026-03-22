namespace LastBiteAPI.Models
{
    public class DetectionItem
    {
        public List<FoodItem> Detections { get; set; }
    }

    public class FoodItem
    {
        public string FoodName { get; set; }
    }
}

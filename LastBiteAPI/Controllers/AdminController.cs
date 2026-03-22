using LastBiteAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastBiteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LastBiteDbContext db_context;

        public AdminController(LastBiteDbContext context)
        {
            db_context = context;
        }

        [HttpPost("seedDemoData")]
        public async Task<IActionResult> SeedDemoData()
        {
            try
            {
                var random = new Random();

                var breakfastFoods = new List<string>
                {
                    "coffee", "waffle", "banana", "eggs", "apple", "blueberry", "strawberry", "raspberry"
                };

                var lunchFoods = new List<string>
                {
                    "burger", "fries", "salad", "sandwich", "rice", "chicken"
                };

                var dinnerFoods = new List<string>
                {
                    "pizza", "pasta", "rice", "chicken", "salad"
                };

                var allScans = new List<ScansModel>();

                for (int dayOffset = 0; dayOffset < 14; dayOffset++)
                {
                    var day = DateTime.UtcNow.Date.AddDays(-dayOffset);

                    int breakfastCount = random.Next(25, 60);
                    int lunchCount = random.Next(50, 120);
                    int dinnerCount = random.Next(50, 120);

                    for (int i = 0; i < breakfastCount; i++)
                    {
                        var food = breakfastFoods[random.Next(breakfastFoods.Count)];
                        allScans.Add(new ScansModel
                        {
                            UUID = Guid.NewGuid(),
                            FoodName = food,
                            CreatedAt = day
                                .AddHours(random.Next(7, 10))
                                .AddMinutes(random.Next(0, 60))
                        });
                    }

                    for (int i = 0; i < lunchCount; i++)
                    {
                        var food = lunchFoods[random.Next(lunchFoods.Count)];
                        allScans.Add(new ScansModel
                        {
                            UUID = Guid.NewGuid(),
                            FoodName = food,
                            CreatedAt = day
                                .AddHours(random.Next(11, 15))
                                .AddMinutes(random.Next(0, 60))
                        });
                    }

                    for (int i = 0; i < dinnerCount; i++)
                    {
                        var food = dinnerFoods[random.Next(dinnerFoods.Count)];
                        allScans.Add(new ScansModel
                        {
                            UUID = Guid.NewGuid(),
                            FoodName = food,
                            CreatedAt = day
                                .AddHours(random.Next(17, 20))
                                .AddMinutes(random.Next(0, 60))
                        });
                    }
                }

                db_context.Scans.AddRange(allScans);

                var existingMetrics = await db_context.Metrics.ToListAsync();

                var frequencyMap = allScans
                    .GroupBy(s => s.FoodName)
                    .ToDictionary(g => g.Key, g => g.Count());

                foreach (var metric in existingMetrics)
                {
                    if (frequencyMap.TryGetValue(metric.FoodName, out int count))
                    {
                        metric.Frequency += count;
                        metric.ShareOfThisWasted = metric.ProvidedCount == 0
                            ? 0
                            : (float)metric.Frequency / metric.ProvidedCount;
                    }
                }

                int newTotalFrequency = existingMetrics.Sum(m => m.Frequency);

                foreach (var metric in existingMetrics)
                {
                    metric.ShareOfAllWasted = newTotalFrequency == 0
                        ? 0
                        : (float)metric.Frequency / newTotalFrequency;
                }

                await db_context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Demo data seeded successfully.",
                    scansInserted = allScans.Count
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return StatusCode(500, new
                {
                    message = e.Message,
                    inner = e.InnerException?.Message
                });
            }
        }
    }
}
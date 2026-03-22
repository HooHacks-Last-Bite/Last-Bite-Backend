using LastBiteAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LastBiteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly LastBiteDbContext db_context;

        public MetricsController(LastBiteDbContext context)
        {
            db_context = context;
        }

        [HttpGet("getMetrics/getAll")]
        public async Task<List<MetricsModel>> GetAllMetrics()
        {
            try
            {
                return await db_context.Metrics.ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return new List<MetricsModel>();
            }
        }

        [NonAction]
        public async Task<bool> UpdateFoodInternal(string foodName)
        {
            var metric = await db_context.Metrics
                .FirstOrDefaultAsync(food => food.FoodName == foodName);

            if (metric == null)
            {
                return false;
            }

            metric.Frequency += 1;
            metric.ShareOfThisWasted = (float) metric.Frequency / metric.ProvidedCount;

            return true;
        }

        [NonAction]
        public async Task UpdateAllWastedShares()
        {
            try
            {
                int frequencySum = 0;
                foreach (MetricsModel foodMetric in db_context.Metrics)
                {
                    frequencySum += foodMetric.Frequency;
                }

                foreach (MetricsModel foodMetric in db_context.Metrics)
                {
                    foodMetric.ShareOfAllWasted = (float)foodMetric.Frequency / frequencySum;
                }

                await db_context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        [HttpPut("updateFood/{foodName}")]
        public async Task<IActionResult> UpdateFood(string foodName)
        {
            try
            {
                bool updated = await UpdateFoodInternal(foodName);

                if (!updated)
                {
                    return NotFound(new { message = "Food not found" });
                }

                await db_context.SaveChangesAsync();
                return Ok();
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

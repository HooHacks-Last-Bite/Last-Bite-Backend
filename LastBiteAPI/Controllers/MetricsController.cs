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

        [HttpPut("updateFood/{foodName}")]
        public async Task<IActionResult> updateFood(string foodName)
        {
            try
            {
                var metric = await db_context.Metrics
                    .FirstOrDefaultAsync(food => food.FoodName == foodName);

                if (metric == null)
                {
                    return NotFound(new { message = "Food not found" });
                }

                metric.Frequency += 1;
                metric.ShareOfThisWasted = metric.Frequency / metric.ProvidedCount;

                int frequencySum = 0;
                foreach (MetricsModel met in db_context.Metrics)
                {
                    frequencySum += met.Frequency;
                }
                metric.ShareOfAllWasted = (float) metric.Frequency / frequencySum;

                await db_context.SaveChangesAsync();

                return Ok(metric);
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

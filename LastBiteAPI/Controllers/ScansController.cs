using LastBiteAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LastBiteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScansController : ControllerBase
    {
        private readonly LastBiteDbContext db_context;

        public ScansController(LastBiteDbContext context)
        {
            db_context = context;
        }

        [HttpPost("addScan")]
        public async Task<IActionResult> AddScan([FromBody] DetectionItem request)
        {
            try
            {
                List<string> foodList = request.Detections
                    .Where(x => !string.IsNullOrWhiteSpace(x.FoodName))
                    .Select(x => x.FoodName.Trim())
                    .ToList();

                var scans = foodList.Select(food => new ScansModel
                {
                    UUID = Guid.NewGuid(),
                    FoodName = food,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                MetricsController metricsController = new MetricsController(db_context);
                foreach (ScansModel scan in scans)
                {
                    await metricsController.UpdateFoodInternal(scan.FoodName);
                }

                db_context.Scans.AddRange(scans);
                await db_context.SaveChangesAsync();

                await metricsController.UpdateAllWastedShares();

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

        [HttpGet("getScans/getAll")]
        public async Task<List<ScansModel>> GetAllScans()
        {
            try
            {
                return await db_context.Scans.ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return new List<ScansModel>();
            }
        }

        [HttpGet("getScans/getAfter/{afterTime}")]
        public async Task<List<ScansModel>> GetScansAfter(DateTime afterTime)
        {
            try
            {
                return await db_context.Scans
                    .Where(scan =>
                        scan.CreatedAt > afterTime)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return new List<ScansModel>();
            }
        }

        [HttpGet("getScans/getBefore/{beforeTime}")]
        public async Task<List<ScansModel>> GetScansBefore(DateTime beforeTime)
        {
            try
            {
                return await db_context.Scans
                    .Where(scan =>
                        scan.CreatedAt < beforeTime)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return new List<ScansModel>();
            }
        }

        [HttpGet("getScans/getBetween/{leftbound}/{rightbound}")]
        public async Task<List<ScansModel>> GetBetween(DateTime leftbound, DateTime rightbound)
        {
            try
            {
                return await db_context.Scans
                    .Where(scan =>
                        scan.CreatedAt > leftbound &&
                        scan.CreatedAt < rightbound)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return new List<ScansModel>();
            }
        }
    }
}

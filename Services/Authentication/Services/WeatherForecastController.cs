using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeAdminAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public class WeatherForecast
        {
            public DateOnly Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

            public string? Summary { get; set; }
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        //[HttpGet("CheckConfig")]
        //public IActionResult CheckConfig(string Config)
        //{
        //    try
        //    {
        //        var configValues = new
        //        {
        //            Config = _config[Config],
        //        };

        //        return Ok(configValues);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error reading config");
        //        return StatusCode(500, "Error reading configuration");
        //    }
        //}

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok(new { message = "Authorized successfully" });
        }



        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "No authentication needed" });
        }
    }
}

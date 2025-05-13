

using HubService.DTO;
using HubService.Services;
using Microsoft.AspNetCore.Mvc;
using WeatherService;

namespace  HubService.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase{

        private readonly ILogger<WeatherController> logger;
        private readonly GrpcWeatherClient grpcWeatherClient;

        public WeatherController(ILogger<WeatherController> logger, GrpcWeatherClient grpcWeatherClient) {
            this.logger = logger;
            this.grpcWeatherClient = grpcWeatherClient;
        }

        [HttpPost("weather")]
        public async Task<IActionResult> GetWeatherResponse([FromBody] WeatherRequestDTO req) {
            var request = new WeatherRequest {
                Longitude = req.Longitude,
                Latitude = req.Latitude
            };
            var response = await grpcWeatherClient.WeatherResponse(request);

            var weather = new WeatherDTO{
                Temp = response.TemperatureC,
                Condition = response.ConditionSummary,
                Code = response.WeatherCode
            }; 


            String fileLocation = "";
            
            switch(weather.Code){
                case 1: fileLocation = "location to be added latter for all cases"; break;
            }

            return Ok( new {
                weather,
                fileLocation
            });
            
        }



    }
}
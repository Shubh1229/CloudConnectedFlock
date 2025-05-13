

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
                Code = response.WeatherCode,
                City = response.City,
                State = response.State,
                Precip = response.Precipitation
            }; 


            String fileLocation = "";
            
            switch (weather.Code)
            {
                case 0:
                    fileLocation = "media/clear_sky.jpg";
                    break;
                case 1:
                case 2:
                case 3:
                    fileLocation = "media/cloudy.jpg";
                    break;
                case 45:
                case 48:
                    fileLocation = "media/fog.jpg";
                    break;
                case 51:
                case 53:
                case 55:
                    fileLocation = "media/drizzle.jpg";
                    break;
                case 56:
                case 57:
                    fileLocation = "media/freezing_drizzle.jpg";
                    break;
                case 61:
                case 63:
                case 65:
                    fileLocation = "media/rain.jpg";
                    break;
                case 66:
                case 67:
                    fileLocation = "media/freezing_rain.jpg";
                    break;
                case 71:
                case 73:
                case 75:
                    fileLocation = "media/snow.jpg";
                    break;
                case 77:
                    fileLocation = "media/snow_grains.jpg";
                    break;
                case 80:
                case 81:
                case 82:
                    fileLocation = "media/rain_showers.jpg";
                    break;
                case 85:
                case 86:
                    fileLocation = "media/snow_showers.jpg";
                    break;
                case 95:
                    fileLocation = "media/thunderstorm.jpg";
                    break;
                case 96:
                case 99:
                    fileLocation = "media/thunderstorm_hail.jpg";
                    break;
                default:
                    fileLocation = "media/unknown.jpg";
                    break;
            }


            return Ok( new {
                weather,
                fileLocation
            });
            
        }



    }
}
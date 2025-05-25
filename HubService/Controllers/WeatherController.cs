

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
            logger.LogInformation($"Request to get weather sent to weather service:\nLatitude: {req.Latitude} and Longitude: {req.Longitude}\n{req}");
            var request = new WeatherRequest {
                Longitude = req.Longitude,
                Latitude = req.Latitude
            };
            logger.LogInformation("Weather request created and sent...");
            var response = await grpcWeatherClient.WeatherResponse(request);

            var weather = new WeatherDTO{
                Temp = response.TemperatureC,
                Condition = response.ConditionSummary,
                Code = response.WeatherCode,
                City = response.City,
                State = response.State,
                Precip = response.Precipitation
            };

            logger.LogInformation($"Response received:\nResponse:{weather}\n{weather.Temp}, {weather.Condition}, {weather.Code}, {weather.City}, {weather.State}, {weather.Precip}");


            String fileLocation = "";
            
            switch (weather.Code)
            {
                case 0:
                    fileLocation = "weather/slightlycloudyskys.mp4"; // Used for clear/sunny
                    break;
                case 1:
                case 2:
                case 3:
                    fileLocation = "weather/overcast.mp4";
                    break;
                case 45:
                case 48:
                    fileLocation = "weather/overcast.mp4"; // foggy.mov is corrupted
                    break;
                case 51:
                case 53:
                case 55:
                    fileLocation = "weather/rainingclouds.mp4";
                    break;
                case 56:
                case 57:
                    fileLocation = "weather/rainingclouds.mp4"; // fallback, freezing drizzle = rain
                    break;
                case 61:
                case 63:
                case 65:
                    fileLocation = "weather/rainingclouds.mp4";
                    break;
                case 66:
                case 67:
                    fileLocation = "weather/rainingclouds.mp4"; // same fallback
                    break;
                case 71:
                case 73:
                case 75:
                    fileLocation = "weather/snow.mp4";
                    break;
                case 77:
                    fileLocation = "weather/snow.mp4"; // snow grains fallback
                    break;
                case 80:
                case 81:
                case 82:
                    fileLocation = "weather/rainingclouds.mp4"; // rain showers
                    break;
                case 85:
                case 86:
                    fileLocation = "weather/snow.mp4"; // snow showers
                    break;
                case 95:
                    fileLocation = "weather/rainingclouds.mp4"; // fallback until thunderstorm video
                    break;
                case 96:
                case 99:
                    fileLocation = "weather/rainingclouds.mp4"; // fallback until thunderstorm w/ hail video
                    break;
                default:
                    fileLocation = "weather/slightlycloudyskys.mp4"; // default sunny video
                    break;
            }

            logger.LogInformation($"sending {weather}, {fileLocation}");

            return Ok( new {
                weather,
                fileLocation
            });
            
        }



    }
}
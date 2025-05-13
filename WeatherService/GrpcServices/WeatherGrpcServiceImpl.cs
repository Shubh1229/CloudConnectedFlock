
using System.Net.Http;
using System.Text.Json;
using Grpc.Core;
using WeatherService;

namespace WeatherService.GrpcServices
{
    public class WeatherGrpcServiceImpl : WeatherService.WeatherServiceBase
    {
        private readonly ILogger<WeatherGrpcServiceImpl> logger;
        private readonly HttpClient httpClient;

        public WeatherGrpcServiceImpl(ILogger<WeatherGrpcServiceImpl> logger)
        {
            this.logger = logger;
            this.httpClient = new HttpClient(); 
        }

        public override async Task<WeatherResponse> GetForecast(WeatherRequest request, ServerCallContext context)
        {
            double latitude = request.Latitude;
            double longitude = request.Longitude;

            string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(apiUrl);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to fetch weather API: {0}", ex.Message);
                throw new RpcException(new Status(StatusCode.Unavailable, "Weather API unreachable"));
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Weather API returned error status: {0}", response.StatusCode);
                throw new RpcException(new Status(StatusCode.Internal, "Weather API error"));
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(jsonString);

            var weatherData = json.RootElement.GetProperty("current_weather");

            double temperatureC = weatherData.GetProperty("temperature").GetDouble();
            long weatherCode = weatherData.GetProperty("weathercode").GetInt64();

            
            string summary = WeatherCodeToSummary(weatherCode);

            return new WeatherResponse
            {
                TemperatureC = temperatureC,
                WeatherCode = weatherCode,
                ConditionSummary = summary
            };
        }

       
        private string WeatherCodeToSummary(long code)
        {
            return code switch
            {
                0 => "Clear sky",
                1 => "Mainly clear",
                2 => "Partly cloudy",
                3 => "Overcast",
                45 or 48 => "Fog",
                51 or 53 or 55 => "Drizzle",
                61 or 63 or 65 => "Rain",
                71 or 73 or 75 => "Snow",
                80 or 81 or 82 => "Rain showers",
                _ => "Unknown"
            };
        }
    }
}

using System.Text.Json;


namespace HubService.Services
{
    public class WeatherAPIClient
    {
        private readonly ILogger<WeatherAPIClient> logger;
        private readonly HttpClient httpClient;

        public WeatherAPIClient(ILogger<WeatherAPIClient> logger)
        {
            this.logger = logger;
            this.httpClient = new HttpClient();
        }
        public async Task<WeatherResponse> GetForecast(WeatherRequest request)
        {
            double latitude = request.Latitude;
            double longitude = request.Longitude;

            logger.LogInformation($"Request : Longitude {longitude}, Latitude {latitude}");

            string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true&hourly=precipitation";

            logger.LogInformation("sending response");
            HttpResponseMessage response;

            response = await httpClient.GetAsync(apiUrl);
            logger.LogInformation($"Got response {response}");

            var jsonString = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(jsonString);

            logger.LogInformation($"JSON: {json}");

            var weatherData = json.RootElement.GetProperty("current_weather");
            var hourly = json.RootElement.GetProperty("hourly");

            logger.LogInformation($"Weather Data: {weatherData}");
            logger.LogInformation($"Hourly Data: {hourly}");

            double temperatureC = weatherData.GetProperty("temperature").GetDouble();
            long weatherCode = weatherData.GetProperty("weathercode").GetInt64();
            string currentTime = weatherData.GetProperty("time").GetString() ?? "";

            var times = hourly.GetProperty("time").EnumerateArray().ToList();
            var precipitations = hourly.GetProperty("precipitation").EnumerateArray().ToList();

            string currentUtcHour = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
            logger.LogInformation(currentUtcHour + " is the current UTC Time...");
            int index = times.FindIndex(t => t.GetString() == currentUtcHour);
            logger.LogInformation($"Index got is {index}");
            double precipitation = (index >= 0) ? precipitations[index].GetDouble() : 0.0;
            logger.LogInformation($"Total precipitation for this hour is {precipitation}");



            string summary = WeatherCodeToSummary(weatherCode);

            logger.LogInformation("Getting Address");
            string reverseGeoUrl = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";

            var geoRequest = new HttpRequestMessage(HttpMethod.Get, reverseGeoUrl);

            geoRequest.Headers.UserAgent.ParseAdd("ccflock.duckdns.org WeatherService/1.0");

            string city = "Breinigsville";
            string state = "PA";

            try
            {
                var geoResponse = await httpClient.SendAsync(geoRequest);
                if (geoResponse.IsSuccessStatusCode)
                {
                    var geoJson = await geoResponse.Content.ReadAsStringAsync();
                    using var geoDoc = JsonDocument.Parse(geoJson);
                    var address = geoDoc.RootElement.GetProperty("address");

                    logger.LogInformation($"Got Response for Address: {geoDoc}");

                    logger.LogInformation(address.ToString());

                    if (address.TryGetProperty("city", out var cityElem))
                        city = cityElem.GetString();
                    else if (address.TryGetProperty("town", out var townElem))
                        city = townElem.GetString();
                    else if (address.TryGetProperty("village", out var villageElem))
                        city = villageElem.GetString();

                    if (address.TryGetProperty("state", out var stateElem))
                        state = stateElem.GetString();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Reverse geocoding failed: {0}", ex.Message);
            }

            logger.LogInformation("Sending weather response");

            return new WeatherResponse
            {
                TemperatureC = temperatureC,
                WeatherCode = weatherCode,
                ConditionSummary = summary,
                City = city,
                State = state,
                Precipitation = precipitation
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
                45 or 48 => "Fog / Rime fog",
                51 or 53 or 55 => "Drizzle",
                56 or 57 => "Freezing drizzle",
                61 or 63 or 65 => "Rain",
                66 or 67 => "Freezing rain",
                71 or 73 or 75 => "Snowfall",
                77 => "Snow grains",
                80 or 81 or 82 => "Rain showers",
                85 or 86 => "Snow showers",
                95 => "Thunderstorm",
                96 or 99 => "Thunderstorm with hail",
                _ => "Unknown"
            };
        }

        public class WeatherResponse
        {
            public required double TemperatureC { get; set; }
            public required long WeatherCode { get; set; }
            public required string ConditionSummary { get; set; }
            public required string City { get; set; }
            public required string State { get; set; }
            public required double Precipitation { get; set; }
        }
        public class WeatherRequest
        {
            public required double Latitude { get; set; }
            public required double Longitude { get; set; }
        }
    }
}
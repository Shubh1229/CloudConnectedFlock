

namespace HubService.DTO {
    public class WeatherDTO{
        public required double Temp {get; set;}
        public required string Condition {get; set;}
        public required long Code {get; set;}

        public required string City {get; set;}

        public required string State {get; set;}
    }
}
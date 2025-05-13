

using Grpc.Net.Client;
using WeatherService;

namespace HubService.Services {
    public class GrpcWeatherClient {
        private readonly ILogger<GrpcWeatherClient> logger;
        private readonly WeatherService.WeatherService.WeatherServiceClient grpcWeatherClient;

        public GrpcWeatherClient(ILogger<GrpcWeatherClient> logger, IConfiguration config){
            this.logger = logger;
            var address = config["WeatherService:Address"];
            var channel = GrpcChannel.ForAddress(address);
            grpcWeatherClient = new WeatherService.WeatherService.WeatherServiceClient(channel);
        }

        public async Task<WeatherResponse> WeatherResponse(WeatherRequest request) {
            var response = await grpcWeatherClient.GetForecastAsync(request);
            return response;
        }
    }
}
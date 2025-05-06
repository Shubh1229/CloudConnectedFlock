using HeartBeatService.HeartBeatModels;
using HeartBeatService.Services;
using StackExchange.Redis;

namespace HeartBeatService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();

            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect("redis:6379") 
            );

            builder.Services.AddSingleton<HeartBeatRedisService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //app.MapGrpcService<GreeterService>();
            //app.MapGrpcService<Services.HeartBeatServiceImpl>();

            app.Run();
        }
    }
}
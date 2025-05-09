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
                ConnectionMultiplexer.Connect("online-users-redis-db:6379") 
            );

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Configure(builder.Configuration.GetSection("Kestrel"));
            });

            builder.Services.AddSingleton<HeartBeatRedisService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<HeartBeatServiceImpl>();

            app.Run();
        }
    }
}
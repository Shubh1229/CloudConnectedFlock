using PrivatechatService.PrivateChatData;
using PrivatechatService.PrivateChatHub;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetSection("Redis");
    string redisHost = configuration.GetValue<string>("Host");
    int redisPort = configuration.GetValue<int>("Port");
    return ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
});
builder.Services.AddSignalR()
    .AddStackExchangeRedis("redis:6379");

builder.Services.AddControllers();

builder.Services.AddSingleton<PrivateChatRedisService>();


var app = builder.Build();

app.MapHub<PCHub>("/api/privatechat/hub");
app.MapControllers();

app.Run();


using AccountServer.AccountData;
using Microsoft.EntityFrameworkCore;
using AccountServer.GrpcServices;


var builder = WebApplication.CreateBuilder(args);


// Register the PostgreSQL database context
builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Configure(builder.Configuration.GetSection("Kestrel"));
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpc();


var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<AccountServer.GrpcServices.AccountServiceImpl>();

app.Run();

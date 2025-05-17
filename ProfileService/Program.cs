using Microsoft.EntityFrameworkCore;
using ProfileService.GrpcClient;
using ProfileService.ProfileData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<GrpcAccountClient>();
builder.Services.AddSingleton<GrpcHeartbeatClient>();
builder.WebHost.ConfigureKestrel(options =>
    {
        options.Configure(builder.Configuration.GetSection("Kestrel"));
    });
var jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT secret key is missing from configuration.");
builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "ccflock",
        ValidAudience = "ccflock-client",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero

    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Console.WriteLine("Message Received: " + context.Token?.ToString());
            Console.WriteLine("Message Received: " + context.Token);
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("AUTH FAILURE:");
            Console.WriteLine(context.Exception?.ToString());
            Console.WriteLine("Header was: " + context.Request.Headers["Authorization"]);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT TOKEN VALIDATED SUCCESSFULLY");
            return Task.CompletedTask;
        }
    };

});



var app = builder.Build();

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
    db.Database.Migrate();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

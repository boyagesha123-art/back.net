using HealthyBites.Api.Data;
using Microsoft.EntityFrameworkCore;
using HealthyBites.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        "Server=DESKTOP-8ACL73A;Database=HealthyBitesDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
    )
);

// Register token service
builder.Services.AddSingleton<ITokenService, TokenService>();

// Register weight dynamics service
builder.Services.AddScoped<IWeightDynamicsService, WeightDynamicsService>();

// Configure email delivery
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

var app = builder.Build();
// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthyBites API v1");
    c.RoutePrefix = "swagger"; // خليها فاضية "" لو عايز Swagger يفتح مباشرة
});

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();

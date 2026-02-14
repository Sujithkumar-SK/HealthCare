using Microsoft.EntityFrameworkCore;
using PVHealth.Business.Services;
using PVHealth.Data.Cache;
using PVHealth.Data.Context;
using PVHealth.Data.Repositories;
using PVHealth.Domain.Entities;
using PVHealth.Domain.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp=>ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.AddScoped<RedisCacheService>();
builder.Services.AddScoped<IRepository<Patient>, PatientRepository>();
builder.Services.AddScoped<IRepository<Survey>, SurveyRepository>();
builder.Services.AddScoped<SurveyRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<ILocationService,LocationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var locationservice = scope.ServiceProvider.GetRequiredService<ILocationService>();
    await locationservice.GetCountriesAsync();
}
app.Run();
 

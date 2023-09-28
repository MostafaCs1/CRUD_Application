using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using RepositoryContracts;
using Repositories;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Use serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

//add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

//http logging options
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

//add database service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //connect into sql server
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Build App
var app = builder.Build();

//Check Environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//Enable http loger
app.UseHttpLogging();

// add pdf service
RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

//middlewares
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
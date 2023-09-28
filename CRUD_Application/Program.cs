using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using RepositoryContracts;
using Repositories;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

//add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

//logging
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventLog();
});

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
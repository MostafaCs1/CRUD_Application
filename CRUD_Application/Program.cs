using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using RepositoryContracts;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

//add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

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

// add pdf service
RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

//middlewares
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
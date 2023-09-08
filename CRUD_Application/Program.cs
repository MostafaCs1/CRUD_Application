using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//add services
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPersonsService, PersonsService>();
builder.Services.AddSingleton<ICountriesService, CountriesService>();

//add database service
builder.Services.AddDbContext<PersonDbContext>(options =>
{
    options.UseSqlServer();
});

//Build App
var app = builder.Build();

//Check Environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//middlewares
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
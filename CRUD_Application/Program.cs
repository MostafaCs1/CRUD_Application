using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

//add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();

//Build App
var app = builder.Build();

//middlewares
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
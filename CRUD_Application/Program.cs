using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

//add services
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPersonsService, PersonsService>();
builder.Services.AddSingleton<ICountriesService, CountriesService>();

//Build App
var app = builder.Build();

//middlewares
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
var builder = WebApplication.CreateBuilder(args);

//add services
builder.Services.AddControllersWithViews();

//Build App
var app = builder.Build();

//middlewares
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
using GameServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//signal r server
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapRazorPages();

/**
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GameHub>("/gameHub"); // Ruta de tu Hub
});
*/
//app.MapHub<GameHub>("/salajuego");

app.MapHub<GameHub>("/gamehub");







app.Run();
